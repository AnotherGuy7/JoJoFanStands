using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class Icicle : ModProjectile
    {
        public override string Texture
        {
            get { return mod.Name + "/Projectiles/IceSpike"; }
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 20;
            projectile.aiStyle = 0;
            projectile.timeLeft = 100;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = false;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            projectile.velocity.Y++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}