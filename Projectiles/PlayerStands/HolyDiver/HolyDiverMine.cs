using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    /// <summary>
    /// Scorching Hot Mine.
    /// - Placed on the ground and sits idle.
    /// - If an enemy enters DetectionRange, it homes in and explodes on contact.
    /// - Explosion deals extreme damage and applies OnFire.
    ///
    /// ai[0] = target NPC whoAmI (-1 = idle, no target)
    /// ai[1] = state: 0 = idle/placed, 1 = homing
    /// </summary>
    public class HolyDiverMine : ModProjectile
    {
        // -------------------------------------------------------
        // Tuning
        // -------------------------------------------------------
        private const int ExplosionDamageMultiplier = 3;   // applied on top of base Projectile.damage
        private const float DetectionRange = 300f; // pixels — radius before mine starts homing
        private const float HomingAcceleration = 0.22f;
        private const float MaxHomingSpeed = 12f;
        private const int BurnChance = 85;   // out of 100
        private const int BurnDuration = 480;
        private const int MineLifetime = 60 * 30; // 30 seconds before self-destruct

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

        // Visual pulse timer
        private int pulseTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = MineLifetime;
            Projectile.penetrate = 1;
            Projectile.ai[0] = -1f; // no target
        }

        public override void AI()
        {
            pulseTimer++;

            if (!IsHoming)
            {
                // ---- Idle: sit still, scan for targets ----
                Projectile.velocity = Vector2.Zero;

                // Idle dust pulse every 30 ticks
                if (pulseTimer % 30 == 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        float angle = MathHelper.TwoPi / 6f * i;
                        Vector2 spawnPos = Projectile.Center + angle.ToRotationVector2() * 10f;
                        int d = Dust.NewDust(spawnPos, 2, 2, DustID.Torch, 0f, 0f, 0, default, 0.7f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = angle.ToRotationVector2() * 0.6f;
                    }
                }

                // Scan for nearest enemy in range
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
                    Projectile.tileCollide = false; // fly over tiles when homing
                    SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                // ---- Homing: nudge toward target each tick (TrackerBubble pattern) ----
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
                    // Target gone — decelerate and return to idle
                    Projectile.velocity *= 0.92f;
                    if (Projectile.velocity.Length() < 0.5f)
                    {
                        Projectile.velocity = Vector2.Zero;
                        IsHoming = false;
                        TargetIndex = -1;
                        Projectile.tileCollide = true;
                    }
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                // Homing dust trail
                if (Main.rand.NextBool(2))
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                        DustID.Torch, Projectile.velocity.X * -0.3f, Projectile.velocity.Y * -0.3f,
                        0, default, Main.rand.NextFloat(0.8f, 1.5f));
                    Main.dust[d].noGravity = true;
                }
                if (Main.rand.NextBool(3))
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                        DustID.Cloud, 0f, 0f, 120, Color.OrangeRed, Main.rand.NextFloat(0.4f, 0.9f));
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

            // Fallback to nearest
            NPC best = null;
            float bestD = DetectionRange * 1.5f; // slightly wider fallback so it doesn't give up too early
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
            if (Main.rand.Next(0, 101) < BurnChance)
                target.AddBuff(BuffID.OnFire, BurnDuration);

            Explode();
        }

        public override void OnKill(int timeLeft)
        {
            Explode();
        }

        private void Explode()
        {
            // Fire burst
            for (int i = 0; i < 25; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Torch,
                    Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-6f, 6f),
                    0, default, Main.rand.NextFloat(1.5f, 3f));
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 2.5f;
            }
            // Steam cloud
            for (int i = 0; i < 15; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Cloud,
                    Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f),
                    100, Color.OrangeRed, Main.rand.NextFloat(1f, 2f));
                Main.dust[d].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Boost damage to extreme on contact
            modifiers.FinalDamage *= ExplosionDamageMultiplier;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            // Idle: gentle bob + warning glow pulse when a target is nearby
            float scale = 1f;
            if (!IsHoming)
            {
                float pulse = (float)Math.Sin(pulseTimer * 0.12f) * 0.08f;
                scale = 1f + pulse;

                // Glow orange when a target is in range
                float glowAlpha = 0f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                        continue;
                    if (Vector2.Distance(Projectile.Center, npc.Center) <= DetectionRange)
                    {
                        glowAlpha = 0.55f + (float)Math.Sin(pulseTimer * 0.25f) * 0.3f;
                        break;
                    }
                }

                if (glowAlpha > 0f)
                {
                    Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null,
                        Color.OrangeRed * glowAlpha, Projectile.rotation,
                        tex.Size() / 2f, scale * 1.4f, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null,
                lightColor, Projectile.rotation, tex.Size() / 2f, scale, SpriteEffects.None, 0);

            return false;
        }
    }
}