using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class IceSpear : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.height = 25;
            Projectile.width = 25;
            Projectile.timeLeft = 1800;
            Projectile.friendly = true;
            Projectile.scale = 0.5f;
            Projectile.tileCollide = true;
        }

        private Texture2D iceSpearTexture;
        private Vector2 iceSpearOrigin;

        public override bool PreDraw(ref Color lightColor)     //draw offsets move too much
        {
            if (iceSpearTexture == null)
            {
                iceSpearTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/IceSpear", AssetRequestMode.ImmediateLoad).Value;
                iceSpearOrigin = new Vector2(iceSpearTexture.Width / 2f, iceSpearTexture.Height / 2f);
            }

            Main.EntitySpriteDraw(iceSpearTexture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, iceSpearOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1f)
            {
                Projectile.velocity.Y += 0.15f;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}