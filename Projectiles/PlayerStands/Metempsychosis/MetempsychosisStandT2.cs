using JoJoFanStands.Buffs;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles.PlayerStands;
using JoJoStands.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static JoJoFanStands.Projectiles.PlayerStands.Metempsychosis.MetempsychosisStandFinal;

namespace JoJoFanStands.Projectiles.PlayerStands.Metempsychosis
{
    public class MetempsychosisStandT2 : StandClass
    {
        public override int HalfStandHeight => 43;
        public override int PunchDamage => 43;
        public override int AltDamage => 50;
        public override int PunchTime => 11;
        public override int TierNumber => 2;
        public override int FistID => FanStandFists.MetempsychosisFists;
        public override bool CanUseAfterImagePunches => false;
        public override Vector2 StandOffset => new Vector2(-12f, 0f);
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        private int robeGlowmaskTimer = 0;
        private int weaponGlowmaskTimer = 0;
        private bool claimingSouls = false;
        private bool soulsClaimed = false;
        private bool playedDeathLine = false;

        public new enum AnimationState
        {
            Idle,
            Attack,
            SecondaryAbility,
            Special1,
            Pose
        }

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            shootCount--;
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;
            if (player.dead && !playedDeathLine)
            {
                playedDeathLine = true;
                SoundEngine.PlaySound(Main.rand.NextBool() ? Death_1 : Death_2, Projectile.Center);
            }
            if (!player.dead && playedDeathLine)
            {
                playedDeathLine = false;
                SoundEngine.PlaySound(Main.rand.NextBool() ? Revival_1 : Revival_2, Projectile.Center);
            }

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

            if (player.HasBuff<GirdSoul>())
            {
                robeGlowmaskTimer = 270;
                weaponGlowmaskTimer = 270;
            }

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft)
                    {
                        Punch(ModContent.ProjectileType<FanStandFists>(), Main.MouseWorld, afterImages: false);
                        currentAnimationState = AnimationState.Attack;
                    }
                    else
                    {
                        attacking = false;
                        currentAnimationState = AnimationState.Idle;
                        if (!secondaryAbility && !claimingSouls)
                            StayBehind();
                    }
                    if (Main.mouseRight && !playerHasAbilityCooldown && !attacking && !claimingSouls)
                    {
                        secondaryAbility = true;
                        currentAnimationState = AnimationState.SecondaryAbility;
                        SoundEngine.PlaySound(Main.rand.NextBool() ? Claim_1 : Claim_2, Projectile.Center);
                    }
                    if (SpecialKeyPressed() && fPlayer.metempsychosisPoints >= 25 && !attacking && !secondaryAbility && !claimingSouls)
                    {
                        claimingSouls = true;
                        soulsClaimed = false;
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                        Projectile.netUpdate = true;
                        fPlayer.metempsychosisPoints -= 25;
                        SoundEngine.PlaySound(SoundID.Item8.WithPitchOffset(-0.8f), Projectile.Center);
                        SoundEngine.PlaySound(Main.rand.NextBool() ? Claim_1 : Claim_2, Projectile.Center);
                    }
                }

                if (claimingSouls)      //Special 1
                {
                    GoInFront();
                    currentAnimationState = AnimationState.Special1;
                    float lightStrength = 2.5f;
                    Lighting.AddLight(Projectile.Center, 63 * lightStrength / 255f, 205 * lightStrength / 255f, 189 * lightStrength / 255f);
                    if (Projectile.frame == 2 && !soulsClaimed)
                    {
                        soulsClaimed = true;
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(30 / TierNumber));
                        if (Main.myPlayer == Projectile.owner)
                        {
                            for (int n = 0; n < Main.maxNPCs; n++)
                            {
                                NPC npc = Main.npc[n];
                                if (npc.CanBeChasedBy(this) && Projectile.Distance(npc.Center) <= newMaxDistance * 4)
                                {
                                    int newHealth = (int)(npc.life * (1f - (0.05f * TierNumber))) - npc.defense;
                                    if (npc.boss || newHealth > 0.25f * npc.lifeMax)
                                    {
                                        int damage = npc.life - newHealth;
                                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                                        {
                                            Damage = damage,
                                            Knockback = 0f,
                                            HitDirection = -Projectile.direction,
                                        };
                                        npc.StrikeNPC(hitInfo);
                                        NetMessage.SendStrikeNPC(npc, hitInfo, Projectile.owner);
                                    }
                                    else
                                    {
                                        npc.StrikeInstantKill();
                                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<SoulEffect>(), 0, 0f, Projectile.owner, player.statLifeMax2 * 0.05f);
                                    }
                                }
                            }
                        }
                    }
                }

                Vector2 direction = player.Center - Projectile.Center;
                float distanceTo = direction.Length();
                if (secondaryAbility)
                {
                    currentAnimationState = AnimationState.SecondaryAbility;
                    Projectile.velocity.X = 12f * Projectile.direction;
                    Projectile.position.Y = player.position.Y + HalfStandHeight / 2;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.CanBeChasedBy(this) && Projectile.Distance(npc.Center) <= 3f * 16f)
                            {
                                if (Projectile.owner == Main.myPlayer)
                                {
                                    NPC.HitInfo hitInfo = new NPC.HitInfo()
                                    {
                                        Damage = (int)(newAltDamage * (1.05f * TierNumber)),
                                        Knockback = 6f,
                                        HitDirection = Projectile.direction
                                    };
                                    npc.StrikeNPC(hitInfo);
                                    NetMessage.SendStrikeNPC(npc, hitInfo, Projectile.owner);
                                }
                            }
                        }
                    }
                    if (distanceTo > newMaxDistance * 2.5f)
                    {
                        secondaryAbility = false;
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(15 / TierNumber));
                    }
                }
                
                if (!secondaryAbility)
                    LimitDistance();

                if (mPlayer.posing)
                    currentAnimationState = AnimationState.Pose;
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                BasicPunchAI();
                if (attacking)
                    currentAnimationState = AnimationState.Attack;
                else
                    currentAnimationState = AnimationState.Idle;

                if (SpecialKeyPressed() && fPlayer.metempsychosisPoints >= 10 && !player.HasBuff<GirdSoul>())
                {
                    robeGlowmaskTimer = 180;
                    weaponGlowmaskTimer = 180;
                    player.AddBuff(ModContent.BuffType<GirdSoul>(), 10 * 60);
                    SoundEngine.PlaySound(Main.rand.NextBool() ? GirdSoul_1 : GirdSoul_2, Projectile.Center);
                }
            }
        }


        public override void ExtraSpawnEffects()
        {
            SoulBar.ShowSoulBar();
            SoundEngine.PlaySound(Main.rand.NextBool() ? Summon_1 : Summon_2, Projectile.Center);
        }

        public override void StandKillEffects()
        {
            SoulBar.HideSoulBar();
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(claimingSouls);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            claimingSouls = reader.ReadBoolean();
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

        public override void AnimationCompleted(string animationName)
        {
            if (animationName == "Claim")
            {
                claimingSouls = false;
                soulsClaimed = false;
                currentAnimationState = AnimationState.Idle;
                Main.player[Projectile.owner].AddBuff(ModContent.BuffType<AbilityCooldown>(), Main.player[Projectile.owner].GetModPlayer<MyPlayer>().AbilityCooldownTime(30 / TierNumber));
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
                AnimateStand(animationName, 4, 15, false);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 500, true);
        }
    }
}