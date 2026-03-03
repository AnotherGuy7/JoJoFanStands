using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    /// <summary>
    /// Homing water missile spawned by the Water Cannon ability when Right Click is held.
    /// Locks onto a specific NPC index passed via Projectile.ai[0].
    /// If the target dies mid-flight, falls back to nearest hostile NPC.
    ///
    /// Projectile.ai[0] = target NPC whoAmI  (-1 = use nearest fallback)
    /// Projectile.ai[1] = unused / reserved
    /// </summary>
    public class HolyDiverWaterMissile : ModProjectile
    {
        // -------------------------------------------------------
        // Tuning constants
        // -------------------------------------------------------
        private const float HomingRange = 900f;   // pixels — fallback detection radius
        private const float MissileAcceleration = 0.18f;  // velocity nudge per tick (like TrackerBubble's BubbleAcceleration)
        private const float TravelSpeed = 10f;    // max speed cap
        private const int MissileLifetime = 180;    // ticks (~3 s)
        private const int BurnChance = 80;     // out of 100
        private const int BurnDuration = 360;

        private int TargetNPCIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = MissileLifetime;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            // ---- Resolve target — prefer assigned, fall back to nearest ----
            NPC target = ResolveTarget();

            if (target != null)
            {
                // Same nudge pattern as TrackerBubble: add a small step toward target each tick,
                // clamp each axis if we're already wildly past MaxSpeed, then soft-cap total length.
                Vector2 nudge = target.Center - Projectile.Center;
                nudge.Normalize();
                nudge *= MissileAcceleration;

                if (Math.Abs(Projectile.velocity.X + nudge.X) > TravelSpeed)
                    nudge.X = Math.Sign(nudge.X) * 0.02f;
                if (Math.Abs(Projectile.velocity.Y + nudge.Y) > TravelSpeed)
                    nudge.Y = Math.Sign(nudge.Y) * 0.02f;

                Projectile.velocity += nudge;

                if (Projectile.velocity.Length() >= TravelSpeed)
                    Projectile.velocity *= 0.98f;
            }

            // ---- Rotation ----
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // ---- Dust trail — watery + hot ----
            if (Main.rand.NextBool(2))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Cloud, 0f, 0f, 120, Color.CornflowerBlue, Main.rand.NextFloat(0.5f, 1.2f));
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = Projectile.velocity * -0.3f;
            }
            if (Main.rand.NextBool(4))
            {
                int f = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Torch, 0f, 0f, 0, default, Main.rand.NextFloat(0.4f, 0.9f));
                Main.dust[f].noGravity = true;
            }
        }

        private NPC ResolveTarget()
        {
            // Check pre-assigned target first
            if (TargetNPCIndex >= 0 && TargetNPCIndex < Main.maxNPCs)
            {
                NPC npc = Main.npc[TargetNPCIndex];
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage)
                    return npc;
            }

            // Target gone — find nearest hostile within range as fallback
            NPC best = null;
            float bestD = HomingRange;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5 || npc.dontTakeDamage)
                    continue;
                float d = Vector2.Distance(Projectile.Center, npc.Center);
                if (d < bestD) { bestD = d; best = npc; }
            }
            if (best != null)
                TargetNPCIndex = best.whoAmI;

            return best;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.Next(0, 101) < BurnChance)
                target.AddBuff(BuffID.OnFire, BurnDuration);

            // Small water explosion on impact
            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Cloud,
                    Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f),
                    100, Color.DeepSkyBlue, Main.rand.NextFloat(1f, 1.8f));
                Main.dust[d].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float t = 1f - i / (float)Projectile.oldPos.Length;
                Color color = Color.Lerp(Color.DeepSkyBlue, Color.Transparent, 1f - t) * 0.55f;
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                Main.EntitySpriteDraw(tex, pos, null, color, Projectile.rotation,
                    tex.Size() / 2f, Projectile.scale * t, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null,
                lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}