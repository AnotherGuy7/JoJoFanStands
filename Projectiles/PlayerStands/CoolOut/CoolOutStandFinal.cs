using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using JoJoStands.Buffs.Debuffs;

namespace JoJoFanStands.Projectiles.PlayerStands.CoolOut
{  
    public class CoolOutStandFinal : StandClass
    {
        public override int ProjectileDamage => 51;
        public override int ShootTime => 20;
        public override int AltDamage => 63;
        public override StandAttackType StandType => StandAttackType.Ranged;
        public override Vector2 StandOffset => new Vector2(-24, 0);
        public override int HalfStandHeight => 32;
        public override float MaxDistance => 0f;

        private int abilityNumber = 0;
        private int specialDamage = 71;
        private int spearWhoAmI = -1;
        private bool letGoOfSpear = false;
        private int slamCounter = 0;
        private readonly Vector2 ShootOffset = new Vector2(0f, -8f);

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            SelectAnimation();
            UpdateStandInfo();
            Lighting.AddLight(Projectile.position, 1.78f, 2.21f, 2.54f);
            if (shootCount > 0)
                shootCount--;
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (spearWhoAmI != -1)
            {
                Projectile spear = Main.projectile[spearWhoAmI];
                if (!spear.active)
                {
                    Projectile.ai[0] = 0f;
                    spearWhoAmI = -1;
                    letGoOfSpear = false;
                }
            }
            if (slamCounter > 0)
            {
                slamCounter--;
                secondaryAbility = true;
                GoInFront();
            }
            else
            {
                secondaryAbility = false;
                StayBehind();
            }

            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.mouseLeft)
                {
                    attacking = true;
                    currentAnimationState = AnimationState.Attack;
                    if (shootCount <= 0f)
                    {
                        Vector2 shootPosition = Projectile.Center + new Vector2(ShootOffset.X * Projectile.direction, ShootOffset.Y);
                        shootCount += newShootTime;
                        Vector2 shootVel = Main.MouseWorld - shootPosition;
                        if (shootVel == Vector2.Zero)
                            shootVel = new Vector2(0f, 1f);

                        shootVel.Normalize();
                        shootVel *= ProjectileSpeed;
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition, shootVel, ModContent.ProjectileType<IceBolt>(), newProjectileDamage, 8f, Main.myPlayer);
                        Main.projectile[proj].netUpdate = true;
                        Projectile.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                    }
                }
                else
                {
                    attacking = false;
                    currentAnimationState = AnimationState.Idle;
                }

                if (!playerHasAbilityCooldown)
                {
                    if (abilityNumber == 0)     //Ice wave 
                    {
                        if (Main.mouseRight && shootCount <= 0f)
                        {
                            secondaryAbility = true;
                            SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                            slamCounter += 80;
                            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y + 16f, 0f, 0f, ModContent.ProjectileType<IceSpike>(), (int)(specialDamage * mPlayer.standDamageBoosts), 2f, Main.myPlayer, Projectile.whoAmI, Projectile.direction);
                            Main.projectile[proj].netUpdate = true;
                            Projectile.netUpdate = true;
                            player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(10));
                        }
                    }
                    else if (abilityNumber == 1)     //Ice spear
                    {
                        if (Main.mouseRight && shootCount <= 0f && player.ownedProjectileCounts[ModContent.ProjectileType<IceSpear>()] == 0 && spearWhoAmI == -1)
                        {
                            Projectile.ai[0] = 0.5f;
                            spearWhoAmI = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - 10f, 0f, 0f, ModContent.ProjectileType<IceSpear>(), (int)(AltDamage * mPlayer.standDamageBoosts), 10f, Main.myPlayer, Projectile.whoAmI);
                            Main.projectile[spearWhoAmI].netUpdate = true;
                            Projectile.netUpdate = true;
                        }
                        if (Main.mouseRight && spearWhoAmI != -1 && !letGoOfSpear)
                        {
                            Projectile spear = Main.projectile[spearWhoAmI];
                            Projectile.ai[0] += 0.005f;     //used to change multiple things, that's why we're using this
                            if (Projectile.ai[0] >= 2f)
                            {
                                player.AddBuff(BuffID.Chilled, 2);
                            }
                            Vector2 direction = Main.MouseWorld - Projectile.Center;
                            if (Projectile.ai[0] <= 1.3f)
                            {
                                spear.scale = Projectile.ai[0];
                            }
                            direction.Normalize();
                            spear.rotation = direction.ToRotation() + 1f;
                            spear.velocity = Vector2.Zero;
                            spear.position = Projectile.Center + new Vector2(0f, -10f);
                        }
                        if (!Main.mouseRight && spearWhoAmI != -1 && !letGoOfSpear)
                        {
                            Projectile spear = Main.projectile[spearWhoAmI];
                            spear.ai[0] = 1f;
                            spear.damage = (int)(AltDamage * (Projectile.ai[0] + 1));
                            Vector2 direction = Main.MouseWorld - Projectile.Center;
                            direction.Normalize();
                            spear.velocity = (direction * 12f) * Projectile.ai[0];
                            player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(5));
                            letGoOfSpear = true;
                        }
                    }
                    else if (Main.mouseRight && abilityNumber == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<SnowGlobe>()] == 0)     //Snow Globe
                    {
                        int globe = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<SnowGlobe>(), 0, 0, Main.myPlayer, Projectile.whoAmI, 500);
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(30));
                        Main.projectile[globe].netUpdate = true;
                    }
                    else if (Main.mouseRight && abilityNumber == 3)     //Hail Storm
                    {
                        float spreadRand = Main.rand.Next(-Main.screenWidth / 2, Main.screenWidth / 2);
                        spearWhoAmI = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center.X + spreadRand, player.Center.Y - (Main.screenHeight / 2f), 0f, 14f, ModContent.ProjectileType<IceBolt>(), newProjectileDamage + 14, 10f, Main.myPlayer);
                        player.AddBuff(BuffID.Chilled, 2);
                    }
                }
            }

            if (SpecialKeyPressed())
            {
                abilityNumber++;
                if (abilityNumber == 0)
                    Main.NewText("Ability: Ice Wave");
                else if (abilityNumber == 1)
                    Main.NewText("Ability: Ice Spear");
                else if (abilityNumber == 2)
                    Main.NewText("Ability: Snow Globe");
                else if (abilityNumber == 3)
                    Main.NewText("Ability: Hail Storm");
                else if (abilityNumber >= 4)
                    Main.NewText("Ability: Ice Wave");
                    abilityNumber = 0;
            }
            if (secondaryAbility)
                currentAnimationState = AnimationState.SecondaryAbility;
            if (mPlayer.posing)
                currentAnimationState = AnimationState.Pose;
        }

        public override void SelectAnimation()
        {
            if (oldAnimationState != currentAnimationState)
            {
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
                oldAnimationState = currentAnimationState;
                Projectile.netUpdate = true;
            }

            if (currentAnimationState == AnimationState.Idle)
                PlayAnimation("Idle");
            else if (currentAnimationState == AnimationState.Attack)
                PlayAnimation("Attack");
            else if (currentAnimationState == AnimationState.SecondaryAbility)
                PlayAnimation("Slam");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Idk");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/CoolOut/CoolOut_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 8, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, 14, true);
            else if (animationName == "Slam")
                AnimateStand(animationName, 1, 180, false);
            else if (animationName == "Idk")
                AnimateStand(animationName, 2, 60, true);
        }
    }
}