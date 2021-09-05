using Microsoft.Xna.Framework;
using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using System.IO;
using JoJoFanStands.NPCs;

namespace JoJoFanStands.Projectiles.PlayerStands.SlavesOfFear
{
    public class SlavesOfFearStandFinal : StandClass
    {
        public override int halfStandHeight => 37;
        public override int punchDamage => 98;
        public override int altDamage => 112;
        public override int punchTime => 10;
        public override int standType => 1;

        private bool weldFrames = false;


        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
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
            weldFrames = JoJoStands.JoJoStands.SpecialHotKey.Current;
            if (weldFrames)
            {
                secondaryAbilityFrames = false;
                if (projectile.Distance(Main.MouseWorld) > 2f * 16f)
                {
                    projectile.velocity = Main.MouseWorld - projectile.Center;
                    projectile.velocity.Normalize();
                    projectile.velocity *= 2f;
                }
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && projectile.Distance(npc.Center) <= 3f * 16f)
                    {
                        npc.GetGlobalNPC<FanGlobalNPC>().welded = true;
                        npc.GetGlobalNPC<FanGlobalNPC>().weldMaxTimer = 15 * 60;
                    }
                }
            }
            Vector2 direction = player.Center - projectile.Center;
            float distanceTo = direction.Length();
            if (secondaryAbilityFrames)
            {
                projectile.velocity.X = 10f * projectile.direction;
                projectile.position.Y = player.position.Y;
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && projectile.Distance(npc.Center) <= 15f)
                    {
                        npc.StrikeNPC(altDamage, 8f, projectile.direction);
                    }
                }
                if (distanceTo > maxDistance * 2)
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