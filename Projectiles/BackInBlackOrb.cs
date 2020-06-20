using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class BackInBlackOrb : ModProjectile
    {
        public float vacuumStrength = 1.5f;
        public float npcDistance = 0f;

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 0;
            projectile.timeLeft = 1500;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 3;
            projectile.alpha = 0;
        }


        public override void AI()
        {
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC npc = Main.npc[k];
                npcDistance = Vector2.Distance(projectile.Center, Main.npc[k].Center);
                if (npc.active && !npc.dontTakeDamage && !npc.immortal && npcDistance < 31f)
                {
                    if (npc.position.X < projectile.position.X)
                    {
                        npc.velocity.X += vacuumStrength;
                    }
                    if (npc.position.X > projectile.position.X)
                    {
                        npc.velocity.X -= vacuumStrength;
                    }
                    if (npc.position.Y < projectile.position.Y)
                    {
                        npc.velocity.Y += vacuumStrength;
                    }
                    if (npc.position.Y > projectile.position.Y)
                    {
                        npc.velocity.Y -= vacuumStrength;
                    }
                }
            }
        }
    }
}