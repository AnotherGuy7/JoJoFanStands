using Microsoft.Xna.Framework;
using JoJoFanStands;
using Terraria;
using Terraria.ID;
using JoJoStands;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Projectiles.Minions
{  
    public class CoolOutStandT2 : ModProjectile
    {
        public override string Texture
        {
            get { return mod.Name + "/Projectiles/PlayerStands/CoolOutStand"; }
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 18;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 46;
            projectile.height = 64;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.netImportant = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            MyPlayer.stopImmune.Add(mod.ProjectileType(Name));
        }

        protected float shootCool = 12f;
        protected float shootSpeed = 12f;
        public int abilityCooldown = 0;
        public bool normalFrames = false;
        public bool attackFrames = false;
        public bool altAttacking = false;
        public bool slamFrames = false;
        public int proj = 0;
        public int projectilesChosen = 0;
        public int chance = 0;
        public int abilityNumber = 0;
        public bool saidAbility = true;
        public int abilitySwitchTimer = 0;      //switches a bunch when you press it, causing a mess and not picking the right ability in the end

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FanPlayer Fplayer = player.GetModPlayer<FanPlayer>();
            SelectFrame();
            shootCool--;
            abilityCooldown--;

            abilitySwitchTimer--;
            if (abilitySwitchTimer <= 0)
            {
                abilitySwitchTimer = 0;
            }
            if (abilityCooldown <= 0)
            {
                abilityCooldown = 0;
            }
            if (abilityNumber >= 4)
            {
                abilityNumber = 0;
            }
            if (shootCool <= 0f)
            {
                shootCool = 0f;
            }
            if (Fplayer.CoolOutActive)
            {
                projectile.timeLeft++;
            }
            else
            {
                projectile.Kill();
            }
            Vector2 vector131 = player.Center;
            vector131.X -= (float)((12 + player.width / 2) * player.direction);
            vector131.Y -= 25f;
            projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
            projectile.velocity *= 0.8f;
            projectile.direction = (projectile.spriteDirection = player.direction);
            Lighting.AddLight(projectile.position, 1.78f, 2.21f, 2.54f);
            if (Main.mouseLeft)
            {
                normalFrames = false;
                attackFrames = true;
                slamFrames = false;
                if (shootCool <= 0f)
                {
                    Main.PlaySound(SoundID.Item28, projectile.position);
                    shootCool += 30;
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    shootVel.Normalize();
                    shootVel *= shootSpeed;
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, mod.ProjectileType("IceBolt"), 35, 10f, Main.myPlayer);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                normalFrames = true;
                attackFrames = false;
                slamFrames = false;
            }
            if (Main.mouseRight && abilityCooldown <= 0f && abilityNumber == 0)
            {
                altAttacking = true;
            }
            if (altAttacking)
            {
                normalFrames = false;
                attackFrames = true;
                slamFrames = false;
                Main.PlaySound(SoundID.Item28, projectile.position);
                Vector2 shootVel = Main.MouseWorld - projectile.Center;
                if (shootVel == Vector2.Zero)
                {
                    shootVel = new Vector2(0f, 1f);
                }
                shootVel.Normalize();
                shootVel *= shootSpeed;
                if (projectilesChosen <= 2)
                {
                    chance = Main.rand.Next(1, 7);
                    projectilesChosen += 1;
                }
                if (projectilesChosen >= 3)
                {
                    chance = 0;
                    altAttacking = false;
                    abilityCooldown += 60;
                }
                if (chance == 1)
                {
                    proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, ProjectileID.NightBeam, 30, projectile.knockBack, Main.myPlayer);
                }
                if (chance == 2)
                {
                    proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, mod.ProjectileType("IceKnife"), 15, projectile.knockBack, Main.myPlayer);
                }
                if (chance == 3)
                {
                    proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, mod.ProjectileType("IceJavelin"), 25, projectile.knockBack, Main.myPlayer);
                }
                if (chance == 4)
                {
                    proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, mod.ProjectileType("IceGlaive"), 30, projectile.knockBack, Main.myPlayer);
                }
                if (chance == 5)
                {
                    proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, mod.ProjectileType("IceArrow"), 20, projectile.knockBack, Main.myPlayer);
                }
                if (chance == 6)
                {
                    proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, ProjectileID.IceSickle, 40, projectile.knockBack, Main.myPlayer);
                }
                Main.projectile[proj].netUpdate = true;
                projectile.netUpdate = true;
            }
            if (Main.mouseRight && abilityNumber == 1 && abilityCooldown <= 0)
            {
                normalFrames = false;
                attackFrames = false;
                slamFrames = true;
                Main.PlaySound(SoundID.Item28, projectile.position);
                abilityCooldown += 10;        //make it 600, 10 seconds
                int proj = Projectile.NewProjectile(projectile.Center.X + 7f, projectile.Center.Y + 3f, 0f, 0f, mod.ProjectileType("IceSpike"), 25, 2f, Main.myPlayer);
                Main.projectile[proj].netUpdate = true;
                projectile.netUpdate = true;
            }
            if (Main.mouseRight && abilityNumber == 2 && abilityCooldown <= 0)
            {
                abilityCooldown += 300;
                Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("IceGlobe"), 1, 10f, Main.myPlayer);
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    if (Main.npc[k].position.X > player.position.X)
                    {
                        Main.npc[k].velocity.X = 10f;
                    }
                    if (Main.npc[k].position.X <= player.position.X)
                    {
                        Main.npc[k].velocity.X = -10f;
                    }
                    if (Main.npc[k].position.Y > player.position.Y)
                    {
                        Main.npc[k].velocity.Y = 10f;
                    }
                    if (Main.npc[k].position.Y <= player.position.Y)
                    {
                        Main.npc[k].velocity.Y = -10f;
                    }
                }
            }
            if (Main.mouseRight && abilityNumber == 3 && abilityCooldown <= 0)
            {
                Fplayer.avalanche = true;
                abilityCooldown += 600;
            }
            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed && abilitySwitchTimer <= 0)
            {
                abilityNumber++;
                abilitySwitchTimer = 10;
                saidAbility = false;
            }
            if (!saidAbility)
            {
                if (abilityNumber == 0)
                {
                    Main.NewText("Ability: Ice Weapons");
                    saidAbility = true;
                }
                if (abilityNumber == 1)
                {
                    Main.NewText("Ability: Ice Wave");
                    saidAbility = true;
                }
                if (abilityNumber == 2)
                {
                    Main.NewText("Ability: Ice Globe");
                    saidAbility = true;
                }
                if (abilityNumber == 3)
                {
                    Main.NewText("Ability: Avalanche");
                    saidAbility = true;
                }
            }
        }

        public virtual void SelectFrame()
        {
            projectile.frameCounter++;
            if (normalFrames)
            {
                if (projectile.frameCounter >= 12)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 8)
                {
                    projectile.frame = 0;
                }
            }
            if (attackFrames)
            {
                if (projectile.frameCounter >= 7)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 7)
                {
                    projectile.frame = 8;
                }
                if (projectile.frame <= 12)
                {
                    projectile.frame = 8;
                }
            }
            if (slamFrames)
            {
                if (projectile.frameCounter >= 10)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 14)
                {
                    projectile.frame = 15;
                }
                if (projectile.frame <= 18)
                {
                    projectile.frame = 15;
                }
            }
        }
    }
}