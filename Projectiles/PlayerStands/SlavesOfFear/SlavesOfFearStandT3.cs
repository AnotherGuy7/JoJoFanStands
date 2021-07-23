using Microsoft.Xna.Framework;
using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using System.IO;
using JoJoFanStands.NPCs;

namespace JoJoFanStands.Projectiles.PlayerStands.SlavesOfFear
{
    public class SlavesOfFearStandT3 : StandClass
    {
        public bool weldFrames = false;

        /*public Vector2 velocityAddition = Vector2.Zero;
        public float mouseDistance = 0f;
        protected float shootSpeed = 16f;
        public bool normalFrames = false;
        public bool attackFrames = false;
        public bool secondaryAbilityFrames = false;
        public float maxDistance = 0f;
        public int punchDamage = 47;
        public int altDamage = 54;*/

        public override int halfStandHeight => 37;
        public override int punchDamage => 68;
        public override int altDamage => 76;
        public override int punchTime => 11;
        public override int standType => 1;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            projectile.frameCounter++;
            if (mPlayer.StandOut)
                projectile.timeLeft = 2;

            if (Main.mouseLeft)
            {
                attackFrames = true;
                Punch();
            }
            else
            {
                if (!secondaryAbilityFrames && !weldFrames)
                {
                    normalFrames = true;
                    StayBehind();
                }
            }

            if (Main.mouseRight)
            {
                secondaryAbilityFrames = true;
            }
            weldFrames = SpecialKeyCurrent();
            if (weldFrames)
            {
                secondaryAbilityFrames = false;
                attackFrames = false;
                normalFrames = false;
                if (projectile.Distance(Main.MouseWorld) > 40f)
                {
                    projectile.velocity = Main.MouseWorld - projectile.Center;
                    projectile.velocity.Normalize();
                    projectile.velocity *= 2f;
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && projectile.Distance(npc.Center) <= 40f)
                    {
                        npc.GetGlobalNPC<FanGlobalNPC>().welded = true;
                        npc.GetGlobalNPC<FanGlobalNPC>().weldMaxTimer = 600;
                    }
                }
            }
            Vector2 direction = player.Center - projectile.Center;
            float distanceTo = direction.Length();
            if (secondaryAbilityFrames)
            {
                normalFrames = false;
                attackFrames = false;
                projectile.velocity.X = 10f * projectile.direction;
                projectile.position.Y = player.position.Y;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && projectile.Distance(npc.Center) <= 15f)
                    {
                        npc.StrikeNPC(altDamage, 8f, projectile.direction);
                    }
                }
                if (distanceTo > newMaxDistance * 2)
                {
                    secondaryAbilityFrames = false;
                }
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
                normalFrames = false;
                PlayAnimation("Attack");
            }
            if (normalFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
            if (secondaryAbilityFrames)
            {
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Secondary");
            }
            if (weldFrames)
            {
                attackFrames = false;
                normalFrames = false;
                secondaryAbility = false;
                PlayAnimation("Weld");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = mod.GetTexture("Projectiles/PlayerStands/SlavesOfFear/SlavesOfFear_" + animationName);
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
                AnimateStand(animationName, 2, 10, true);
            }
            if (animationName == "Weld")
            {
                AnimateStand(animationName, 4, 13, true);
            }
        }
    }
}