using JoJoStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands
{
    public class TheFates : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            Main.projFrames[projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 38;
            projectile.height = 1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.netImportant = true;
            projectile.minionSlots = 1;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public Vector2 velocityAddition = Vector2.Zero;
        public float mouseDistance = 0f;
        protected float shootSpeed = 16f;       //how fast the projectile the minion shoots goes
        public bool normalFrames = false;
        public bool attackFrames = false;
        int shootCount = 0;

        public override void AI()
        {
            SelectFrame();
            shootCount--;
            Player player = Main.player[projectile.owner];
            MyPlayer MPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer FPlayer = player.GetModPlayer<FanPlayer>();
            projectile.frameCounter++;
            if (player.HeldItem.type == mod.ItemType("TheFates") && FPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }
            if (player.HeldItem.type != mod.ItemType("TheFates") || !FPlayer.StandOut || player.dead)
            {
                projectile.active = false;
            }
            if (projectile.spriteDirection == 1)
            {
                drawOffsetX = -10;
            }
            if (projectile.spriteDirection == -1)
            {
                drawOffsetX = -60;
            }
            drawOriginOffsetY = -80;
            if (Main.mouseLeft)
            {
                attackFrames = true;
                normalFrames = false;
                Main.mouseRight = false;        //so that the player can't just stop time while punching
                float rotaY = Main.MouseWorld.Y - projectile.Center.Y;
                projectile.rotation = MathHelper.ToRadians((rotaY * projectile.spriteDirection) / 6f);
                if (Main.MouseWorld.X > projectile.position.X)
                {
                    projectile.spriteDirection = 1;
                    projectile.direction = 1;
                }
                if (Main.MouseWorld.X < projectile.position.X)
                {
                    projectile.spriteDirection = -1;
                    projectile.direction = -1;
                }
                if (projectile.position.X < Main.MouseWorld.X - 5f)
                {
                    velocityAddition.X = 5f;
                }
                if (projectile.position.X > Main.MouseWorld.X + 5f)
                {
                    velocityAddition.X = -5f;
                }
                if (projectile.position.X > Main.MouseWorld.X - 5f && projectile.position.X < Main.MouseWorld.X + 5f)
                {
                    velocityAddition.X = 0f;
                }
                if (projectile.position.Y > Main.MouseWorld.Y + 5f)
                {
                    velocityAddition.Y = -5f;
                }
                if (projectile.position.Y < Main.MouseWorld.Y - 5f)
                {
                    velocityAddition.Y = 5f;
                }
                if (projectile.position.Y < Main.MouseWorld.Y + 5f && projectile.position.Y > Main.MouseWorld.Y - 5f)
                {
                    velocityAddition.Y = 0f;
                }
                mouseDistance = Vector2.Distance(Main.MouseWorld, projectile.Center);
                if (mouseDistance > 20f)
                {
                    projectile.velocity = player.velocity + velocityAddition;
                }
                if (mouseDistance <= 20f)
                {
                    projectile.velocity = Vector2.Zero;
                }
                if (shootCount <= 0)
                {
                    shootCount += 12;
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    shootVel.Normalize();
                    shootVel *= shootSpeed;
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 20f, shootVel.X, shootVel.Y, mod.ProjectileType("Fists"), 7, 2f, Main.myPlayer, 0f, 0f);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                }
                if (shootCount <= 0)
                {
                    shootCount = 0;
                }
            }
            else
            {
                Vector2 vector131 = player.Center;
                vector131.X -= (float)((12 + player.width / 2) * player.direction);
                vector131.Y -= -15f;
                projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
                projectile.velocity *= 0.8f;
                projectile.direction = (projectile.spriteDirection = player.direction);
                projectile.rotation = 0;
                normalFrames = true;
                attackFrames = false;
            }
            if (Main.mouseRight && !player.HasBuff(mod.BuffType("SoreEye")))
            {
                Main.mouseLeft = false;
                player.AddBuff(mod.BuffType("Forewarned"), 180);
            }
            Vector2 direction = player.Center - projectile.Center;
            float distanceTo = direction.Length();
            if (distanceTo > 98f)
            {
                if (projectile.position.X <= player.position.X - 15f)
                {
                    projectile.position = new Vector2(projectile.position.X + 0.2f, projectile.position.Y);
                    projectile.velocity = Vector2.Zero;
                }
                if (projectile.position.X >= player.position.X + 15f)
                {
                    projectile.position = new Vector2(projectile.position.X - 0.2f, projectile.position.Y);
                    projectile.velocity = Vector2.Zero;
                }
                if (projectile.position.Y >= player.position.Y + 15f)
                {
                    projectile.position = new Vector2(projectile.position.X, projectile.position.Y - 0.2f);
                    projectile.velocity = Vector2.Zero;
                }
                if (projectile.position.Y <= player.position.Y - 15f)
                {
                    projectile.position = new Vector2(projectile.position.X, projectile.position.Y + 0.2f);
                    projectile.velocity = Vector2.Zero;
                }
            }
            if (distanceTo >= 120)
            {
                Main.mouseLeft = false;
                Main.mouseRight = false;
                projectile.Center = player.Center;
            }
        }

        public virtual void SelectFrame()
        {
            projectile.frameCounter++;
            if (attackFrames)
            {
                normalFrames = false;
                if (projectile.frameCounter >= 12)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame <= 15)
                {
                    projectile.frame = 16;
                }
                if (projectile.frame >= 20)
                {
                    projectile.frame = 16;
                }
            }
            if (normalFrames)
            {
                if (projectile.frameCounter >= 15)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 16)
                {
                    projectile.frame = 0;
                }
            }
        }
    }
}