using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    /// <summary>
    /// Visual-only streak left behind by the Iai Slash.
    /// Damage and teleport are handled entirely in the stand's AI so we have
    /// full control over the line-segment hit-test.  This projectile just draws
    /// the slash trail and plays particles.
    /// 
    /// ai[0]  = world X of the start point
    /// ai[1]  = world Y of the start point
    /// The end point is Projectile.Center (set at spawn).
    /// </summary>
    public class HolyDiverIaiSlashVisual : ModProjectile
    {
        private const int Lifetime = 22;

        // Cached from ai[] so we only read once
        private Vector2 SlashStart => new Vector2(Projectile.ai[0], Projectile.ai[1]);
        private Vector2 SlashEnd => Projectile.Center;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            float progress = 1f - (float)Projectile.timeLeft / Lifetime;

            int count = Projectile.timeLeft > Lifetime / 2 ? 4 : 2;
            for (int i = 0; i < count; i++)
            {
                float t = Main.rand.NextFloat(0f, 1f);
                Vector2 pos = Vector2.Lerp(SlashStart, SlashEnd, t);

                int d = Dust.NewDust(pos, 2, 2, DustID.Water,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    80, Color.White, Main.rand.NextFloat(1.2f, 2.0f));
                Main.dust[d].noGravity = true;

                if (Projectile.timeLeft > Lifetime * 0.7f && Main.rand.NextBool(2))
                {
                    int sp = Dust.NewDust(pos, 2, 2, DustID.Electric,
                        Main.rand.NextFloat(-3f, 3f),
                        Main.rand.NextFloat(-3f, 3f),
                        0, Color.OrangeRed, 1.4f);
                    Main.dust[sp].noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float alpha = (float)Projectile.timeLeft / Lifetime;

            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;

            Vector2 start = SlashStart - Main.screenPosition;
            Vector2 end = SlashEnd - Main.screenPosition;
            Vector2 dir = end - start;
            float length = dir.Length();
            if (length < 1f) return false;

            Vector2 unit = dir / length;
            float rot = unit.ToRotation();

            DrawSlashLayer(pixel, start, length, rot, Color.White * alpha * 0.90f, 4f);
            DrawSlashLayer(pixel, start, length, rot, Color.DeepSkyBlue * alpha * 0.60f, 10f);
            DrawSlashLayer(pixel, start, length, rot, Color.OrangeRed * alpha * 0.35f, 18f);

            return false;
        }

        private void DrawSlashLayer(Texture2D pixel, Vector2 start, float length, float rot,
                                    Color color, float halfThickness)
        {
            Main.EntitySpriteDraw(
                pixel,
                start,
                new Rectangle(0, 0, 1, 1),
                color,
                rot,
                new Vector2(0f, 0.5f),
                new Vector2(length, halfThickness * 2f),
                SpriteEffects.None,
                0);
        }
    }
}