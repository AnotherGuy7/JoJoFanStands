using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using JoJoFanStands.Buffs;
using static Terraria.ModLoader.ModContent;
using JoJoFanStands.NPCs;

namespace JoJoFanStands.Projectiles.PlayerStands.Banks
{
    public class BanksStandFinal : StandClass
    {
        public override float shootSpeed => 16f;
        public override int shootTime => 6;
        public override int projectileDamage => 17;
        public override int standType => 2;
        public override int halfStandHeight => 32;
        public override int standOffset => 0;


        private const float TargetDetectionRange = 32f * 16f;
        private NPC target = null;
        private int shotgunChargeTimer = 0;
        private bool riskyRewardsActive = false;
        private int specialPressTimer = 0;
        private float coinMultiplier = 1f;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            //Lighting.AddLight((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f), 0.6f, 0.9f, 0.3f);
            //Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 35, projectile.velocity.X * -0.5f, projectile.velocity.Y * -0.5f);

            if (shootCount > 0)
                shootCount--;
            if (specialPressTimer > 0)
                specialPressTimer--;
            if (mPlayer.standOut)
                projectile.timeLeft = 2;


            if (Main.mouseLeft && player.whoAmI == projectile.owner)
            {
                if (target == null)
                {
                    target = FindNearestTarget(TargetDetectionRange);
                }
                else
                {
                    attackFrames = true;
                    projectile.position = target.Center + new Vector2(((target.width / 2f) + projectile.width) * -target.direction, 0f);
                    projectile.direction = target.direction;
                    if (projectile.Distance(target.Center) >= TargetDetectionRange)
                    {
                        target = null;
                    }

                    if (shootCount <= 0 && projectile.frame == 2)
                    {
                        shootCount += shootTime;
                        target.StrikeNPC(newProjectileDamage, 0.2f, projectile.direction);
                        Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 41, 1f, 3f);
                    }
                    target.GetGlobalNPC<FanGlobalNPC>().banksCoinMultiplier = coinMultiplier;
                }
            }
            if (Main.mouseRight && player.whoAmI == projectile.owner && !attackFrames)
            {
                if (target == null)
                {
                    shotgunChargeTimer = 0;
                    target = FindNearestTarget(TargetDetectionRange);
                }
                else
                {
                    secondaryAbilityFrames = true;
                    projectile.Center = target.Center + new Vector2(((target.width / 2f) + projectile.width) * -target.direction, 0f);
                    projectile.direction = target.direction;
                    if (projectile.Distance(target.Center) >= TargetDetectionRange)
                    {
                        target = null;
                    }

                    shotgunChargeTimer++;
                    if (shotgunChargeTimer >= 60)
                    {
                        shootCount += newShootTime;
                        Vector2 shootVel = target.Center - projectile.Center;
                        shootVel.Normalize();
                        shootVel *= shootSpeed;

                        float numberProjectiles = 6;
                        float rotation = MathHelper.ToRadians(30f);
                        float random = Main.rand.NextFloat(-6f, 6f + 1f);
                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            Vector2 perturbedSpeed = new Vector2(shootVel.X + random, shootVel.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                            int proj = Projectile.NewProjectile(projectile.Center, perturbedSpeed, ProjectileID.Bullet, newProjectileDamage * 2, 1f, player.whoAmI);
                            Main.projectile[proj].netUpdate = true;
                        }
                        shotgunChargeTimer = 0;
                        Main.PlaySound(2, projectile.position, 36);
                    }
                    target.GetGlobalNPC<FanGlobalNPC>().banksCoinMultiplier = coinMultiplier;
                }
            }
            if (!Main.mouseLeft && !Main.mouseRight)
            {
                normalFrames = true;
                attackFrames = false;
                secondaryAbilityFrames = false;
                target = null;
            }
            riskyRewardsActive = player.HasBuff(BuffType<RiskyRewards>());
            if (SpecialKeyPressedNoCooldown() && specialPressTimer <= 0)
            {
                if (riskyRewardsActive)
                {
                    fPlayer.banksDefenseReduction = 0;
                    coinMultiplier = 1f;
                    player.ClearBuff(BuffType<RiskyRewards>());
                }
                else
                {
                    fPlayer.banksDefenseReduction = 12;
                    coinMultiplier = 2f;
                    player.AddBuff(BuffType<RiskyRewards>(), 2);
                }
                specialPressTimer += 30;
            }

            if (!attackFrames && !secondaryAbilityFrames)
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
                PlayAnimation("Secondary");
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
                AnimateStand(animationName, 4, 15, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 5, shootTime / 5, true);
            }
            if (animationName == "Secondary")
            {
                AnimateStand(animationName, 1, 60, true);
            }
            if (animationName == "Pose")
            {
                AnimateStand(animationName, 1, 300, true);
            }
        }
    }
}