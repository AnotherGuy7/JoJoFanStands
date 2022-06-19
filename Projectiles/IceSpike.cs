using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class IceSpike : ModProjectile
    {
        private float Yaddition = 0;
        private bool spawnedAnother = false;
        private bool canSpawn = false;
        private int moveDistanceX = 16;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.5f;
            DrawOriginOffsetY = 8;
        }

        public override void AI()
        {
            Projectile ownerProj = Main.projectile[(int)Projectile.ai[0]];

            Projectile.direction = (int)Projectile.ai[1];
            Projectile.velocity.Y += 5f;
            Projectile.scale -= 0.025f;     //0.025f obtained by doing (max scale - max time left)
            DrawOriginOffsetY = 8 + (int)((1.5f - Projectile.scale) * 20f);
            if (!ownerProj.active || Projectile.scale <= 0f)
            {
                Projectile.Kill();
            }

            /*if (Collision.SolidTiles((int)Projectile.position.X / 16, (int)(Projectile.position.X + moveDistanceX) / 16, (int)Projectile.position.Y / 16, (int)(Projectile.position.Y - Yaddition) / 16))
            {
                spawnedAnother = true;
                Yaddition += 1;
            }*/
            if (Main.tile[(int)(Projectile.position.X + (moveDistanceX * Projectile.direction)) / 16, (int)(Projectile.position.Y - Yaddition) / 16].TileType == 0 && Main.tile[(int)(Projectile.position.X + (moveDistanceX * Projectile.direction)) / 16, (int)(Projectile.position.Y - Yaddition) / 16].TileType != TileID.Trees)
            {
                canSpawn = true;
            }
            else
            {
                Yaddition += 5;
            }
            if (canSpawn && !spawnedAnother && Projectile.timeLeft <= 55)        //&& Projectile.timeLeft <= 5
            {
                /*if (FPlayer.additionMode)
                {
                    FPlayer.numberOfSpikesCreated += 0.20f;
                }
                if (!FPlayer.additionMode)
                {
                    FPlayer.numberOfSpikesCreated -= 0.20f;
                }*/
                spawnedAnother = true;
                canSpawn = false;
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + (5f * Projectile.direction), Projectile.Center.Y - Yaddition, 0f, 0f, Mod.Find<ModProjectile>("IceSpike").Type, Projectile.damage, 2f, Main.myPlayer, ownerProj.whoAmI, Projectile.direction);
                Main.projectile[proj].direction = Projectile.direction;
                Main.projectile[proj].netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}