using Microsoft.Xna.Framework;
using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using System.IO;
using JoJoFanStands.NPCs;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace JoJoFanStands.Projectiles.PlayerStands.SlavesOfFear
{
    public class SlavesOfFearStandT3 : StandClass
    {
        public bool weldFrames = false;

        /*public Vector2 velocityAddition = Vector2.Zero;
        public float mouseDistance = 0f;
        protected float ProjectileSpeed = 16f;
        public bool idleFrames = false;
        public bool attackFrames = false;
        public bool secondaryAbilityFrames = false;
        public float MaxDistance = 0f;
        public int PunchDamage = 47;
        public int AltDamage = 54;*/

        public override int HalfStandHeight => 37;
        public override int PunchDamage => 68;
        public override int AltDamage => 76;
        public override int PunchTime => 11;
        public override int TierNumber => 3;
        public override StandAttackType StandType => StandAttackType.Melee;

        private bool welding = false;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            Projectile.frameCounter++;
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (Main.mouseLeft)
            {
                attackFrames = true;
                Punch();
            }
            else
            {
                if (!secondaryAbilityFrames && !weldFrames)
                {
                    idleFrames = true;
                    StayBehind();
                }
            }

            if (Main.mouseRight)
                secondaryAbilityFrames = true;

            if (SpecialKeyPressed(false))
                welding = !welding;
            weldFrames = welding;
            if (weldFrames)
            {
                secondaryAbilityFrames = false;
                attackFrames = false;
                idleFrames = false;
                if (Projectile.Distance(Main.MouseWorld) > 40f)
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
                    if (npc.active && Projectile.Distance(npc.Center) <= 40f)
                    {
                        npc.GetGlobalNPC<FanGlobalNPC>().welded = true;
                        npc.GetGlobalNPC<FanGlobalNPC>().weldMaxTimer = 10 * 60;
                    }
                }
            }
            Vector2 direction = player.Center - Projectile.Center;
            float distanceTo = direction.Length();
            if (secondaryAbilityFrames)
            {
                idleFrames = false;
                attackFrames = false;
                Projectile.velocity.X = 10f * Projectile.direction;
                Projectile.position.Y = player.position.Y;
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && Projectile.Distance(npc.Center) <= 15f)
                    {
                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                        {
                            Damage = AltDamage,
                            Knockback = 8f,
                            HitDirection = Projectile.direction
                        };
                        npc.StrikeNPC(hitInfo);
                    }
                }
                if (distanceTo > newMaxDistance * 2)
                    secondaryAbilityFrames = false;
            }
            else
            {
                LimitDistance();
            }
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(weldFrames);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            weldFrames = reader.ReadBoolean();
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                idleFrames = false;
                PlayAnimation("Attack");
            }
            if (idleFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
            if (secondaryAbilityFrames)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Secondary");
            }
            if (weldFrames)
            {
                attackFrames = false;
                idleFrames = false;
                secondaryAbility = false;
                PlayAnimation("Weld");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/SlavesOfFear/SlavesOfFear_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 2, 14, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 4, newPunchTime, true);
            }
            if (animationName == "Secondary")
            {
                AnimateStand(animationName, 1, 15, true);
            }
            if (animationName == "Weld")
            {
                AnimateStand(animationName, 1, 15, true);
            }
        }
    }
}