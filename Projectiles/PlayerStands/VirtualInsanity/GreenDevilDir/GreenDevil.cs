using JoJoStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.GreenDevilDir
{
    public class GreenDevil : ModProjectile
    {
        public static Texture2D impactSpritesheet;

        public static readonly SoundStyle ShootSound = new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GreenDevil/GreenDevilProjectileShoot")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };

        /*public static readonly SoundStyle[] SpawnSounds = new SoundStyle[6]
        {
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GreenDevil/GreenDevilProjectileSpawn") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GreenDevil/GreenDevilSpawn1") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GreenDevil/GreenDevilSpawn2") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GreenDevil/GreenDevilSpawn3") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GreenDevil/GreenDevilSpawn4") { Volume = JoJoStands.JoJoStands.ModSoundsVolume },
            new SoundStyle("JoJoFanStands/Sounds/SoundEffects/GreenDevil/GreenDevilSpawn5") { Volume = JoJoStands.JoJoStands.ModSoundsVolume }
        };*/

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
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

        private bool exploding = false;
        private bool playedSpawnSound = false;
        private bool explosionEffect = false;

        public override void AI()
        {
            if (!exploding)
            {
                Projectile.velocity.Y += 0.1f;
                if (Projectile.velocity.Y >= 18f)
                    Projectile.velocity.Y = 18f;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;

                if (exploding)
                {
                    Projectile.frameCounter += 1;
                    if (Projectile.frame >= 21)
                    {
                        Projectile.Kill();
                        return;
                    }
                    if (!explosionEffect && Projectile.frame >= 11)
                    {
                        explosionEffect = true;
                        for (int i = 0; i < 30; i++)
                        {
                            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Alpha: 100, Scale: 1.5f);
                            Main.dust[dustIndex].velocity *= 1.4f;
                        }
                        for (int i = 0; i < 20; i++)
                        {
                            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Alpha: 100, Scale: 3.5f);
                            Main.dust[dustIndex].noGravity = true;
                            Main.dust[dustIndex].velocity *= 7f;
                            dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Alpha: 100, Scale: 1.5f);
                            Main.dust[dustIndex].velocity *= 3f;
                        }
                        bool crit = false;
                        MyPlayer mPlayer = Main.player[Projectile.owner].GetModPlayer<MyPlayer>();
                        if (Main.rand.Next(1, 100 + 1) <= mPlayer.standCritChangeBoosts)
                            crit = true;
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.active)
                            {
                                if (npc.lifeMax > 5 && !npc.friendly && !npc.hide && !npc.immortal && npc.Distance(Projectile.Center) <= 12f * 16f)
                                {
                                    int hitDirection = -1;
                                    if (npc.position.X - Projectile.position.X > 0)
                                        hitDirection = 1;

                                    NPC.HitInfo hitInfo = new NPC.HitInfo()
                                    {
                                        Damage = (int)Projectile.ai[0],
                                        Knockback = 8f,
                                        HitDirection = hitDirection,
                                        Crit = crit
                                    };
                                    npc.StrikeNPC(hitInfo);
                                }
                            }
                        }
                        SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
                    }
                }
                if (!exploding && Projectile.frame >= 3)
                    Projectile.frame = 0;
            }

            if (!playedSpawnSound)
            {
                playedSpawnSound = true;
                //SoundEngine.PlaySound(SpawnSounds[0], Projectile.Center);
                //SoundEngine.PlaySound(SpawnSounds[Main.rand.Next(1, SpawnSounds.Length)], Projectile.Center);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!exploding)
                exploding = true;
            else
                return false;

            Projectile.velocity = Vector2.Zero;
            float numberProjectiles = 12;
            float rotation = MathHelper.ToRadians(30);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(0f, -32f).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<GoopProjectile>(), Projectile.damage, Projectile.knockBack / 2f, Projectile.owner);
                Main.projectile[projIndex].netUpdate = true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (exploding)
                Main.EntitySpriteDraw(impactSpritesheet, Projectile.position - Main.screenPosition, new Rectangle(0, 94 * Projectile.frame, Projectile.width, 94), lightColor, Projectile.rotation, Vector2.Zero, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return !exploding;
        }
    }
}