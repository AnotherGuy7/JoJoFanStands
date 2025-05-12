using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.PowerMusclerDir
{
    public class PowerPieces : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        private bool setFrame = false;

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 10 * 60;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.damage = 48;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y >= 5f)
                Projectile.velocity.Y = 5f;

            Projectile.rotation += Projectile.velocity.X / 10f;

            if (Projectile.timeLeft <= 60)
                Projectile.alpha = 255 - (int)(255 * (Projectile.timeLeft / 60f));

            if (!setFrame)
            {
                setFrame = true;
                Projectile.frame = Main.rand.Next(0, Main.projFrames[Projectile.type]);
            }
            if (Main.rand.Next(0, 1 + 1) == 0)
                Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke)].noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity += target.velocity * -0.6f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y == 0)
                Projectile.velocity.Y = -oldVelocity.Y * 0.9f;
            if (Projectile.velocity.X == 0)
                Projectile.velocity.X = -oldVelocity.X * 0.4f;
            return false;
        }
    }
}