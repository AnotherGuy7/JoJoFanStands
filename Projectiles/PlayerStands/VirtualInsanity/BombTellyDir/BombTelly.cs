using JoJoStands;
using JoJoStands.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.BombTellyDir
{
    public class BombTelly : ModProjectile
    {
        public static Texture2D tellySpawnSpritesheet;
        public static Texture2D tellySecondSpritesheet;

        /*public static readonly SoundStyle ShootSound = new SoundStyle("JoJoFanStands/Sounds/SoundEffects/BombTelly/BombTellyProjectileShoot")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };*/

        /*public static readonly SoundStyle[] SpawnSounds = new SoundStyle[6]
        {
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/BombTelly/BombTellyProjectileSpawn") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/BombTelly/BombTellySpawn1") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/BombTelly/BombTellySpawn2") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/BombTelly/BombTellySpawn3") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/BombTelly/BombTellySpawn4") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/BombTelly/BombTellySpawn5") { Volume = JoJoStands.JoJoStands.ModSoundsVolume }
        };*/

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 106;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 30 * 60;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.damage = 64;
        }

        private bool playedSpawnSound = false;
        private bool spawning = true;
        private bool secondSpritesheet;
        private int bombSpawnTimer = 0;

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 6;
                if (Projectile.alpha <= 0)
                    Projectile.alpha = 0;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;

                if (spawning && Projectile.frame >= 11)
                {
                    spawning = false;
                    Projectile.frame = 0;
                    Projectile.frameCounter = 3;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.velocity = Main.MouseWorld - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 4f;
                        Projectile.netUpdate = true;
                    }
                }
                if (!spawning && Projectile.frame >= 12)
                {
                    Projectile.frame = 0;
                    secondSpritesheet = !secondSpritesheet;
                }
            }

            if (spawning)
            {
                Projectile.velocity = Vector2.Zero;
                return;
            }

            if (!playedSpawnSound)
            {
                playedSpawnSound = true;
                //SoundEngine.PlaySound(SpawnSounds[0], Projectile.Center);
                //SoundEngine.PlaySound(SpawnSounds[Main.rand.Next(1, SpawnSounds.Length)], Projectile.Center);
            }
            if (Main.myPlayer == Projectile.owner)
            {
                bombSpawnTimer++;
                if (bombSpawnTimer > 60 - (5 * Main.player[Projectile.owner].GetModPlayer<MyPlayer>().standTier))
                {
                    bombSpawnTimer = 0;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, Projectile.height / 2f), Projectile.velocity, ModContent.ProjectileType<BombTellyBomb>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y == 0)
                Projectile.velocity.Y = -oldVelocity.Y;
            if (Projectile.velocity.X == 0)
                Projectile.velocity.X = -oldVelocity.X;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (spawning)
                Main.EntitySpriteDraw(tellySpawnSpritesheet, Projectile.position - Main.screenPosition, new Rectangle(0, 128 * Projectile.frame, 104, 128), lightColor * Projectile.Opacity, Projectile.rotation, Vector2.Zero, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            if (secondSpritesheet)
                Main.EntitySpriteDraw(tellySecondSpritesheet, Projectile.position - Main.screenPosition, new Rectangle(0, 124 * Projectile.frame, 96, 124), lightColor * Projectile.Opacity, Projectile.rotation, Vector2.Zero, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return !spawning && !secondSpritesheet;
        }
    }
}