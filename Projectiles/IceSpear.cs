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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}