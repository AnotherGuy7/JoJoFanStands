using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using JoJoStands;
using Terraria.ModLoader;
using System;

namespace JoJoFanStands.Projectiles.Minions
{
    public class BackInBlackMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 8;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 66;
            projectile.aiStyle = 0;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.friendly = true;
            MyPlayer.stopImmune.Add(mod.ProjectileType(Name));
        }

        protected float shootSpeed = 16f;
        public bool normalFrames = false;
        public bool attackFrames = false;
        public bool portalFrames = false;
        public int shootCount = 0;
        public float maxDistance = 98f;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FanPlayer modPlayer = player.GetModPlayer<FanPlayer>();
            shootCount--;
            if (shootCount <= 0)
            {
                shootCount = 0;
            }
            if (!Main.mouseLeft && !Main.mouseRight)
            {
                projectile.frameCounter++;
                if (player.dead)
                {
                    modPlayer.InTheShadowsPet = false;
                }
                if (modPlayer.InTheShadowsPet)
                {
                    projectile.timeLeft = 2;
                }
                if (Main.dayTime)
                {
                    projectile.alpha = 129;
                }
                Vector2 vector131 = player.Center;
                vector131.X -= (float)((12 + player.width / 2) * player.direction);
                vector131.Y -= 25f;
                projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
                projectile.velocity *= 0.8f;
                projectile.direction = (projectile.spriteDirection = player.direction);
            }
            if (Main.mouseLeft)
            {
                if (projectile.position.X <= Main.MouseWorld.X)
                {
                    projectile.velocity.X = 5f;
                }
                if (projectile.position.X >= Main.MouseWorld.X)
                {
                    projectile.velocity.X = -5f;
                }
                if (projectile.position.Y >= Main.MouseWorld.Y)
                {
                    projectile.velocity.Y = -5f;
                }
                if (projectile.position.Y <= Main.MouseWorld.Y)
                {
                    projectile.velocity.Y = 5f;
                }
                if (projectile.position.X == (Main.MouseWorld.X / 16f))
                {
                    projectile.velocity.X = 0f;
                }
                if (projectile.position.Y == (Main.MouseWorld.Y / 16f))
                {
                    projectile.velocity.Y = 0f;
                }
                attackFrames = true;
                if (shootCount <= 0)
                {
                    shootCount += 10;
                    Vector2 targetPos = Main.MouseWorld;
                    Vector2 shootVel = targetPos - projectile.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    shootVel.Normalize();
                    if (projectile.direction == 1)
                    {
                        shootVel *= shootSpeed;
                    }
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, mod.ProjectileType("InTheShadowFist"), 62, 2f, Main.myPlayer, 0f, 0f);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                }
            }
            if (Main.mouseRight)        //opens a void and if anything walks in there it just disappears then drops loot, a void that only consumes living things
            {
                portalFrames = true;

            }
        }
    }
}