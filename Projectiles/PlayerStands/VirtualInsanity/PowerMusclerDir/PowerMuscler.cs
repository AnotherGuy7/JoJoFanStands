using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.GreenDevilDir;
using JoJoStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.PowerMusclerDir
{
    public class PowerMuscler : ModProjectile
    {
        public static Texture2D spawnSheet;

        public static readonly SoundStyle FlySound = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/PowerMuscler/PowerMusclerThrown")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };
        public static readonly SoundStyle CrashSound = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/PowerMuscler/MetalCrash")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };
        public static readonly SoundStyle ImpactSound = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/PowerMuscler/Impact")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 116;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 10 * 60;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        private bool spawning = true;
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
            if (Projectile.frameCounter >= 12)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;
                if (spawning && Projectile.frame >= 3)
                {
                    spawning = false;
                    Projectile.frame = 0;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.velocity = Main.MouseWorld - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 16f;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                        Projectile.netUpdate = true;
                        if (Projectile.velocity.X > 0)
                            Projectile.direction = Projectile.spriteDirection = 1;
                        else
                            Projectile.direction = Projectile.spriteDirection = -1;
                    }
                }
                else if (!spawning && Projectile.frame >= 6)
                    Projectile.frame = 0;
                if (!spawning)
                    Projectile.frameCounter = 8;
            }
            if (spawning)
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }
            else
                Projectile.spriteDirection = 1;

            if (!playedSpawnSound)
            {
                playedSpawnSound = true;
                if (JoJoFanStands.SoundsLoaded)
                    SoundEngine.PlaySound(FlySound, Projectile.Center);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.velocity = Vector2.Zero;
            float numberProjectiles = 12;
            float rotation = MathHelper.ToRadians(30);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(0f, -32f).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<PowerPieces>(), Projectile.damage, Projectile.knockBack / 2f, Projectile.owner);
                Main.projectile[projIndex].netUpdate = true;
            }
            if (JoJoFanStands.SoundsLoaded)
            {
                SoundEngine.PlaySound(CrashSound, Projectile.Center);
                SoundEngine.PlaySound(ImpactSound, Projectile.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (spawning)
                Main.EntitySpriteDraw(spawnSheet, Projectile.position - Main.screenPosition, new Rectangle(0, 116 * Projectile.frame, 100, 116), lightColor * Projectile.Opacity, Projectile.rotation, Vector2.Zero, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return !spawning;
        }
    }
}