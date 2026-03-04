using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    public class HolyDiverWaterCannon : ModProjectile
    {
        // How long the beam lingers before fading (in ticks, before extraUpdates multiply it)
        private const int BeamLifetime = 55;
        // Burn chance (out of 100)
        private const int BurnChance = 75;
        // Burn debuff duration (ticks)
        private const int BurnDuration = 300;
        // Alpha fades out near death
        private float Alpha => MathHelper.Clamp((float)Projectile.timeLeft / BeamLifetime, 0f, 1f);

        public override void SetStaticDefaults()
        {
            // Longer trail for a beam effect
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            // Much bigger hitbox — feels like a real beam
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = BeamLifetime;
            // Punches through many enemies
            Projectile.penetrate = 8;
            // 6 extra updates per tick = ~7× the normal travel per frame → very long range
            Projectile.extraUpdates = 6;
        }

        public override void AI()
        {
            // ---- Heavy particle trail ------------------------------------------------

            // Core water orbs — fired every sub-tick
            for (int i = 0; i < 3; i++)
            {
                Vector2 perpOffset = new Vector2(-Projectile.velocity.Y, Projectile.velocity.X);
                perpOffset.Normalize();
                perpOffset *= Main.rand.NextFloat(-8f, 8f);

                int water = Dust.NewDust(
                    Projectile.position + perpOffset, Projectile.width, Projectile.height,
                    DustID.Water,
                    Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f,
                    80, Color.DeepSkyBlue, Main.rand.NextFloat(1.2f, 2.2f));
                Main.dust[water].noGravity = true;
            }

            // Steam clouds — slightly wider spread
            if (Main.rand.NextBool(2))
            {
                int cloud = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Cloud,
                    Projectile.velocity.X * 0.15f + Main.rand.NextFloat(-1f, 1f),
                    Projectile.velocity.Y * 0.15f + Main.rand.NextFloat(-1f, 1f),
                    120, Color.SkyBlue, Main.rand.NextFloat(1.0f, 1.8f));
                Main.dust[cloud].noGravity = true;
            }

            // Hot blue-white sparks / electric arcs
            if (Main.rand.NextBool(2))
            {
                int spark = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Electric,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    0, Color.White, Main.rand.NextFloat(0.8f, 1.5f));
                Main.dust[spark].noGravity = true;
            }

            // Fire/ember cores — gives it the scorching undertone
            if (Main.rand.NextBool(3))
            {
                int fire = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Torch,
                    0f, -Main.rand.NextFloat(0.5f, 1.5f),
                    0, default, Main.rand.NextFloat(0.7f, 1.3f));
                Main.dust[fire].noGravity = true;
            }

            // Glowing aura rings (RainbowMk2 or similar large glow dust)
            if (Main.rand.NextBool(3))
            {
                int glow = Dust.NewDust(
                    Projectile.Center - new Vector2(10f), 20, 20,
                    DustID.BlueFairy,
                    0f, 0f,
                    0, Color.CornflowerBlue, Main.rand.NextFloat(1.4f, 2.5f));
                Main.dust[glow].noGravity = true;
                Main.dust[glow].velocity = Vector2.Zero;
            }

            // Spin sprite for visual flair
            Projectile.rotation += 0.4f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.Next(0, 101) < BurnChance)
                target.AddBuff(BuffID.OnFire, BurnDuration);

            // Impact burst — extra splash particles on hit
            for (int i = 0; i < 8; i++)
            {
                int splash = Dust.NewDust(
                    target.Center, target.width, target.height,
                    DustID.Water,
                    Main.rand.NextFloat(-4f, 4f),
                    Main.rand.NextFloat(-4f, 4f),
                    0, Color.DeepSkyBlue, Main.rand.NextFloat(1.5f, 2.5f));
                Main.dust[splash].noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            // Dissipation splash when the beam ends / hits a wall
            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Water,
                    Main.rand.NextFloat(-5f, 5f),
                    Main.rand.NextFloat(-5f, 5f),
                    0, Color.SkyBlue, Main.rand.NextFloat(1.2f, 2.0f));
                Main.dust[d].noGravity = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() / 2f;

            // ---- Afterimage trail —————————————————————————
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = 1f - i / (float)Projectile.oldPos.Length;
                // Outer glow: cyan → orange-red gradient along the trail
                Color trailColor = Color.Lerp(Color.DeepSkyBlue, Color.OrangeRed, 1f - progress)
                                       * progress * Alpha * 0.7f;
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition
                                  + new Vector2(Projectile.width / 2f, Projectile.height / 2f);

                // Scale up the trail slightly for a fatter beam look
                float trailScale = Projectile.scale * MathHelper.Lerp(1.8f, 0.4f, i / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(tex, drawPos, null, trailColor, Projectile.rotation,
                    origin, trailScale, SpriteEffects.None, 0);
            }

            // ---- Core glow layer (additive-style double draw) ————————————
            Color glowColor = Color.Lerp(Color.White, Color.DeepSkyBlue, 0.35f) * Alpha * 0.55f;
            Vector2 mainPos = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(tex, mainPos, null, glowColor, Projectile.rotation,
                origin, Projectile.scale * 2.2f, SpriteEffects.None, 0);

            // ---- Main sprite ————————————————————————————————
            Color mainColor = Color.Lerp(Color.White, Color.OrangeRed, 0.3f) * Alpha;
            Main.EntitySpriteDraw(tex, mainPos, null, mainColor, Projectile.rotation,
                origin, Projectile.scale * 1.1f, SpriteEffects.None, 0);

            return false;
        }
    }
}