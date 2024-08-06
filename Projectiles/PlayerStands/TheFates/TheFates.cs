using JoJoFanStands.Buffs;
using JoJoStands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.TheFates
{
    public class TheFates : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 38;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.netImportant = true;
            Projectile.minionSlots = 1;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        protected float ProjectileSpeed = 16f;       //how fast the Projectile the minion shoots goes
        private bool idleFrames;
        private bool attackFrames;
        private int shootCount = 0;

        public override void AI()
        {
            SelectFrame();
            if (shootCount > 0)
                shootCount--;
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            DrawOriginOffsetY = -80;
            if (Projectile.spriteDirection == 1)
                DrawOffsetX = -10;
            if (Projectile.spriteDirection == -1)
                DrawOffsetX = -60;

            if (Main.mouseLeft)
            {
                idleFrames = false;
                attackFrames = true;
                Main.mouseRight = false;        //so that the player can't just stop time while punching
                float rotaY = Main.MouseWorld.Y - Projectile.Center.Y;
                Projectile.rotation = MathHelper.ToRadians((rotaY * Projectile.spriteDirection) / 6f);
                if (Main.MouseWorld.X > Projectile.position.X)
                {
                    Projectile.spriteDirection = 1;
                    Projectile.direction = 1;
                }
                if (Main.MouseWorld.X < Projectile.position.X)
                {
                    Projectile.spriteDirection = -1;
                    Projectile.direction = -1;
                }
                Vector2 velocityAddition = Vector2.Zero;
                if (Projectile.position.X < Main.MouseWorld.X - 5f)
                    velocityAddition.X = 5f;
                else if (Projectile.position.X > Main.MouseWorld.X + 5f)
                    velocityAddition.X = -5f;
                else if (Projectile.position.X > Main.MouseWorld.X - 5f && Projectile.position.X < Main.MouseWorld.X + 5f)
                    velocityAddition.X = 0f;

                if (Projectile.position.Y > Main.MouseWorld.Y + 5f)
                    velocityAddition.Y = -5f;
                else if (Projectile.position.Y < Main.MouseWorld.Y - 5f)
                    velocityAddition.Y = 5f;
                else if (Projectile.position.Y < Main.MouseWorld.Y + 5f && Projectile.position.Y > Main.MouseWorld.Y - 5f)
                    velocityAddition.Y = 0f;

                float mouseDistance = Vector2.Distance(Main.MouseWorld, Projectile.Center);
                if (mouseDistance > 16f)
                    Projectile.velocity = player.velocity + velocityAddition;
                else
                    Projectile.velocity = Vector2.Zero;

                if (shootCount <= 0)
                {
                    shootCount += 12;
                    Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = new Vector2(0f, 1f);

                    shootVel.Normalize();
                    shootVel *= ProjectileSpeed;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - 20f, shootVel.X, shootVel.Y, ModContent.ProjectileType<FanStandFists>(), 7, 2f, Main.myPlayer, 0f, 0f);
                    Main.projectile[proj].netUpdate = true;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                idleFrames = true;
                attackFrames = false;
                Vector2 vector131 = player.Center;
                vector131.X -= (float)((12 + player.width / 2) * player.direction);
                vector131.Y -= -15f;
                Projectile.Center = Vector2.Lerp(Projectile.Center, vector131, 0.2f);
                Projectile.velocity *= 0.8f;
                Projectile.direction = (Projectile.spriteDirection = player.direction);
                Projectile.rotation = 0;
            }
            if (Main.mouseRight && !player.HasBuff(ModContent.BuffType<SoreEye>()))
            {
                Main.mouseLeft = false;
                player.AddBuff(ModContent.BuffType<Forewarned>(), 180);
            }
            Vector2 direction = player.Center - Projectile.Center;
            float distanceTo = direction.Length();
            if (distanceTo > 98f)
            {
                if (Projectile.position.X <= player.position.X - 15f)
                {
                    Projectile.position = new Vector2(Projectile.position.X + 0.2f, Projectile.position.Y);
                    Projectile.velocity = Vector2.Zero;
                }
                else if (Projectile.position.X >= player.position.X + 15f)
                {
                    Projectile.position = new Vector2(Projectile.position.X - 0.2f, Projectile.position.Y);
                    Projectile.velocity = Vector2.Zero;
                }

                if (Projectile.position.Y >= player.position.Y + 15f)
                {
                    Projectile.position = new Vector2(Projectile.position.X, Projectile.position.Y - 0.2f);
                    Projectile.velocity = Vector2.Zero;
                }
                else if (Projectile.position.Y <= player.position.Y - 15f)
                {
                    Projectile.position = new Vector2(Projectile.position.X, Projectile.position.Y + 0.2f);
                    Projectile.velocity = Vector2.Zero;
                }
            }
            if (distanceTo >= 120)
            {
                Main.mouseLeft = false;
                Main.mouseRight = false;
                Projectile.Center = player.Center;
            }
        }

        public virtual void SelectFrame()
        {
            Projectile.frameCounter++;
            if (attackFrames)
            {
                idleFrames = false;
                if (Projectile.frameCounter >= 12)
                {
                    Projectile.frame += 1;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame <= 15)
                    Projectile.frame = 16;
                if (Projectile.frame >= 20)
                    Projectile.frame = 16;
            }
            if (idleFrames)
            {
                if (Projectile.frameCounter >= 15)
                {
                    Projectile.frame += 1;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 16)
                    Projectile.frame = 0;
            }
        }
    }
}