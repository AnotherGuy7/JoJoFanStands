using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.SlavesOfFear
{
    public class SlavesOfFearStandT2 : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 47;
        public override int AltDamage => 54;
        public override int PunchTime => 12;
        public override int TierNumber => 2;
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            shootCount--;
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.mouseLeft)
                    Punch(afterImages: false);
                else
                {
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
                    if (npc.active && npc.lifeMax > 5 && npc.CanBeChasedBy(this) && Projectile.Distance(npc.Center) <= 15f)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            NPC.HitInfo hitInfo = new NPC.HitInfo()
                            {
                                Damage = newAltDamage,
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
            {
                LimitDistance();
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
                PlayAnimation("Secondary");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/SlavesOfFear/SlavesOfFear_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 2, 14, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
            else if (animationName == "Secondary")
                AnimateStand(animationName, 1, 15, true);
        }
    }
}