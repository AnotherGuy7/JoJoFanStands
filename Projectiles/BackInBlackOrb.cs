using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class BackInBlackOrb : ModProjectile
    {
        private float vacuumStrength = 0.09f;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 15 * 60;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.alpha = 0;
        }


        public override void AI()
        {
            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC npc = Main.npc[n];
                if (npc.active && !npc.dontTakeDamage && !npc.immortal && Vector2.Distance(Projectile.Center, Main.npc[n].Center) < 4 * 16)
                {
                    Vector2 positionDifference = Projectile.Center - Main.npc[n].Center;
                    positionDifference.Normalize();
                    npc.velocity += positionDifference * vacuumStrength;

                }
            }
        }
    }
}