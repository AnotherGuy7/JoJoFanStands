using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    /// <summary>
    /// Tiny short-lived hitbox punched out by the Water Replicant sentry.
    /// Disappears after 1 hit or 4 ticks.
    /// </summary>
    public class HolyDiverReplicantPunch : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 4;
            Projectile.penetrate = 1;
            Projectile.alpha = 255; // invisible — it's just a hitbox
        }

        public override void AI()
        {
            // Kill velocity quickly so the hitbox stays near the replicant
            Projectile.velocity *= 0.4f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Small splash on impact
            for (int i = 0; i < 6; i++)
            {
                int d = Dust.NewDust(target.Center, 0, 0, DustID.Water,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f),
                    100, Color.CornflowerBlue, 1.2f);
                Main.dust[d].noGravity = true;
            }
        }
    }
}