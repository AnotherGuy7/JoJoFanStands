using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using JoJoStands;
using Terraria.ModLoader;
using System;

namespace JoJoFanStands.Projectiles.Minions
{
    public class RoseColoredSun : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.aiStyle = 0;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 80;
            projectile.friendly = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FanPlayer modPlayer = player.GetModPlayer<FanPlayer>();
            if (modPlayer.RoseColoredSunActive)
            {
                projectile.timeLeft = 2;
            }

            projectile.position.Y = player.position.Y - 240f;
            projectile.position.X = player.position.X + 5f;
            Lighting.AddLight(projectile.position, 255f, 84f, 161f);
            projectile.rotation += (float)projectile.direction * 0.01f;

            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC npc = Main.npc[n];
                if (npc.active && projectile.Distance(npc.position) <= 30f * 16f)
                {
                    npc.AddBuff(BuffID.Confused, 300);
                    npc.AddBuff(BuffID.OnFire, 300);
                }
            }
        }
    }
}