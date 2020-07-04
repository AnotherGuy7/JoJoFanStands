using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class TeleportationWormhole : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 25;
            projectile.aiStyle = 0;
            projectile.timeLeft = 300;
            projectile.friendly = true;
        }

        public override void AI()
        {
            projectile.rotation += MathHelper.ToRadians(3f);       //90 degrees every 2s
            projectile.timeLeft++;
        }
    }
}