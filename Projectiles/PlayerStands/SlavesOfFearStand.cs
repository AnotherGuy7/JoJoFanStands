using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;

namespace JoJoFanStands.Projectiles.PlayerStands
{
    public class SlavesOfFearStand : ModProjectile      //has 2 poses
    {
        public override string Texture
        {
            get { return mod.Name + "/Projectiles/PlayerStands/SlavesOfFearStand"; }
        }

        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            Main.projFrames[projectile.type] = 14;
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
            MyPlayer.stopImmune.Add(mod.ProjectileType(Name));
        }

        public Vector2 velocityAddition = Vector2.Zero;
        public float mouseDistance = 0f;
        protected float shootSpeed = 16f;
        public bool normalFrames = false;
        public bool attackFrames = false;
        public float maxDistance = 0f;
        public int punchDamage = 23;
        public int altDamage = 96;
        int shootCount = 0;
        public int halfStandHeight = 37;
        Mod JoJoStands = null;

        public override void AI()
        {
            SelectFrame();
            shootCount--;
            Player player = Main.player[projectile.owner];
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            projectile.frameCounter++;
            fPlayer.StandOut = true;
            if (player.HeldItem.type == mod.ItemType("SlavesOfFearT1") && fPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }
            if (player.HeldItem.type != mod.ItemType("SlavesOfFearT1") || !fPlayer.StandOut || player.dead)
            {
                projectile.active = false;
            }
            if (JoJoStands == null)
            {
                JoJoStands = ModLoader.GetMod("JoJoStands");
            }
            if (projectile.spriteDirection == 1)
            {
                drawOffsetX = -10;
            }
            if (projectile.spriteDirection == -1)
            {
                drawOffsetX = -60;
            }
            drawOriginOffsetY = -halfStandHeight;
            if (Main.mouseLeft)
            {
                attackFrames = true;
                normalFrames = false;
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
                if (mouseDistance > 40f)
                {
                    projectile.velocity = player.velocity + velocityAddition;
                }
                if (mouseDistance <= 40f)
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
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, JoJoStands.ProjectileType("Fists"), (int)(punchDamage * modPlayer.standDamageBoosts), 2f, Main.myPlayer, 1f);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                Vector2 vector131 = player.Center;
                vector131.X -= (float)((12 + player.width / 2) * player.direction);
                vector131.Y -= -35f + halfStandHeight;
                projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
                projectile.velocity *= 0.8f;
                projectile.direction = (projectile.spriteDirection = player.direction);
                projectile.rotation = 0;
                normalFrames = true;
                attackFrames = false;
            }
            Vector2 direction = player.Center - projectile.Center;
            float distanceTo = direction.Length();
            maxDistance = 98f + modPlayer.standRangeBoosts;
            if (distanceTo > maxDistance)
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
            if (distanceTo >= maxDistance + 22f)
            {
                Main.mouseLeft = false;
                Main.mouseRight = false;
                projectile.Center = player.Center;
            }
			if (shootCount <= 0)
			{
				shootCount = 0;
			}
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            if (MyPlayer.RangeIndicators)
            {
                Texture2D texture = JoJoStands.GetTexture("Extras/RangeIndicator");        //the initial tile amount the indicator covers is 20 tiles, 320 pixels, border is included in the measurements
                spriteBatch.Draw(texture, player.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), maxDistance / 122.5f, SpriteEffects.None, 0);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
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
                if (projectile.frame <= 1)
                {
                    projectile.frame = 2;
                }
                if (projectile.frame >= 6)
                {
                    projectile.frame = 2;
                }
            }
            if (normalFrames)
            {
                if (projectile.frameCounter >= 30)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 2)
                {
                    projectile.frame = 0;
                }
            }
        }
    }
}