using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class IceSpear : ModProjectile
    {
        public override void SetDefaults()
        {
            //projectile.CloneDefaults(ProjectileID.JavelinFriendly);
            projectile.height = 25;
            projectile.width = 25;
            projectile.timeLeft = 1800;
            projectile.friendly = true;
            projectile.scale = 0.5f;
            projectile.tileCollide = true;
            //drawOffsetX = -21;
            //drawOriginOffsetX = -21;
            //drawOriginOffsetY = -42;
            //projectile.alpha = 255;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)     //draw offsets move too much
        {
            Texture2D texture = mod.GetTexture("Projectiles/IceSpear");
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 1f)
            {
                projectile.velocity.Y += 0.15f;
                projectile.rotation = projectile.velocity.ToRotation() + 1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.Item28, projectile.position);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}