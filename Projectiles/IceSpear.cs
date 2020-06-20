using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class IceSpear : ModProjectile
    {
        public override string Texture => mod.Name + "/Projectiles/IceJavelin";

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.JavelinFriendly);
            projectile.timeLeft = 1800;
            projectile.scale = 0.5f;
        }

        private bool released = false;

        public override void AI()
        {
            if (!released)
            {
                Vector2 direction = Main.MouseWorld - projectile.Center;
                projectile.scale += Main.projectile[(int)projectile.ai[0]].ai[0];
                projectile.rotation = direction.ToRotation();
                projectile.velocity = Vector2.Zero;
                projectile.position = Main.projectile[(int)projectile.ai[0]].position;
            }
            if (!Main.mouseRight && !released)
                released = true;
            if (released && projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                projectile.velocity = Main.MouseWorld - projectile.Center;
                projectile.velocity.Normalize();
                projectile.velocity *= Main.projectile[(int)projectile.ai[0]].ai[0];
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}