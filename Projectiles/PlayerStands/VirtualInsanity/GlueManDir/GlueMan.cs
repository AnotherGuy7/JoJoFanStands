using JoJoStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.GlueManDir
{
    public class GlueMan : ModProjectile
    {
        public static Texture2D spawnSheet;
        public static Texture2D npcGlueIcon;

        /*public static readonly SoundStyle ShootSound = new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GlueMan/GlueManProjectileShoot")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };

        public static readonly SoundStyle[] SpawnSounds = new SoundStyle[6]
        {
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GlueMan/GlueManProjectileSpawn") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GlueMan/GlueManSpawn1") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GlueMan/GlueManSpawn2") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GlueMan/GlueManSpawn3") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GlueMan/GlueManSpawn4") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GlueMan/GlueManSpawn5") { Volume = JoJoStands.JoJoStands.ModSoundsVolume }
        };*/

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 92;
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
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;
                if (spawning && Projectile.frame >= 22)
                {
                    Projectile.frame = 0;
                    spawning = false;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.velocity = Main.MouseWorld - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 12f;
                        Projectile.netUpdate = true;
                        if (Projectile.velocity.X > 0)
                            Projectile.direction = Projectile.spriteDirection = 1;
                        else
                            Projectile.direction = Projectile.spriteDirection = -1;
                    }
                }
                else if (!spawning && Projectile.frame >= 7)
                    Projectile.frame = 0;
            }

            if (spawning)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.position = (Main.player[Projectile.owner].Center - new Vector2(0f, (37 * 4) - 12f)) + new Vector2(-58, 0) - new Vector2(0f, 37 / 2f) + new Vector2(5, -46);
                return;
            }

            if (!playedSpawnSound)
            {
                playedSpawnSound = true;
                //SoundEngine.PlaySound(SpawnSounds[0], Projectile.Center);
                //SoundEngine.PlaySound(SpawnSounds[Main.rand.Next(1, SpawnSounds.Length)], Projectile.Center);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlueBomb>(), 5 * Main.player[Projectile.owner].GetModPlayer<MyPlayer>().standTier, 1f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (spawning)
                Main.EntitySpriteDraw(spawnSheet, Projectile.position - Main.screenPosition, new Rectangle(0, 92 * Projectile.frame, 90, 92), lightColor * Projectile.Opacity, Projectile.rotation, Vector2.Zero, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return !spawning;
        }
    }
}