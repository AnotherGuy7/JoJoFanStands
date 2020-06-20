using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using ReLogic.Graphics;

namespace JoJoFanStands.Projectiles
{
    public class IceGlobe : ModProjectile
    {
        public int globeMaxHealth = 500;
        public int globeHealth = 500;

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 320;
            projectile.aiStyle = 0;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.alpha = 125;
        }

        public override void AI()
        {
            if (globeHealth <= 0)
            {
                projectile.Kill();
            }
            for (int p = 0; p < Main.maxProjectiles; p++)
            {
                if (Main.projectile[p].hostile)
                {
                    globeHealth -= Main.projectile[p].damage;
                    Main.projectile[p].Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.velocity.X = 10f * -projectile.spriteDirection;
            damage = target.damage / 2;
            knockback = 5f;
            CombatText.NewText(new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height), Color.Orange, target.damage / 2);
            globeHealth -= target.damage;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.DrawString(Main.fontMouseText, globeHealth + "/" + globeMaxHealth, projectile.Center, Color.White);
        }
    }
}