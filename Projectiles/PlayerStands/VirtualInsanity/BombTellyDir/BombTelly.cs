using JoJoStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.BombTellyDir
{
    public class BombTelly : ModProjectile
    {
        public static Texture2D tellySpawnSpritesheet;
        public static Texture2D tellySecondSpritesheet;

        public static readonly SoundStyle BombDropSound = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/BombDrone/BombDrop")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };

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
            if (Main.myPlayer == Projectile.owner)
            {
                bombSpawnTimer++;
                if (bombSpawnTimer > 60 - (5 * Main.player[Projectile.owner].GetModPlayer<MyPlayer>().standTier))
                {
                    bombSpawnTimer = 0;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, Projectile.height / 2f), Projectile.velocity, ModContent.ProjectileType<BombTellyBomb>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                    if (JoJoFanStands.SoundsLoaded)
                        SoundEngine.PlaySound(BombDropSound, Projectile.Center);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
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
                    if (npc.lifeMax > 5 && !npc.friendly && !npc.hide && !npc.immortal && npc.Distance(Projectile.Center) <= 8f * 16f)
                    {
                        int hitDirection = -1;
                        if (npc.position.X - Projectile.position.X > 0)
                            hitDirection = 1;

                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                        {
                            Damage = Projectile.damage * 2,
                            Knockback = 4f,
                            HitDirection = hitDirection,
                            Crit = crit
                        };
                        npc.StrikeNPC(hitInfo);
                    }
                }
            }
            SoundEngine.PlaySound(SoundID.Item14.WithPitchOffset(-0.4f), Projectile.Center);
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