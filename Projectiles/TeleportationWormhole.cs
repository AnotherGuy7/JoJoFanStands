using Terraria;
using Terraria.ID;
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
            projectile.rotation += (float)projectile.direction * 0.8f;
            if (Items.BackInBlack.teleporting)
            {
                projectile.scale += 1.13f;
            }
            else
            {
                projectile.Kill();
            }
            projectile.position = Main.player[projectile.owner].position;
        }
    }
}