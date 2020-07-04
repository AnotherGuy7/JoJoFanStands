using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;

namespace JoJoFanStands.Projectiles.PlayerStands
{
    public class SlavesOfFearStandT2 : StandClass
    {
        public override float maxDistance       
        {
            get { return 98f; }
        }

        public override int halfStandHeight => 37;          //a simpler version of doing the above

        public override int punchDamage => 47;
        public override int altDamage => 54;

        public override int punchTime => 12;
        public override int standType => 1;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            shootCount--;
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }

            if (Main.mouseLeft)
            {
                Punch();
            }
            else
            {
                if (!secondaryAbilityFrames)
                {
                    normalFrames = true;
                    StayBehind();
                }
            }
            if (Main.mouseRight)
            {
                secondaryAbilityFrames = true;
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
        }
    }
}