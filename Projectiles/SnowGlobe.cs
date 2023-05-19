using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
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
            Projectile.width = Projectile.height = 320;
            Projectile.aiStyle = 0;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 125;
        }

        public override void AI()
        {
            globeMaxHealth = (int)Projectile.ai[1];
            if (!setHealth)
            {
                globeHealth = globeMaxHealth;
                setHealth = true;
            }
            if (globeHealth <= 0 || !Main.projectile[(int)Projectile.ai[0]].active)
            {
                Projectile.Kill();
            }
            float circleRadius = Projectile.width / 2;
            for (int p = 0; p < Main.maxProjectiles; p++)
            {
                Projectile hostileProj = Main.projectile[p];
                if (hostileProj.active && hostileProj.hostile && Projectile.Distance(hostileProj.Center) <= circleRadius)
                {
                    globeHealth -= hostileProj.damage;
                    hostileProj.Kill();
                }
            }
            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC npc = Main.npc[n];
                if (npc.active && !npc.immortal && !npc.townNPC && npc.lifeMax > 5 && Projectile.Distance(npc.Center) <= circleRadius)
                {
                    globeHealth -= npc.damage;
                    npc.velocity.X = 10f * -Projectile.spriteDirection;
                    NPC.HitInfo hitInfo = new NPC.HitInfo()
                    {
                        Damage = npc.damage / 2,
                        Knockback = 5f,
                        HitDirection = -npc.direction
                        };
                    npc.StrikeNPC(hitInfo);
                    npc.AddBuff(BuffID.Chilled, 180);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, globeHealth + "/" + globeMaxHealth, Projectile.Center - Main.screenPosition - new Vector2(5f, 5f), Color.White);
        }
    }
}