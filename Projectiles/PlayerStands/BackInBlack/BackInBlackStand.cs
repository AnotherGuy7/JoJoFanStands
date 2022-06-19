using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace JoJoFanStands.Projectiles.PlayerStands.BackInBlack
{
    public class BackInBlackStand : StandClass
    {
        public override float shootSpeed => 16f;
        public override int shootTime => 40;
        public override int projectileDamage => 62;
        public override StandType standType => StandType.Ranged;
        public override int halfStandHeight => 33;
        public override int standOffset => 20;

        private int blackHoleWhoAmI = -1;
        private int wormholeWhoAmI = -1;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            secondaryAbilityFrames = player.ownedProjectileCounts[ModContent.ProjectileType<BlackHole>()] != 0;

            if (shootCount > 0)
                shootCount--;
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BlackHole>()] == 0)
                blackHoleWhoAmI = -1;

            if (!secondaryAbilityFrames)
            {
                StayBehind();
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.position = Main.projectile[blackHoleWhoAmI].Center - new Vector2(0f, -300f);
            }
            /*if (blackHoleWhoAmI == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<BlackHole")] != 0)
            {
                blackHoleWhoAmI = BlackHole.whoAmI;
            }*/

            if (Main.mouseLeft && !secondaryAbilityFrames)
            {
                attackFrames = true;
                if (shootCount <= 0)
                {
                    SoundEngine.PlaySound(SoundID.Item78, Projectile.position);
                    shootCount += newShootTime;
                    Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = new Vector2(0f, 1f);

                    shootVel.Normalize();
                    shootVel *= 1.5f;
                    Vector2 perturbedSpeed = new Vector2(shootVel.X, shootVel.Y);
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<BackInBlackOrb>(), newProjectileDamage, 2f, player.whoAmI);
                    Main.projectile[proj].netUpdate = true;
                    Projectile.netUpdate = true;
                }
            }
            if (!Main.mouseLeft && !secondaryAbilityFrames)
            {
                idleFrames = true;
                attackFrames = false;
            }
            if (Main.mouseRight && shootCount <= 0 && player.whoAmI == Main.myPlayer && !secondaryAbilityFrames)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<TeleportationWormhole>()] == 0)
                {
                    wormholeWhoAmI = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<TeleportationWormhole>(), 0, 0f, player.whoAmI);
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
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BlackHole>()] == 0 && blackHoleWhoAmI == -1)
                {
                    blackHoleWhoAmI = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - 300f, 0f, 0f, ModContent.ProjectileType<BlackHole>(), 0, 0f, player.whoAmI);
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
                PlayAnimation("Pose");
            }
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/BackInBlack/BackInBlack_" + animationName).Value;
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