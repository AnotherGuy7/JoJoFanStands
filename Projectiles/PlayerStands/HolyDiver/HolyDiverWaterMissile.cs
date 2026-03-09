using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    public class HolyDiverWaterMissile : ModProjectile
    {
        private const float HomingRange = 900f;
        private const float MissileAcceleration = 0.18f;
        private const float TravelSpeed = 10f;
        private const int MissileLifetime = 180;
        private const int BurnChance = 80;
        private const int BurnDuration = 360;
        private const float DrawScale = 0.62f;

        private int TargetNPCIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 14;
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
            NPC target = ResolveTarget();

            if (target != null)
            {
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(2))
            {
                int d = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Water,
                    Main.rand.NextFloat(-1.2f, 1.2f),
                    Main.rand.NextFloat(-1.2f, 1.2f),
                    140, Color.CornflowerBlue,
                    Main.rand.NextFloat(0.55f, 1.1f));
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = Projectile.velocity * -0.45f
                                      + new Vector2(Main.rand.NextFloat(-0.4f, 0.4f),
                                                    Main.rand.NextFloat(-0.4f, 0.4f));
            }
            if (Main.rand.NextBool(3))
            {
                int m = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Cloud,
                    0f, 0f,
                    160, new Color(80, 160, 255, 60),
                    Main.rand.NextFloat(0.3f, 0.75f));
                Main.dust[m].noGravity = true;
                Main.dust[m].velocity = Projectile.velocity * -0.25f;
            }
            if (Main.rand.NextBool(4))
            {
                Vector2 perp = new Vector2(-Projectile.velocity.Y, Projectile.velocity.X);
                perp.Normalize();
                float side = Main.rand.NextBool() ? 1f : -1f;

                int s = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.SparksMech,
                    perp.X * side * Main.rand.NextFloat(1.5f, 3f),
                    perp.Y * side * Main.rand.NextFloat(1.5f, 3f),
                    0, Color.White,
                    Main.rand.NextFloat(0.4f, 0.9f));
                Main.dust[s].noGravity = true;
            }
            if (Main.rand.NextBool(5))
            {
                int f = Dust.NewDust(
                    Projectile.Center.ToPoint().ToVector2() + Projectile.velocity * 1.5f,
                    0, 0,
                    DustID.Torch,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    0, default,
                    Main.rand.NextFloat(0.35f, 0.7f));
                Main.dust[f].noGravity = true;
            }
            if (Main.rand.NextBool(8))
            {
                int b = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.SilverFlame,
                    Main.rand.NextFloat(-0.5f, 0.5f),
                    Main.rand.NextFloat(-1.5f, -0.5f),
                    180, new Color(100, 200, 255, 80),
                    Main.rand.NextFloat(0.6f, 1.1f));
                Main.dust[b].noGravity = false;
            }
        }

        private NPC ResolveTarget()
        {
            if (TargetNPCIndex >= 0 && TargetNPCIndex < Main.maxNPCs)
            {
                NPC npc = Main.npc[TargetNPCIndex];
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage)
                    return npc;
            }

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
            for (int i = 0; i < 18; i++)
            {
                float angle = MathHelper.TwoPi / 18f * i + Main.rand.NextFloat(-0.15f, 0.15f);
                float speed = Main.rand.NextFloat(2.5f, 5.5f);
                Vector2 vel = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;
                int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Water,
                    vel.X, vel.Y,
                    100, Color.DeepSkyBlue,
                    Main.rand.NextFloat(0.9f, 1.8f));
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 8; i++)
            {
                int m = Dust.NewDust(Projectile.Center, 0, 0, DustID.Cloud,
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f),
                    80, Color.AliceBlue,
                    Main.rand.NextFloat(1.2f, 2f));
                Main.dust[m].noGravity = true;
            }
            for (int i = 0; i < 6; i++)
            {
                int g = Dust.NewDust(Projectile.Center, 0, 0, DustID.SparksMech,
                    Main.rand.NextFloat(-4f, 4f),
                    Main.rand.NextFloat(-4f, 4f),
                    0, Color.White, 1.2f);
                Main.dust[g].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() / 2f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float t = 1f - i / (float)Projectile.oldPos.Length;
                Color trailColor = Color.Lerp(
                    new Color(160, 230, 255, 200),
                    new Color(20, 80, 200, 0),
                    i / (float)Projectile.oldPos.Length)
                    * 0.6f;
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition
                            + new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                Main.EntitySpriteDraw(
                    tex, pos, null, trailColor,
                    Projectile.rotation, origin,
                    DrawScale * t,
                    SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                origin,
                DrawScale,
                SpriteEffects.None,
                0);
            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                new Color(80, 180, 255, 0) * 0.45f,
                Projectile.rotation,
                origin,
                DrawScale * 1.35f,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}