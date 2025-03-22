using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class YellowDevilProjectile : ModProjectile
    {
        public static readonly SoundStyle ImpactSound = new SoundStyle("JoJoFanStands/Sounds/SoundEffects/YellowDevil/YellowDevilProjectileImpact")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 26;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(ImpactSound, Projectile.Center);
        }
    }
}