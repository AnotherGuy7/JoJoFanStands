using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.Metempsychosis
{
    public class SoulEffect : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 1500;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 velocity = player.Center - Projectile.Center;
            velocity.Normalize();
            velocity *= 4f;
            Projectile.velocity = velocity;

            Vector2 dustVelocity = velocity;
            dustVelocity *= -Main.rand.Next(1, 100 + 1) * 0.02f;
            int amountOfDusts = Main.rand.Next(3, 7 + 1);
            for (int d = 0; d < amountOfDusts; d++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, dustVelocity.X, dustVelocity.Y);
                Main.dust[dustIndex].noGravity = true;
            }

            if (player.whoAmI == Main.myPlayer && Vector2.Distance(player.Center, Projectile.Center) < 2f * 16f)
            {
                player.Heal((int)Projectile.ai[0]);
                SoundEngine.PlaySound(SoundID.Item8.WithPitchOffset(0.8f), Projectile.Center);
                Projectile.Kill();
            }
        }
    }
}