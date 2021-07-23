using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;

namespace JoJoFanStands.Projectiles.PlayerStands.BackInBlack
{
    public class BackInBlackStand : StandClass
    {
        public override float shootSpeed => 16f;
        public override int shootTime => 40;
        public override int projectileDamage => 62;
        public override int standType => 2;
        public override int halfStandHeight => 33;
        public override int standOffset => 20;

        private int blackHoleWhoAmI = -1;
        private int wormholeWhoAmI = -1;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            secondaryAbilityFrames = player.ownedProjectileCounts[mod.ProjectileType("BlackHole")] != 0;
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
            if (player.ownedProjectileCounts[mod.ProjectileType("BlackHole")] == 0)
            {
                blackHoleWhoAmI = -1;
            }
            if (!secondaryAbilityFrames)
            {
                StayBehind();
            }
            else
            {
                projectile.velocity = Vector2.Zero;
                projectile.position = Main.projectile[blackHoleWhoAmI].Center - new Vector2(0f, -300f);
            }
            /*if (blackHoleWhoAmI == 0 && player.ownedProjectileCounts[mod.ProjectileType("BlackHole")] != 0)
            {
                blackHoleWhoAmI = BlackHole.whoAmI;
            }*/

            if (Main.mouseLeft && !secondaryAbilityFrames)
            {
                attackFrames = true;
                if (shootCount <= 0)
                {
                    Main.PlaySound(SoundID.Item78, projectile.position);
                    shootCount += newShootTime;
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    shootVel.Normalize();
                    shootVel *= 1.5f;
                    Vector2 perturbedSpeed = new Vector2(shootVel.X, shootVel.Y);
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("BackInBlackOrb"), newProjectileDamage, 2f, player.whoAmI);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                }
            }
            if (!Main.mouseLeft && !secondaryAbilityFrames)
            {
                normalFrames = true;
                attackFrames = false;
            }
            if (Main.mouseRight && shootCount <= 0 && player.whoAmI == Main.myPlayer && !secondaryAbilityFrames)
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("TeleportationWormhole")] == 0)
                {
                    wormholeWhoAmI = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, mod.ProjectileType("TeleportationWormhole"), 0, 0f, player.whoAmI);
                    shootCount += 60;
                }
                else
                {
                    Projectile wormhole = Main.projectile[wormholeWhoAmI];
                    player.position = wormhole.position;
                    wormholeWhoAmI = -1;
                    wormhole.Kill();
                    shootCount += 60;
                }
            }
            if (SpecialKeyCurrent())
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("BlackHole")] == 0 && blackHoleWhoAmI == -1)
                {
                    blackHoleWhoAmI = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 300f, 0f, 0f, mod.ProjectileType("BlackHole"), 0, 0f, player.whoAmI);
                    Main.projectile[blackHoleWhoAmI].scale = 0.05f;
                    Main.projectile[blackHoleWhoAmI].netUpdate = true;
                }
                else
                {
                    Main.projectile[blackHoleWhoAmI].scale += 0.003f;
                    Main.projectile[blackHoleWhoAmI].timeLeft += 2;
                }
            }
            if (!SpecialKeyCurrent() && secondaryAbilityFrames)
            {
                Main.projectile[blackHoleWhoAmI].scale -= 0.005f;
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
            standTexture = mod.GetTexture("Projectiles/PlayerStands/BackInBlack/BackInBlack_" + animationName);
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 2, 14, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 6, 8, true);
            }
            if (animationName == "Pose")
            {
                AnimateStand(animationName, 2, 15, true);
            }
        }
    }
}