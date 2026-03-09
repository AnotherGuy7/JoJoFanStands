using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    public class HolyDiverMine : ModProjectile
    {
        private const int ExplosionDamageMultiplier = 3;
        private const float DetectionRange = 300f;
        private const float HomingAcceleration = 0.22f;
        private const float MaxHomingSpeed = 12f;
        private const int WetChance = 85;
        private const int WetDuration = 480;
        private const int MineLifetime = 60 * 30;

        private const float BobAmplitude = 5f;
        private const float BobSpeed = 0.04f;

        private int TargetIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private bool IsHoming
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }

        private float bobOriginY = -1f;
        private int pulseTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = MineLifetime;
            Projectile.penetrate = 1;
            Projectile.ai[0] = -1f;
        }

        public override void AI()
        {
            pulseTimer++;
            if (!IsHoming)
            {
                if (bobOriginY < 0f)
                    bobOriginY = Projectile.position.Y;
                float bobOffset = (float)Math.Sin(pulseTimer * BobSpeed) * BobAmplitude;
                Projectile.position.Y = bobOriginY + bobOffset;
                Projectile.velocity = Vector2.Zero;
                Projectile.rotation = (float)Math.Sin(pulseTimer * BobSpeed * 0.7f) * 0.08f;
                if (Main.rand.NextBool(18))
                {
                    Vector2 spawnPos = Projectile.Center + new Vector2(
                        Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-4f, 4f));

                    int d = Dust.NewDust(spawnPos, 2, 2, DustID.Water,
                        Main.rand.NextFloat(-0.3f, 0.3f),
                        Main.rand.NextFloat(-1.2f, -0.4f),
                        150, Color.LightCyan, Main.rand.NextFloat(0.5f, 1.1f));
                    Main.dust[d].noGravity = true;
                    Main.dust[d].fadeIn = 0.4f;
                }

                if (Main.rand.NextBool(55))
                {
                    Vector2 spawnPos = Projectile.Center + new Vector2(
                        Main.rand.NextFloat(-8f, 8f), 0f);

                    int d = Dust.NewDust(spawnPos, 4, 4, DustID.Water,
                        Main.rand.NextFloat(-0.2f, 0.2f),
                        Main.rand.NextFloat(-0.8f, -0.3f),
                        100, Color.CornflowerBlue, Main.rand.NextFloat(1.2f, 1.8f));
                    Main.dust[d].noGravity = true;
                }

                NPC found = null;
                float bestD = DetectionRange;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.friendly || npc.lifeMax <= 5 || npc.dontTakeDamage)
                        continue;
                    float d = Vector2.Distance(Projectile.Center, npc.Center);
                    if (d < bestD) { bestD = d; found = npc; }
                }

                if (found != null)
                {
                    TargetIndex = found.whoAmI;
                    IsHoming = true;
                    Projectile.tileCollide = false;
                    SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                NPC target = ResolveTarget();

                if (target != null)
                {
                    Vector2 nudge = target.Center - Projectile.Center;
                    nudge.Normalize();
                    nudge *= HomingAcceleration;

                    if (Math.Abs(Projectile.velocity.X + nudge.X) > MaxHomingSpeed)
                        nudge.X = Math.Sign(nudge.X) * 0.02f;
                    if (Math.Abs(Projectile.velocity.Y + nudge.Y) > MaxHomingSpeed)
                        nudge.Y = Math.Sign(nudge.Y) * 0.02f;

                    Projectile.velocity += nudge;
                    if (Projectile.velocity.Length() >= MaxHomingSpeed)
                        Projectile.velocity *= 0.98f;
                }
                else
                {
                    Projectile.velocity *= 0.92f;
                    if (Projectile.velocity.Length() < 0.5f)
                    {
                        Projectile.velocity = Vector2.Zero;
                        IsHoming = false;
                        TargetIndex = -1;
                        Projectile.tileCollide = false;
                        bobOriginY = Projectile.position.Y;
                    }
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                if (Main.rand.NextBool(2))
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                        DustID.Water,
                        Projectile.velocity.X * -0.4f, Projectile.velocity.Y * -0.4f,
                        100, Color.DeepSkyBlue, Main.rand.NextFloat(0.8f, 1.5f));
                    Main.dust[d].noGravity = true;
                }
                if (Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                        DustID.Water, 0f, 0f,
                        120, Color.CornflowerBlue, Main.rand.NextFloat(0.4f, 0.9f));
                    Main.dust[d].noGravity = true;
                }
            }
        }

        private NPC ResolveTarget()
        {
            if (TargetIndex >= 0 && TargetIndex < Main.maxNPCs)
            {
                NPC npc = Main.npc[TargetIndex];
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage)
                    return npc;
            }

            NPC best = null;
            float bestD = DetectionRange * 1.5f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5 || npc.dontTakeDamage)
                    continue;
                float d = Vector2.Distance(Projectile.Center, npc.Center);
                if (d < bestD) { bestD = d; best = npc; }
            }
            if (best != null)
                TargetIndex = best.whoAmI;

            return best;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.Next(0, 101) < WetChance)
                target.AddBuff(BuffID.Wet, WetDuration);

            Explode();
        }

        public override void OnKill(int timeLeft)
        {
            Explode();
        }

        private void Explode()
        {
            for (int i = 0; i < 30; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float speed = Main.rand.NextFloat(2f, 7f);
                Vector2 vel = angle.ToRotationVector2() * speed;

                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Water,
                    vel.X, vel.Y, 0, Color.DeepSkyBlue, Main.rand.NextFloat(1.5f, 3f));
                Main.dust[d].noGravity = false;
            }

            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Water,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-5f, -1.5f),
                    100, Color.LightCyan, Main.rand.NextFloat(1f, 2.2f));
                Main.dust[d].noGravity = true;
            }

            for (int i = 0; i < 15; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Cloud,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 1f),
                    120, Color.AliceBlue, Main.rand.NextFloat(0.8f, 1.6f));
                Main.dust[d].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= ExplosionDamageMultiplier;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float scale = 1f;

            if (!IsHoming)
            {
                float breathe = 0.7f + (float)Math.Sin(pulseTimer * BobSpeed * 1.3f) * 0.15f;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive,
                    Main.DefaultSamplerState, DepthStencilState.None,
                    Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                Main.EntitySpriteDraw(tex, drawPos, null,
                    Color.CornflowerBlue * (0.22f * breathe), Projectile.rotation,
                    tex.Size() / 2f, scale * 1.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex, drawPos, null,
                    Color.DeepSkyBlue * (0.12f * breathe), Projectile.rotation,
                    tex.Size() / 2f, scale * 2.4f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(tex, drawPos, null,
                    Color.LightCyan * (0.06f * breathe), Projectile.rotation,
                    tex.Size() / 2f, scale * 3.5f, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                    Main.DefaultSamplerState, DepthStencilState.None,
                    Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                        continue;
                    if (Vector2.Distance(Projectile.Center, npc.Center) <= DetectionRange)
                    {
                        float alertAlpha = 0.45f + (float)Math.Sin(pulseTimer * 0.25f) * 0.25f;
                        Main.EntitySpriteDraw(tex, drawPos, null,
                            Color.DeepSkyBlue * alertAlpha, Projectile.rotation,
                            tex.Size() / 2f, scale * 1.35f, SpriteEffects.None, 0);
                        break;
                    }
                }
            }
            Main.EntitySpriteDraw(tex, drawPos, null,
                lightColor, Projectile.rotation, tex.Size() / 2f, scale, SpriteEffects.None, 0);

            return false;
        }
    }
}