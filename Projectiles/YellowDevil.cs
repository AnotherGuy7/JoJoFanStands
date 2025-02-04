using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class YellowDevil : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 12;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        { target.AddBuff(BuffID.Bleeding, 360); }

        public override void AI()
        { Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(75f); }
    }
}