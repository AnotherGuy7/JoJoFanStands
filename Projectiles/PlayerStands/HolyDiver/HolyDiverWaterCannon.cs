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
        // How long the beam lingers before fading (in ticks)
        private const int BeamLifetime = 28;
        // Burn chance (out of 100)
        private const int BurnChance = 75;
        // Burn debuff duration (ticks)
        private const int BurnDuration = 300;

        // Alpha starts full-bright and fades out near death
        private float Alpha => MathHelper.Clamp((float)Projectile.timeLeft / BeamLifetime, 0f, 1f);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = BeamLifetime;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            // Water steam / mist trail
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Cloud,
                    Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f,
                    100, Color.SkyBlue, Main.rand.NextFloat(0.8f, 1.6f));
                Main.dust[dust].noGravity = true;
            }

            // Hot steam / ember sparks
            if (Main.rand.NextBool(3))
            {
                int fire = Dust.NewDust(
                    Projectile.position, Projectile.width, Projectile.height,
                    DustID.Torch,
                    0f, -Main.rand.NextFloat(1f, 2f),
                    0, default, Main.rand.NextFloat(0.6f, 1.2f));
                Main.dust[fire].noGravity = true;
            }

            // Spin sprite
            Projectile.rotation += 0.3f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.Next(0, 101) < BurnChance)
                target.AddBuff(BuffID.OnFire, BurnDuration);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            // Draw afterimage trail
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = 1f - i / (float)Projectile.oldPos.Length;
                Color trailColor = Color.Lerp(Color.DeepSkyBlue, Color.OrangeRed, 1f - progress)
                                       * progress * Alpha * 0.6f;
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                Main.EntitySpriteDraw(tex, drawPos, null, trailColor, Projectile.rotation,
                    tex.Size() / 2f, Projectile.scale * progress, SpriteEffects.None, 0);
            }

            // Draw main projectile
            Color mainColor = Color.Lerp(Color.White, Color.OrangeRed, 0.4f) * Alpha;
            Vector2 mainPos = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(tex, mainPos, null, mainColor, Projectile.rotation,
                tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}