using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    /// <summary>
    /// The Hydro Symbiosis melee sword slash.
    /// ai[0] == 0 → standard swing
    /// ai[0] == 1 → charged heavy swing (larger, more damage via Projectile.damage set at spawn)
    /// </summary>
    public class HolyDiverSymbiosisSword : ModProjectile
    {
        private const int SwingLifetime = 18;
        private const int ChargedLifetime = 28;
        private const int BurnChance = 60;
        private const int BurnDuration = 360;

        private bool IsCharged => Projectile.ai[0] == 1;

        private float ArcRadius => IsCharged ? 80f : 55f;

        private float swingAngle;

        private float swingBaseAngle => Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = SwingLifetime;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ownerHitCheck = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            int lifespan = IsCharged ? ChargedLifetime : SwingLifetime;
            float progress = 1f - (float)Projectile.timeLeft / lifespan;
            float halfArc = IsCharged ? MathHelper.ToRadians(120f) : MathHelper.ToRadians(80f);
            float angleNow = swingBaseAngle + MathHelper.Lerp(-halfArc, halfArc, progress);
            swingAngle = angleNow;
            Vector2 arcPos = owner.Center + new Vector2(ArcRadius, 0f).RotatedBy(angleNow);
            Projectile.Center = arcPos;
            Projectile.rotation = angleNow + MathHelper.PiOver2;
            for (int i = 0; i < (IsCharged ? 3 : 2); i++)
            {
                Color dustColor = IsCharged ? Color.OrangeRed : Color.DeepSkyBlue;
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height,
                    DustID.Water,
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f),
                    80, dustColor, IsCharged ? 2.0f : 1.4f);
                Main.dust[d].noGravity = true;
            }
            if (IsCharged && Main.rand.NextBool(2))
            {
                int spark = Dust.NewDust(Projectile.Center, 4, 4, DustID.Electric,
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f),
                    0, Color.White, 1.2f);
                Main.dust[spark].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.Next(100) < BurnChance)
                target.AddBuff(BuffID.OnFire, BurnDuration);
            for (int i = 0; i < (IsCharged ? 14 : 8); i++)
            {
                int splash = Dust.NewDust(target.Center, target.width, target.height,
                    DustID.Water,
                    Main.rand.NextFloat(-5f, 5f),
                    Main.rand.NextFloat(-5f, 5f),
                    0, Color.Aquamarine, Main.rand.NextFloat(1.4f, 2.4f));
                Main.dust[splash].noGravity = false;
            }
            if (IsCharged)
            {
                for (int i = 0; i < 16; i++)
                {
                    float a = MathHelper.TwoPi / 16f * i;
                    Vector2 vel = new Vector2(5f, 0f).RotatedBy(a);
                    int d = Dust.NewDust(target.Center, 2, 2, DustID.BlueFairy,
                        vel.X, vel.Y, 0, Color.OrangeRed, 1.8f);
                    Main.dust[d].noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.Item14, target.Center);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                int d = Dust.NewDust(Projectile.Center, 4, 4, DustID.Water,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f),
                    0, Color.SkyBlue, 1.2f);
                Main.dust[d].noGravity = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() / 2f;
            float alpha = (float)Projectile.timeLeft /
                (IsCharged ? ChargedLifetime : SwingLifetime);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float trailFade = 1f - i / (float)Projectile.oldPos.Length;
                Color trailColor = (IsCharged
                    ? Color.Lerp(Color.OrangeRed, Color.DeepSkyBlue, trailFade)
                    : Color.Lerp(Color.DeepSkyBlue, Color.White, trailFade))
                    * trailFade * alpha * 0.6f;
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition
                                  + new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                float trailScale = MathHelper.Lerp(IsCharged ? 1.6f : 1.1f, 0.3f,
                    i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(tex, drawPos, null, trailColor,
                    Projectile.oldRot[i], origin, trailScale, SpriteEffects.None, 0);
            }
            Color glowColor = (IsCharged ? Color.OrangeRed : Color.DeepSkyBlue) * alpha * 0.5f;
            Vector2 mainPos = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(tex, mainPos, null, glowColor,
                Projectile.rotation, origin, IsCharged ? 2.4f : 1.8f, SpriteEffects.None, 0);
            Color mainColor = Color.Lerp(Color.White, IsCharged ? Color.OrangeRed : Color.DeepSkyBlue, 0.4f) * alpha;
            Main.EntitySpriteDraw(tex, mainPos, null, mainColor,
                Projectile.rotation, origin, IsCharged ? 1.6f : 1.1f, SpriteEffects.None, 0);
            return false;
        }
    }
}