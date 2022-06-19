using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class TeleportationWormhole : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(3f);       //90 degrees every 2s
            Projectile.timeLeft++;
        }
    }
}