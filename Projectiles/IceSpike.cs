using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class IceSpike : ModProjectile
    {
        public float Yaddition = 0;
        public bool spawnedAnother = false;

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 20;
            projectile.aiStyle = 0;
            projectile.timeLeft = 10;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FanPlayer FPlayer = player.GetModPlayer<FanPlayer>();
            projectile.velocity.Y += 5f;
            if (Collision.SolidTiles((int)(projectile.Center.X + 5f) / 16, (int)(projectile.Center.X + 5f) / 16, (int)(projectile.Center.Y - Yaddition) / 16, (int)(projectile.Center.Y - Yaddition) / 16))
            {
                spawnedAnother = true;
                Yaddition += 1;
            }
            /*if (!spawnedAnother && projectile.timeLeft <= 5)
            {
                if (FPlayer.additionMode)
                {
                    FPlayer.numberOfSpikesCreated += 0.20f;
                }
                if (!FPlayer.additionMode)
                {
                    FPlayer.numberOfSpikesCreated -= 0.20f;
                }
                spawnedAnother = true;
                int proj = Projectile.NewProjectile(projectile.Center.X + 5f, projectile.Center.Y - Yaddition, 0f, 0f, mod.ProjectileType("IceSpike"), 25, 2f, Main.myPlayer);
                Main.projectile[proj].scale = 1 + (FPlayer.numberOfSpikesCreated / 5);
                Main.projectile[proj].netUpdate = true;
                projectile.netUpdate = true;
            }*/
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}