using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using ReLogic.Graphics;

namespace JoJoFanStands.Projectiles
{
    public class SnowGlobe : ModProjectile
    {
        private int globeMaxHealth = 500;
        private int globeHealth = 500;
        private bool setHealth = false;

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
            globeMaxHealth = (int)projectile.ai[1];
            if (!setHealth)
            {
                globeHealth = globeMaxHealth;
                setHealth = true;
            }
            if (globeHealth <= 0 || !Main.projectile[(int)projectile.ai[0]].active)
            {
                projectile.Kill();
            }
            float circleRadius = projectile.width / 2;
            for (int p = 0; p < Main.maxProjectiles; p++)
            {
                Projectile hostileProj = Main.projectile[p];
                if (hostileProj.active && hostileProj.hostile && projectile.Distance(hostileProj.Center) <= circleRadius)
                {
                    globeHealth -= hostileProj.damage;
                    hostileProj.Kill();
                }
            }
            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC npc = Main.npc[n];
                if (npc.active && !npc.immortal && !npc.townNPC && npc.lifeMax > 5 && projectile.Distance(npc.Center) <= circleRadius)
                {
                    globeHealth -= npc.damage;
                    npc.velocity.X = 10f * -projectile.spriteDirection;
                    npc.StrikeNPC(npc.damage / 2, 5f, -npc.direction);
                    npc.AddBuff(BuffID.Chilled, 180);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.DrawString(Main.fontMouseText, globeHealth + "/" + globeMaxHealth, projectile.Center - Main.screenPosition - new Vector2(5f, 5f), Color.White);
        }
    }
}