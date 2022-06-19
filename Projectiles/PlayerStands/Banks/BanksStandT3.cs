using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using JoJoFanStands.Buffs;
using static Terraria.ModLoader.ModContent;
using JoJoFanStands.NPCs;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace JoJoFanStands.Projectiles.PlayerStands.Banks
{
    public class BanksStandT3 : StandClass
    {
        public override float shootSpeed => 16f;
        public override int shootTime => 8;
        public override int projectileDamage => 10;
        public override StandType standType => StandType.Ranged;
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
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            riskyRewardsActive = player.HasBuff(BuffType<RiskyRewards>());
            //Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), 0.6f, 0.9f, 0.3f);
            //Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 35, Projectile.velocity.X * -0.5f, Projectile.velocity.Y * -0.5f);

            if (shootCount > 0)
                shootCount--;
            if (specialPressTimer > 0)
                specialPressTimer--;
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (Main.mouseLeft && player.whoAmI == Projectile.owner)
            {
                if (target == null)
                {
                    target = FindNearestTarget(TargetDetectionRange);
                }
                else
                {
                    attackFrames = true;
                    Projectile.position = target.Center + new Vector2(((target.width / 2f) + Projectile.width) * -target.direction, 0f);
                    Projectile.direction = target.direction;
                    if (Projectile.Distance(target.Center) >= TargetDetectionRange)
                    {
                        target = null;
                    }

                    if (shootCount <= 0 && Projectile.frame == 2)
                    {
                        shootCount += shootTime;
                        target.StrikeNPC(newProjectileDamage, 0.2f, Projectile.direction);
                        SoundStyle item41 = SoundID.Item41;
                        item41.Pitch = 3f;
                        SoundEngine.PlaySound(item41, Projectile.Center);
                    }
                    target.GetGlobalNPC<FanGlobalNPC>().banksCoinMultiplier = coinMultiplier;
                }
            }
            if (Main.mouseRight && player.whoAmI == Projectile.owner && !attackFrames)
            {
                if (target == null)
                {
                    shotgunChargeTimer = 0;
                    target = FindNearestTarget(TargetDetectionRange);
                }
                else
                {
                    secondaryAbilityFrames = true;
                    Projectile.Center = target.Center + new Vector2(((target.width / 2f) + Projectile.width) * -target.direction, 0f);
                    Projectile.direction = target.direction;
                    if (Projectile.Distance(target.Center) >= TargetDetectionRange)
                    {
                        target = null;
                    }

                    shotgunChargeTimer++;
                    if (shotgunChargeTimer >= 75)
                    {
                        shootCount += newShootTime;
                        Vector2 shootVel = target.Center - Projectile.Center;
                        shootVel.Normalize();
                        shootVel *= shootSpeed;

                        float numberProjectiles = 6;
                        float rotation = MathHelper.ToRadians(30f);
                        float random = Main.rand.NextFloat(-6f, 6f + 1f);
                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            Vector2 perturbedSpeed = new Vector2(shootVel.X + random, shootVel.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ProjectileID.Bullet, newProjectileDamage * 2, 1f, player.whoAmI);
                            Main.projectile[proj].netUpdate = true;
                        }
                        shotgunChargeTimer = 0;
                        SoundEngine.PlaySound(SoundID.Item36, Projectile.position);
                    }
                    target.GetGlobalNPC<FanGlobalNPC>().banksCoinMultiplier = coinMultiplier;
                }
            }
            if (!Main.mouseLeft && !Main.mouseRight)
            {
                idleFrames = true;
                attackFrames = false;
                secondaryAbilityFrames = false;
                target = null;
            }
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
                    fPlayer.banksDefenseReduction = 6;
                    coinMultiplier = 1.5f;
                    player.AddBuff(BuffType<RiskyRewards>(), 2);
                }
                specialPressTimer += 30;
            }

            if (!attackFrames && !secondaryAbilityFrames)
            {
                StayBehind();
            }
            Projectile.spriteDirection = Projectile.direction;
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
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Banks/BanksStand_" + animationName).Value;
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