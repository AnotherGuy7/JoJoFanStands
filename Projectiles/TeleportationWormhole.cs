using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class TeleportationWormhole : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(3f);       //90 degrees every 2s
            Projectile.timeLeft++;

            if (Main.rand.Next(0, 1 + 1) == 0)
            {
                int dustIndex = Dust.NewDust(Projectile.position, 24, 24, DustID.Smoke, Scale: 0.7f);
                Vector2 velocity = Projectile.Center - Main.dust[dustIndex].position;
                velocity.Normalize();
                Main.dust[dustIndex].velocity = velocity * 1.4f;
                Main.dust[dustIndex].color = Color.Black;
            }
        }
    }
}