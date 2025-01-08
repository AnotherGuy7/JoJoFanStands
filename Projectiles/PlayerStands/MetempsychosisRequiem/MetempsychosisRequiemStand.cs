using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.SlavesOfFear
{
    public class MetempsychosisRequiemStand : StandClass
    {
        public override int HalfStandHeight => 43;
        public override int PunchDamage => 131;
        public override int AltDamage => 162;
        public override int PunchTime => 8;
        public override int TierNumber => 5;
        public override bool CanUseAfterImagePunches => false;
        public override Vector2 StandOffset => new Vector2(-12f, 0f);
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        private int robeGlowmaskTimer = 0;
        private int weaponGlowmaskTimer = 0;

        public new enum AnimationState
        {
            Idle,
            Attack,
            SecondaryAbility,
            Special1,
            Special2,
            Pose
        }

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            shootCount--;
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (robeGlowmaskTimer < 360)
            {
                robeGlowmaskTimer += 1;
                if (robeGlowmaskTimer >= 360)
                    robeGlowmaskTimer -= 360;
            }

            if (weaponGlowmaskTimer < 360)
            {
                weaponGlowmaskTimer += 2;
                if (weaponGlowmaskTimer >= 360)
                    weaponGlowmaskTimer -= 360;
            }


            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.mouseLeft)
                {
                    Punch(afterImages: false);
                    currentAnimationState = AnimationState.Attack;
                }
                else
                {
                    currentAnimationState = AnimationState.Idle;
                    if (!secondaryAbility)
                        StayBehind();
                }
                if (Main.mouseRight)
                {
                    secondaryAbility = true;
                    currentAnimationState = AnimationState.SecondaryAbility;
                }
            }

            Vector2 direction = player.Center - Projectile.Center;
            float distanceTo = direction.Length();
            if (secondaryAbility)
            {
                currentAnimationState = AnimationState.SecondaryAbility;
                Projectile.velocity.X = 10f * Projectile.direction;
                Projectile.position.Y = player.position.Y;
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && Projectile.Distance(npc.Center) <= 15f)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            NPC.HitInfo hitInfo = new NPC.HitInfo()
                            {
                                Damage = AltDamage,
                                Knockback = 8f,
                                HitDirection = Projectile.direction
                            };
                            npc.StrikeNPC(hitInfo);
                            NetMessage.SendStrikeNPC(npc, hitInfo, Main.myPlayer);
                        }
                    }
                }
                if (distanceTo > newMaxDistance * 2)
                    secondaryAbility = false;
            }
            else
                LimitDistance();

            if (mPlayer.posing)
                currentAnimationState = AnimationState.Pose;
        }

        public override void PostDrawExtras()
        {
            string animationName = string.Empty;
            if (currentAnimationState == AnimationState.Idle)
                animationName = "Idle";
            else if (currentAnimationState == AnimationState.Attack)
                animationName = "Attack";
            else if (currentAnimationState == AnimationState.SecondaryAbility)
                animationName = "Reave";
            else if (currentAnimationState == AnimationState.Special1)
                animationName = "Claim";
            else if (currentAnimationState == AnimationState.Special2)
                animationName = "Rend";
            else if (currentAnimationState == AnimationState.Pose)
                animationName = "Pose";

            Texture2D robeGlowmask = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Metempsychosis/Metempsychosis_" + animationName + "_Glowmask").Value;
            Texture2D weaponGlowmask = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Metempsychosis/Metempsychosis_" + animationName + "_Weapon").Value;
            float robeAlpha = (0.4f * (float)Math.Abs(Math.Sin(Math.Tau * (robeGlowmaskTimer / 360f)))) + 0.2f;
            float weaponAlpha = (0.9f * (float)Math.Abs(Math.Sin(Math.Tau * (weaponGlowmaskTimer / 360f)))) + 0.5f;

            effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;

            if (standTexture != null && Main.netMode != NetmodeID.Server)
            {
                int frameHeight = standTexture.Height / amountOfFrames;
                Vector2 drawOffset = StandOffset;
                drawOffset.X *= Projectile.spriteDirection;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                Rectangle animRect = new Rectangle(0, frameHeight * Projectile.frame, standTexture.Width, frameHeight);
                Vector2 standOrigin = new Vector2(standTexture.Width / 2f, frameHeight / 2f);
                Main.EntitySpriteDraw(robeGlowmask, drawPosition, animRect, Color.White * robeAlpha, Projectile.rotation, standOrigin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(weaponGlowmask, drawPosition, animRect, Color.White * weaponAlpha, Projectile.rotation, standOrigin, Projectile.scale, effects, 0);
            }
        }

        public override void SelectAnimation()
        {
            if (oldAnimationState != currentAnimationState)
            {
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
                oldAnimationState = currentAnimationState;
                Projectile.netUpdate = true;
            }


            if (currentAnimationState == AnimationState.Idle)
                PlayAnimation("Idle");
            else if (currentAnimationState == AnimationState.Attack)
                PlayAnimation("Attack");
            else if (currentAnimationState == AnimationState.SecondaryAbility)
                PlayAnimation("Reave");
            else if (currentAnimationState == AnimationState.Special1)
                PlayAnimation("Claim");
            else if (currentAnimationState == AnimationState.Special2)
                PlayAnimation("Rend");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Metempsychosis/Metempsychosis_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
            else if (animationName == "Reave")
                AnimateStand(animationName, 2, 15, true);
            else if (animationName == "Claim")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Rend")
                AnimateStand(animationName, 2, 15, true);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 500, true);
        }
    }
}