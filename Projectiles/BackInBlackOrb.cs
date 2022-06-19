using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class BackInBlackOrb : ModProjectile
    {
        private float vacuumStrength = 1.5f;
        public float npcDistance = 0f;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 1500;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.alpha = 0;
        }


        public override void AI()
        {
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC npc = Main.npc[k];
                npcDistance = Vector2.Distance(Projectile.Center, Main.npc[k].Center);
                if (npc.active && !npc.dontTakeDamage && !npc.immortal && npcDistance < 31f)
                {
                    /*if (npc.position.X < Projectile.position.X)
                    {
                        npc.velocity.X += vacuumStrength;
                    }
                    if (npc.position.X > Projectile.position.X)
                    {
                        npc.velocity.X -= vacuumStrength;
                    }
                    if (npc.position.Y < Projectile.position.Y)
                    {
                        npc.velocity.Y += vacuumStrength;
                    }
                    if (npc.position.Y > Projectile.position.Y)
                    {
                        npc.velocity.Y -= vacuumStrength;
                    }*/
                    Vector2 positionDifference = Projectile.Center - Main.npc[k].Center;
                    positionDifference.Normalize();
                    npc.velocity = positionDifference * vacuumStrength;

                }
            }
        }
    }
}