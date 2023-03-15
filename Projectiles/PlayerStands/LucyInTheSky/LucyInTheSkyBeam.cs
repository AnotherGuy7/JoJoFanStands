using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.LucyInTheSky
{
    public class LucyInTheSkyBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 2;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 1500;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
        }

        public Vector2 targetPosition;

        public override void AI()
        {
            Vector2 velocity = targetPosition - Projectile.Center;
            velocity.Normalize();
            velocity *= 12f;

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.6f);
            Projectile.rotation = velocity.ToRotation();

            if (Vector2.Distance(targetPosition, Projectile.Center) <= 16)
            {
                int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LightMarker>(), 0, 0f, Projectile.owner, Projectile.ai[0]);
                Main.player[Projectile.owner].GetModPlayer<FanPlayer>().lucySelectedMarkerWhoAmI = index;
                Projectile.Kill();
            }
        }
    }
}