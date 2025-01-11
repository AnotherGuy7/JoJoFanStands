using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.WaywardSon
{
    public class WaywardSonStandT2 : StandClass
    {
        public override int HalfStandHeight => 37;          //a simpler version of doing the above

        public override int PunchDamage => 41;
        public override int AltDamage => 50;
        public override int PunchTime => 12;
        public override int TierNumber => 2;
        public override bool CanUseAfterImagePunches => false;
        public override int FistID => FanStandFists.WaywardSonFists;
        public override Vector2 StandOffset => new Vector2(-12, 0f);
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        private const float AttackVacuumRange = 10f * 16f;
        private const float AttackVacuumForce = 0.20f;

        private Vector2 secondaryDirection;
        private bool secondaryAbilityStab = false;
        private int stabNPCTarget = -1;

        public new enum AnimationState
        {
            Idle,
            Attack,
            SecondaryAbility,
            SecondaryAbilityStab,
            Special,
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

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft)
                    {
                        Punch(afterImages: false);
                        currentAnimationState = AnimationState.Attack;
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            if (Main.npc[n].active)
                            {
                                NPC npc = Main.npc[n];
                                bool directionCheck = Projectile.direction == 1 ? npc.Center.X > Projectile.Center.X : npc.Center.X < Projectile.Center.X;
                                float npcDistance = Vector2.Distance(npc.Center, Projectile.Center);
                                if (directionCheck && npcDistance < AttackVacuumRange)
                                {
                                    Vector2 direction = Projectile.Center - npc.Center;
                                    direction.Normalize();
                                    direction *= AttackVacuumForce;
                                    npc.velocity += direction;
                                }
                            }
                        }

                        int amountOfDusts = Main.rand.Next(0, 2 + 1);
                        for (int i = 0; i < amountOfDusts; i++)
                        {
                            float xOffset = Main.rand.Next(0, (int)AttackVacuumRange + 1) * Projectile.direction;
                            float yOffset = (Main.rand.Next(-100, 100 + 1) / 100f) * (xOffset / AttackVacuumRange) * (AttackVacuumRange * 3f / 4f);       //Cone-like shape
                            Vector2 dustPosition = Projectile.Center + new Vector2(xOffset, yOffset);
                            Vector2 dustVelocity = -new Vector2(xOffset, yOffset);
                            dustVelocity.Normalize();
                            dustVelocity *= 3f * (Main.rand.Next(80, 120 + 1) / 100f);
                            int dustIndex = Dust.NewDust(dustPosition, 2, 2, DustID.Cloud);
                            Main.dust[dustIndex].velocity = dustVelocity;
                            Main.dust[dustIndex].noGravity = true;
                        }
                    }
                    else
                    {
                        if (!secondaryAbility)
                        {
                            StayBehind();
                            currentAnimationState = AnimationState.Idle;
                        }
                    }
                    if (Main.mouseRight && !playerHasAbilityCooldown && !secondaryAbility)
                    {
                        stabNPCTarget = -1;
                        secondaryAbility = true;
                        secondaryAbilityStab = false;
                        secondaryDirection = Main.MouseWorld - Projectile.Center;
                        secondaryDirection.Normalize();
                        secondaryDirection *= 12f;
                        Projectile.netUpdate = true;
                    }
                }

                float playerDistance = (player.Center - Projectile.Center).Length();
                if (secondaryAbility)
                {
                    if (!secondaryAbilityStab)
                    {
                        currentAnimationState = AnimationState.SecondaryAbility;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.velocity = secondaryDirection;
                            Projectile.direction = 1;
                            if (Projectile.velocity.X < 0)
                                Projectile.direction = -1;
                        }

                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.CanBeChasedBy(this) && Projectile.Distance(npc.Center) <= 3f * 16f)
                            {
                                if (Projectile.owner == Main.myPlayer)
                                {
                                    stabNPCTarget = n;
                                    secondaryAbilityStab = true;
                                    Projectile.frame = 0;
                                    Projectile.frameCounter = 0;
                                    Projectile.netUpdate = true;
                                    break;
                                }
                            }
                        }
                        if (playerDistance > newMaxDistance * 3)
                        {
                            secondaryAbility = false;
                            Projectile.netUpdate = true;
                        }
                    }
                    else
                    {
                        Projectile.velocity = Vector2.Zero;
                        currentAnimationState = AnimationState.SecondaryAbilityStab;
                        NPC npc = Main.npc[stabNPCTarget];
                        if (!npc.active || npc.life <= 0 || Projectile.frame >= 3)
                        {
                            stabNPCTarget = -1;
                            secondaryAbility = false;
                            secondaryAbilityStab = false;
                            player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(10));
                            Projectile.netUpdate = true;
                            return;
                        }

                        if (Projectile.owner == Main.myPlayer && npc.active && (Projectile.frame == 0 || Projectile.frame == 2) && shootCount <= 0)     //hit frames
                        {
                            shootCount += 4;
                            NPC.HitInfo hitInfo = new NPC.HitInfo()
                            {
                                Damage = newAltDamage,
                                Knockback = 1f,
                                HitDirection = Projectile.direction,
                                Crit = true
                            };
                            npc.StrikeNPC(hitInfo);
                            NetMessage.SendStrikeNPC(npc, hitInfo, Projectile.owner);
                            SoundEngine.PlaySound(SoundID.Item2);
                        }
                    }
                }
                else
                    LimitDistance();
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                BasicPunchAI();
                if (!attacking)
                    currentAnimationState = AnimationState.Idle;
                else
                    currentAnimationState = AnimationState.Attack;
            }
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(secondaryAbilityStab);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            secondaryAbilityStab = reader.ReadBoolean();
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
                PlayAnimation("Dash");
            else if (currentAnimationState == AnimationState.SecondaryAbilityStab)
                PlayAnimation("Slash");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/WaywardSon/WaywardSon_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
            else if (animationName == "Dash")
                AnimateStand(animationName, 2, 15, true);
            else if (animationName == "Slash")
                AnimateStand(animationName, 4, 6, false);
            else if (animationName == "Pose")
                AnimateStand(animationName, 2, 8, true);
        }
    }
}