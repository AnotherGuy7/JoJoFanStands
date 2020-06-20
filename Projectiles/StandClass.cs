using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using Microsoft.Xna.Framework.Graphics;

namespace JoJoFanStands.Projectiles
{
    public abstract class StandClass : ModProjectile        //for use with those normal stands
    {
        public Vector2 velocityAddition = Vector2.Zero;     //normals
        public float mouseDistance = 0f;
        protected float shootSpeed = 16f;
        public int shootCount = 0;

        public virtual float maxDistance { get; } = 0f;      //ones you set
        public virtual int punchDamage { get; } = 0;
        public virtual int punchTime { get; } = 0;
        public virtual int altDamage { get; } = 0;
        public virtual int halfStandHeight { get; } = 0;
        public virtual bool useNormalDistance { get; } = true;

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 38;
            projectile.height = 1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.netImportant = true;
            projectile.minionSlots = 1;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            MyPlayer.stopImmune.Add(mod.ProjectileType(Name));
        }

        public void Punch()
        {
            Player player = Main.player[projectile.owner];
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            float maxDist = maxDistance + modPlayer.standRangeBoosts;

            float rotaY = Main.MouseWorld.Y - projectile.Center.Y;
            projectile.rotation = MathHelper.ToRadians((rotaY * projectile.spriteDirection) / 6f);

            if (shootCount <= 0)
            {
                shootCount = 0;
            }

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
                shootCount += punchTime;
                Vector2 shootVel = Main.MouseWorld - projectile.Center;
                if (shootVel == Vector2.Zero)
                {
                    shootVel = new Vector2(0f, 1f);
                }
                shootVel.Normalize();
                shootVel *= shootSpeed;
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, JoJoFanStands.JoJoStandsMod.ProjectileType("Fists"), (int)(punchDamage * modPlayer.standDamageBoosts), 2f, Main.myPlayer);
                Main.projectile[proj].netUpdate = true;
                projectile.netUpdate = true;
            }

            if (useNormalDistance)
            {
                Vector2 direction = player.Center - projectile.Center;
                float distanceTo = direction.Length();
                if (distanceTo > maxDist)
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
                if (distanceTo >= maxDist + 22f)
                {
                    Main.mouseLeft = false;
                    Main.mouseRight = false;
                    projectile.Center = player.Center;
                }
            }
        }

        public void StayBehind()
        {
            Player player = Main.player[projectile.owner];

            Vector2 vector131 = player.Center;
            vector131.X -= (float)((12 + player.width / 2) * player.direction);
            vector131.Y -= -35f + halfStandHeight;
            projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
            projectile.velocity *= 0.8f;
            projectile.direction = (projectile.spriteDirection = player.direction);
            projectile.rotation = 0;

            drawOriginOffsetY = -halfStandHeight;
            if (projectile.spriteDirection == 1)
            {
                drawOffsetX = -10;
            }
            if (projectile.spriteDirection == -1)
            {
                drawOffsetX = -60;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            if (MyPlayer.RangeIndicators)
            {
                Texture2D texture = JoJoFanStands.JoJoStandsMod.GetTexture("Extras/RangeIndicator");        //the initial tile amount the indicator covers is 20 tiles, 320 pixels, border is included in the measurements
                spriteBatch.Draw(texture, player.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White * (((float)MyPlayer.RangeIndicatorAlpha * 3.9215f) / 1000f), 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), maxDistance / 122.5f, SpriteEffects.None, 0);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}