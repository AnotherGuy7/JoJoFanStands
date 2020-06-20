using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class Crystal : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.height = 22;
            projectile.width = 22;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
			projectile.timeLeft = 100;
        }

        public override void AI()
        {
            projectile.ai[0]++;
            if (projectile.ai[0] >= 80f && projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                projectile.velocity = (Main.MouseWorld - projectile.Center) * 16;
            }
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}