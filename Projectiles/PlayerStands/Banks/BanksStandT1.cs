using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;

namespace JoJoFanStands.Projectiles.PlayerStands.Banks
{
    public class BanksStandT1 : StandClass
    {
        public override float shootSpeed => 16f;
        public override int shootTime => 12;
        public override int projectileDamage => 5;
        public override int standType => 2;
        public override int halfStandHeight => 32;
        public override int standOffset => 20;

        private NPC target = null;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            //Lighting.AddLight((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f), 0.6f, 0.9f, 0.3f);
            //Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 35, projectile.velocity.X * -0.5f, projectile.velocity.Y * -0.5f);

            if (shootCount > 0)
            {
                shootCount--;
            }
            if (mPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }

            if (Main.mouseLeft && player.whoAmI == projectile.owner)
            {
                attackFrames = true;
                if (target == null)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (npc.active && npc.lifeMax > 5 && !npc.townNPC && !npc.immortal && !npc.hide && projectile.Distance(npc.Center) <= 32f * 16f)
                        {
                            target = npc;
                            break;
                        }
                    }
                }
                else
                {
                    projectile.position = target.Center + new Vector2(((target.width / 2f) + projectile.width) * -target.direction, 0f);
                    projectile.direction = target.direction;

                    if (shootCount <= 0 && projectile.frame == 2)
                    {
                        shootCount += shootTime;
                        target.StrikeNPC(newProjectileDamage, 0.2f, projectile.direction);
                    }
                }
            }
            else
            {
                attackFrames = false;
                normalFrames = true;
                target = null;
            }

            if (!attackFrames)
            {
                StayBehind();
            }
            projectile.spriteDirection = projectile.direction;
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
                PlayAnimation("Pose");
            }
            if (Main.player[projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = mod.GetTexture("Projectiles/PlayerStands/Banks/BanksStand_" + animationName);
            if (animationName == "Idle")
            {
                AnimationStates(animationName, 4, 15, true);
            }
            if (animationName == "Attack")
            {
                AnimationStates(animationName, 5, shootTime / 5, true);
            }
            if (animationName == "Pose")
            {
                AnimationStates(animationName, 1, 300, true);
            }
        }
    }
}