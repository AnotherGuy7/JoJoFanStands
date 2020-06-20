using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
 
namespace JoJoFanStands.Projectiles.PlayerStands
{
    public class BackInBlackStand : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 10;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.netImportant = true;
            projectile.minionSlots = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = 2;
            projectile.timeLeft = 0;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            MyPlayer.stopImmune.Add(mod.ProjectileType(Name));
        }

        public Vector2 velocityAddition = Vector2.Zero;
        public float mouseDistance = 0f;
        protected float shootSpeed = 16f;       //how fast the projectile the minion shoots goes
        public bool normalFrames = false;
        public bool attackFrames = false;
        public bool HoldingBlackHole = false;
        public int shootCount = 0;
        public int blackHoleProjectile = 0;
        public override void AI()
        {
            SelectFrame();
            shootCount--;
            Player player = Main.player[projectile.owner];
            Vector2 vector131 = player.Center;
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            Lighting.AddLight((int)(projectile.Center.X / 16f), (int)(projectile.Center.Y / 16f), 0.6f, 0.9f, 0.3f);
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 35, projectile.velocity.X * -0.5f, projectile.velocity.Y * -0.5f);
            if (!HoldingBlackHole)
            {
                vector131.X -= (float)((15 + player.width / 2) * player.direction);
                vector131.Y -= 15f;
                projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
                projectile.velocity *= 0.8f;
                projectile.direction = (projectile.spriteDirection = player.direction);
            }
            else
            {
                projectile.velocity = Vector2.Zero;
                projectile.Center = Main.projectile[blackHoleProjectile].Center + new Vector2(0f, -50f);
            }
            if (blackHoleProjectile == 0 && player.ownedProjectileCounts[mod.ProjectileType("BlackHole")] != 0)
            {
                blackHoleProjectile = BlackHole.whoAmI;
            }
            if (player.HeldItem.type == mod.ItemType("BackInBlack"))
            {
                projectile.timeLeft = 2;
            }
            if (player.HeldItem.type != mod.ItemType("BackInBlack"))
            {
                projectile.active = false;
            }
            if (player.dead)
            {
                projectile.active = false;
            }
            if (Main.mouseLeft && !HoldingBlackHole)
            {
                attackFrames = true;
                normalFrames = false;
                Main.mouseRight = false;
                if (shootCount <= 0)
                {
                    Main.PlaySound(SoundID.Item78, projectile.position);
                    shootCount += 40;
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    shootVel.Normalize();
                    shootVel *= 1.5f;
                    Vector2 perturbedSpeed = new Vector2(shootVel.X, shootVel.Y);
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("BackInBlackOrb"), 62, 2f, player.whoAmI);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                }
            }
            if (!Main.mouseLeft && !HoldingBlackHole)
            {
                normalFrames = true;
                attackFrames = false;
            }
            if (JoJoStands.JoJoStands.SpecialHotKey.Current)
            {
                Main.mouseLeft = false;
                Main.mouseRight = false;
                if (player.ownedProjectileCounts[mod.ProjectileType("BlackHole")] <= 0)
                {
                    HoldingBlackHole = true;
                    blackHoleProjectile = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y + 50f, 0f, 0f, mod.ProjectileType("BlackHole"), 150, 2f, player.whoAmI);
                    Main.projectile[blackHoleProjectile].scale = 0.05f;
                    Main.projectile[blackHoleProjectile].netUpdate = true;
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("BlackHole")] != 0)
                {
                    Main.projectile[blackHoleProjectile].scale += 0.003f;
                    Main.projectile[blackHoleProjectile].timeLeft += 2;
                }
            }
            if (!JoJoStands.JoJoStands.SpecialHotKey.Current && HoldingBlackHole)
            {
                Main.projectile[blackHoleProjectile].scale -= 0.005f;
            }
            if (player.ownedProjectileCounts[mod.ProjectileType("BlackHole")] == 0 && HoldingBlackHole)
            {
                HoldingBlackHole = false;
            }
            if (shootCount <= 0)
            {
                shootCount = 0;
            }
        }

        public void SelectFrame()
        {
            projectile.frameCounter++;
            if (normalFrames)
            {
                if (projectile.frameCounter >= 20)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 2)
                {
                    projectile.frame = 0;
                }
            }
            if (attackFrames)
            {
                if (projectile.frameCounter >= 12)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 8)
                {
                    projectile.frame = 2;
                }
                if (projectile.frame <= 1)
                {
                    projectile.frame = 2;
                }
            }
            if (HoldingBlackHole)
            {
                if (projectile.frameCounter >= 10)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 10)
                {
                    projectile.frame = 8;
                }
                if (projectile.frame <= 7)
                {
                    projectile.frame = 8;
                }
            }
        }
    }
}