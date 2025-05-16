using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.YellowDevilDir
{
    public class YellowDevil : ModProjectile
    {
        public static Texture2D splitUpTexture;

        public static readonly SoundStyle ShootSound = new SoundStyle("JoJoFanStands/Sounds/SoundEffects/YellowDevil/YellowDevilProjectileShoot")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };

        public static readonly SoundStyle[] SpawnSounds = new SoundStyle[6]
        {
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/YellowDevil/YellowDevilProjectileSpawn") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/YellowDevil/YellowDevilSpawn1") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/YellowDevil/YellowDevilSpawn2") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/YellowDevil/YellowDevilSpawn3") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/YellowDevil/YellowDevilSpawn4") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/YellowDevil/YellowDevilSpawn5") { Volume = JoJoStands.JoJoStands.ModSoundsVolume }
        };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 224;
            Projectile.height = 126;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 1140;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        private bool splittingUp = false;
        private bool playedSpawnSound = false;

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 6;
                if (Projectile.alpha <= 0)
                    Projectile.alpha = 0;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;
                if (!splittingUp && Projectile.frame >= 7)
                {
                    Projectile.frame = 0;
                    splittingUp = true;
                }
                else if (Projectile.frame >= 11)
                {
                    Projectile.Kill();
                    return;
                }
                if (splittingUp && Projectile.frame <= 10)
                {
                    Projectile.frameCounter = -5;
                    if (Projectile.frame >= 1)
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            int offsetFrame = Projectile.frame - 1;
                            Vector2 offset = new Vector2(72 + (32 * (offsetFrame % 3)), 16 + (38 * (offsetFrame / 3)));
                            if (Projectile.spriteDirection == -1)
                                offset.X = 72 + 96 - (32 * (offsetFrame % 3));
                            Vector2 shootVelocity = Main.MouseWorld - (Projectile.position + offset);
                            shootVelocity.Normalize();
                            shootVelocity *= 12f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position + offset, shootVelocity, ModContent.ProjectileType<YellowDevilProjectile>(), (int)Projectile.ai[0], 2f, Projectile.owner);
                            SoundEngine.PlaySound(ShootSound, Projectile.Center);
                        }
                    }
                }
            }

            if (!playedSpawnSound)
            {
                playedSpawnSound = true;
                SoundEngine.PlaySound(SpawnSounds[0], Projectile.Center);
                SoundEngine.PlaySound(SpawnSounds[Main.rand.Next(1, SpawnSounds.Length)], Projectile.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (splittingUp)
                Main.EntitySpriteDraw(splitUpTexture, Projectile.position - Main.screenPosition, new Rectangle(0, Projectile.height * Projectile.frame, Projectile.width, Projectile.height), lightColor * Projectile.Opacity, Projectile.rotation, Vector2.Zero, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return !splittingUp;
        }
    }
}