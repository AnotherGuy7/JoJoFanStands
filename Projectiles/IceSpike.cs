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
            projectile.width = 14;
            projectile.height = 20;
            projectile.aiStyle = 0;
            projectile.timeLeft = 60;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.scale = 1.5f;
            drawOriginOffsetY = 8;
        }

        public override void AI()
        {
            Projectile ownerProj = Main.projectile[(int)projectile.ai[0]];

            projectile.direction = (int)projectile.ai[1];
            projectile.velocity.Y += 5f;
            projectile.scale -= 0.025f;     //0.025f obtained by doing (max scale - max time left)
            drawOriginOffsetY = 8 + (int)((1.5f - projectile.scale) * 20f);
            if (!ownerProj.active || projectile.scale <= 0f)
            {
                projectile.Kill();
            }

            /*if (Collision.SolidTiles((int)projectile.position.X / 16, (int)(projectile.position.X + moveDistanceX) / 16, (int)projectile.position.Y / 16, (int)(projectile.position.Y - Yaddition) / 16))
            {
                spawnedAnother = true;
                Yaddition += 1;
            }*/
            if (Main.tile[(int)(projectile.position.X + (moveDistanceX * projectile.direction)) / 16, (int)(projectile.position.Y - Yaddition) / 16].type == 0 && Main.tile[(int)(projectile.position.X + (moveDistanceX * projectile.direction)) / 16, (int)(projectile.position.Y - Yaddition) / 16].type != TileID.Trees)
            {
                canSpawn = true;
            }
            else
            {
                Yaddition += 5;
            }
            if (canSpawn && !spawnedAnother && projectile.timeLeft <= 55)        //&& projectile.timeLeft <= 5
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
                int proj = Projectile.NewProjectile(projectile.Center.X + (5f * projectile.direction), projectile.Center.Y - Yaddition, 0f, 0f, mod.ProjectileType("IceSpike"), projectile.damage, 2f, Main.myPlayer, ownerProj.whoAmI, projectile.direction);
                Main.projectile[proj].direction = projectile.direction;
                Main.projectile[proj].netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}