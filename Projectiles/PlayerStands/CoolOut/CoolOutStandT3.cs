using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.CoolOut
{
    public class CoolOutStandT3 : StandClass
    {
        public override int projectileDamage => 39;
        public override int shootTime => 30;
        public override int altDamage => 44;
        public override StandType standType => StandType.Ranged;
        public override int standOffset => 20;
        public override int halfStandHeight => 32;
        public override float maxDistance => 0f;

        private int abilityNumber = 0;
        private int specialDamage = 51;
        private int spearWhoAmI = -1;
        private bool letGoOfSpear = false;
        private int slamCounter = 0;

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
                secondaryAbilityFrames = true;
                GoInFront();
            }
            else
            {
                secondaryAbilityFrames = false;
                StayBehind();
            }

            if (Main.mouseLeft)
            {
                attackFrames = true;
                if (shootCount <= 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                    shootCount += newShootTime;
                    Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    shootVel.Normalize();
                    shootVel *= shootSpeed;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<IceBolt>(), newProjectileDamage, 8f, Main.myPlayer);
                    Main.projectile[proj].netUpdate = true;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                idleFrames = true;
            }
            if (!player.HasBuff(JoJoFanStands.JoJoStandsMod.Find<ModBuff>("AbilityCooldown").Type) && Projectile.owner == Main.myPlayer)
            {
                if (abilityNumber == 0)     //Ice wave 
                {
                    if (Main.mouseRight && shootCount <= 0f)
                    {
                        secondaryAbilityFrames = true;
                        SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                        slamCounter += 80;
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + 7f, Projectile.Center.Y + 3f, 0f, 0f, ModContent.ProjectileType<IceSpike>(), specialDamage, 2f, Main.myPlayer, Projectile.whoAmI, Projectile.direction);
                        Main.projectile[proj].netUpdate = true;
                        Projectile.netUpdate = true;
                        player.AddBuff(JoJoFanStands.JoJoStandsMod.Find<ModBuff>("AbilityCooldown").Type, mPlayer.AbilityCooldownTime(15));
                    }
                }
                if (abilityNumber == 1)     //Ice spear
                {
                    if (Main.mouseRight && shootCount <= 0f && player.ownedProjectileCounts[ModContent.ProjectileType<IceSpear>()] == 0 && spearWhoAmI == -1)
                    {
                        Projectile.ai[0] = 0.5f;
                        spearWhoAmI = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - 10f, 0f, 0f, ModContent.ProjectileType<IceSpear>(), (int)(altDamage * mPlayer.standDamageBoosts), 10f, Main.myPlayer, Projectile.whoAmI);
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
                        spear.damage = (int)(altDamage * (Projectile.ai[0] + 1));
                        Vector2 direction = Main.MouseWorld - Projectile.Center;
                        direction.Normalize();
                        spear.velocity = (direction * 12f) * Projectile.ai[0];
                        player.AddBuff(JoJoFanStands.JoJoStandsMod.Find<ModBuff>("AbilityCooldown").Type, mPlayer.AbilityCooldownTime(10));
                        letGoOfSpear = true;
                    }
                }
                if (Main.mouseRight && abilityNumber == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<SnowGlobe>()] == 0)     //Snow Globe
                {
                    int globe = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<SnowGlobe>(), 0, 0, Main.myPlayer, Projectile.whoAmI, 200);
                    player.AddBuff(JoJoFanStands.JoJoStandsMod.Find<ModBuff>("AbilityCooldown").Type, mPlayer.AbilityCooldownTime(60));
                    Main.projectile[globe].netUpdate = true;
                }
                if (Main.mouseRight && abilityNumber == 3)     //Hail Storm
                {
                    float spreadRand = Main.rand.Next(-Main.screenWidth / 2, Main.screenWidth / 2);
                    spearWhoAmI = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center.X + spreadRand, player.Center.Y - (Main.screenHeight / 2f), 0f, 14f, ModContent.ProjectileType<IceBolt>(), newProjectileDamage + 14, 10f, Main.myPlayer);
                    player.AddBuff(BuffID.Chilled, 2);
                }
            }

            if (SpecialKeyPressed())
            {
                abilityNumber++;
                if (abilityNumber == 0)
                {
                    Main.NewText("Ability: Ice Wave");
                }
                if (abilityNumber == 1)
                {
                    Main.NewText("Ability: Ice Spear");
                }
                if (abilityNumber == 2)
                {
                    Main.NewText("Ability: Snow Globe");
                }
                if (abilityNumber == 3)
                {
                    Main.NewText("Ability: Hail Storm");
                }
                if (abilityNumber >= 4)
                {
                    Main.NewText("Ability: Ice Wave");
                    abilityNumber = 0;
                }
            }
        }

        public override void SelectAnimation()
        {
            if (secondaryAbilityFrames)     //so this takes effect above all else
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Slam");
                Projectile.frame = 0;
            }
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
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Idk");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/CoolOut/CoolOut_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 8, 15, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 4, 14, true);
            }
            if (animationName == "Slam")
            {
                AnimateStand(animationName, 1, 180, true);
            }
            if (animationName == "Idk")
            {
                AnimateStand(animationName, 2, 60, true);
            }
        }
    }
}