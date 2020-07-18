using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands
{  
    public class CoolOutStandFinal : StandClass
    {
        public override int projectileDamage => 51;
        public override int shootTime => 20;
        public override int altDamage => 63;
        public override int standType => 2;
        public override int standOffset => 20;
        public override int halfStandHeight => 32;
        public override float maxDistance => 0f;

        private int abilityNumber = 0;
        private int specialDamage = 71;
        private int spearWhoAmI = -1;
        private bool letGoOfSpear = false;
        private int slamCounter = 0;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            SelectAnimation();
            UpdateStandInfo();
            Lighting.AddLight(projectile.position, 1.78f, 2.21f, 2.54f);
            if (shootCount > 0)
            {
                shootCount--;
            }
            if (mPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }
            if (spearWhoAmI != -1)
            {
                Projectile spear = Main.projectile[spearWhoAmI];
                if (!spear.active)
                {
                    projectile.ai[0] = 0f;
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
                    Main.PlaySound(SoundID.Item28, projectile.position);
                    shootCount += newShootTime;
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    shootVel.Normalize();
                    shootVel *= shootSpeed;
                    int proj = Projectile.NewProjectile(projectile.Center, shootVel, mod.ProjectileType("IceBolt"), newProjectileDamage, 8f, Main.myPlayer);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                normalFrames = true;
            }
            if (!player.HasBuff(JoJoFanStands.JoJoStandsMod.BuffType("AbilityCooldown")) && projectile.owner == Main.myPlayer)
            {
                if (abilityNumber == 0)     //Ice wave 
                {
                    if (Main.mouseRight && shootCount <= 0f)
                    {
                        secondaryAbilityFrames = true;
                        Main.PlaySound(SoundID.Item28, projectile.position);
                        slamCounter += 80;
                        int proj = Projectile.NewProjectile(projectile.Center.X + 7f, projectile.Center.Y + 3f, 0f, 0f, mod.ProjectileType("IceSpike"), specialDamage, 2f, Main.myPlayer, projectile.whoAmI, projectile.direction);
                        Main.projectile[proj].netUpdate = true;
                        projectile.netUpdate = true;
                        player.AddBuff(JoJoFanStands.JoJoStandsMod.BuffType("AbilityCooldown"), mPlayer.AbilityCooldownTime(10));
                    }
                }
                if (abilityNumber == 1)     //Ice spear
                {
                    if (Main.mouseRight && shootCount <= 0f && player.ownedProjectileCounts[mod.ProjectileType("IceSpear")] == 0 && spearWhoAmI == -1)
                    {
                        projectile.ai[0] = 0.5f;
                        spearWhoAmI = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 10f, 0f, 0f, mod.ProjectileType("IceSpear"), (int)(altDamage * mPlayer.standDamageBoosts), 10f, Main.myPlayer, projectile.whoAmI);
                        Main.projectile[spearWhoAmI].netUpdate = true;
                        projectile.netUpdate = true;
                    }
                    if (Main.mouseRight && spearWhoAmI != -1 && !letGoOfSpear)
                    {
                        Projectile spear = Main.projectile[spearWhoAmI];
                        projectile.ai[0] += 0.005f;     //used to change multiple things, that's why we're using this
                        if (projectile.ai[0] >= 2f)
                        {
                            player.AddBuff(BuffID.Chilled, 2);
                        }
                        Vector2 direction = Main.MouseWorld - projectile.Center;
                        if (projectile.ai[0] <= 1.3f)
                        {
                            spear.scale = projectile.ai[0];
                        }
                        direction.Normalize();
                        spear.rotation = direction.ToRotation() + 1f;
                        spear.velocity = Vector2.Zero;
                        spear.position = projectile.Center + new Vector2(0f, -10f);
                    }
                    if (!Main.mouseRight && spearWhoAmI != -1 && !letGoOfSpear)
                    {
                        Projectile spear = Main.projectile[spearWhoAmI];
                        spear.ai[0] = 1f;
                        spear.damage = (int)(altDamage * (projectile.ai[0] + 1));
                        Vector2 direction = Main.MouseWorld - projectile.Center;
                        direction.Normalize();
                        spear.velocity = (direction * 12f) * projectile.ai[0];
                        player.AddBuff(JoJoFanStands.JoJoStandsMod.BuffType("AbilityCooldown"), mPlayer.AbilityCooldownTime(5));
                        letGoOfSpear = true;
                    }
                }
                if (Main.mouseRight && abilityNumber == 2 && player.ownedProjectileCounts[mod.ProjectileType("SnowGlobe")] == 0)     //Snow Globe
                {
                    int globe = Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("SnowGlobe"), 0, 0, Main.myPlayer, projectile.whoAmI, 500);
                    player.AddBuff(JoJoFanStands.JoJoStandsMod.BuffType("AbilityCooldown"), mPlayer.AbilityCooldownTime(30));
                    Main.projectile[globe].netUpdate = true;
                }
                if (Main.mouseRight && abilityNumber == 3)     //Hail Storm
                {
                    float spreadRand = Main.rand.Next(-Main.screenWidth / 2, Main.screenWidth / 2);
                    spearWhoAmI = Projectile.NewProjectile(player.Center.X + spreadRand, player.Center.Y - (Main.screenHeight / 2f), 0f, 14f, mod.ProjectileType("IceBolt"), newProjectileDamage + 14, 10f, Main.myPlayer);
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
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Slam");
                projectile.frame = 0;
            }
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
            if (Main.player[projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Idk");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = mod.GetTexture("Projectiles/PlayerStands/CoolOut/CoolOut_" + animationName);
            if (animationName == "Idle")
            {
                AnimationStates(animationName, 8, 15, true);
            }
            if (animationName == "Attack")
            {
                AnimationStates(animationName, 4, 14, true);
            }
            if (animationName == "Slam")
            {
                AnimationStates(animationName, 1, 180, true);
            }
            if (animationName == "Idk")
            {
                AnimationStates(animationName, 2, 60, true);
            }
        }
    }
}