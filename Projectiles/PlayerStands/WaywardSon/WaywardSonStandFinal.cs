using JoJoFanStands.NPCs;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.WaywardSon
{
    public class WaywardSonStandFinal : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 98;
        public override int AltDamage => 112;
        public override int PunchTime => 10;
        public override int TierNumber => 4;
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;

        private const float AttackVacuumRange = 14f * 16f;
        private const float AttackVacuumForce = 0.3f;

        private bool welding = false;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

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

                    int amountOfDusts = Main.rand.Next(1, 3 + 1);
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
                if (Main.mouseRight)
                {
                    secondaryAbility = true;
                    currentAnimationState = AnimationState.SecondaryAbility;
                }
            }


            if (SpecialKeyPressed(false))
                welding = !welding;
            if (welding)
            {
                secondaryAbility = false;
                currentAnimationState = AnimationState.Special;
                if (Projectile.Distance(Main.MouseWorld) > 2f * 16f)
                {
                    Projectile.velocity = Main.MouseWorld - Projectile.Center;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 6f;
                    if (Projectile.velocity.X > 0)
                        Projectile.direction = 1;
                    else
                        Projectile.direction = -1;
                    Projectile.spriteDirection = Projectile.direction;
                }

                int dustIndex = Dust.NewDust(Projectile.Center + new Vector2(8 * Projectile.spriteDirection, 0f) - new Vector2(3), 6, 6, DustID.IceTorch);
                Main.dust[dustIndex].noGravity = true;

                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && Projectile.Distance(npc.Center) <= 3f * 16f)
                    {
                        npc.GetGlobalNPC<FanGlobalNPC>().welded = true;
                        npc.GetGlobalNPC<FanGlobalNPC>().weldMaxTimer = 15 * 60;
                    }
                }
            }
            float distanceTo = (player.Center - Projectile.Center).Length();
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
                if (distanceTo > MaxDistance * 2)
                    secondaryAbility = false;
            }
            else
                LimitDistance();
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(welding);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            welding = reader.ReadBoolean();
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
            else if (currentAnimationState == AnimationState.Special)
                PlayAnimation("Weld");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/WaywardSon/WaywardSon_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 2, 14, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
            else if (animationName == "Secondary")
                AnimateStand(animationName, 1, 15, true);
            else if (animationName == "Weld")
                AnimateStand(animationName, 1, 15, true);
        }
    }
}