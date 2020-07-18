using Microsoft.Xna.Framework;
using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using System.IO;

namespace JoJoFanStands.Projectiles.PlayerStands
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
            {
                shootCount--;
            }
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }

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
                        npc.GetGlobalNPC<NPCs.FanGlobalNPC>().welded = true;
                        npc.GetGlobalNPC<NPCs.FanGlobalNPC>().weldMaxTimer = 900;
                    }
                }
            }
            Vector2 direction = player.Center - projectile.Center;
            float distanceTo = direction.Length();
            if (secondaryAbilityFrames)
            {
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
                if (distanceTo > maxDistance * 2)
                {
                    secondaryAbilityFrames = false;
                }
            }
            else
            {
                if (distanceTo > maxDistance)
                {
                    if (projectile.position.X <= player.position.X - 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X + 0.2f, projectile.position.Y);
                        projectile.velocity = Vector2.Zero;
                    }
                    if (projectile.position.X >= player.position.X + 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X - 0.2f, projectile.position.Y);
                        projectile.velocity = Vector2.Zero;
                    }
                    if (projectile.position.Y >= player.position.Y + 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X, projectile.position.Y - 0.2f);
                        projectile.velocity = Vector2.Zero;
                    }
                    if (projectile.position.Y <= player.position.Y - 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X, projectile.position.Y + 0.2f);
                        projectile.velocity = Vector2.Zero;
                    }
                }
                if (distanceTo >= maxDistance + 22f)
                {
                    Main.mouseLeft = false;
                    Main.mouseRight = false;
                    projectile.Center = player.Center;
                }
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
                AnimationStates(animationName, 2, 14, true);
            }
            if (animationName == "Attack")
            {
                AnimationStates(animationName, 4, newPunchTime, true);
            }
            if (animationName == "Secondary")
            {
                AnimationStates(animationName, 2, 10, true);
            }
            if (animationName == "Weld")
            {
                AnimationStates(animationName, 4, 13, true);
            }
        }
    }
}