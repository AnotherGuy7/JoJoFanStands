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
            projectile.width = 64;
            projectile.height = 64;
            projectile.aiStyle = 0;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.alpha = 80;
            projectile.ownerHitCheck = true;
            projectile.friendly = true;
            MyPlayer.stopImmune.Add(mod.ProjectileType(Name));
        }

        public float NPCDistances = 0f;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FanPlayer modPlayer = player.GetModPlayer<FanPlayer>();
            projectile.position.Y = player.position.Y - 240f;
            projectile.position.X = player.position.X + 5f;
            Lighting.AddLight(projectile.position, 255f, 84f, 161f);
            projectile.rotation += (float)projectile.direction * 0.1f;
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                if (Main.npc[k].active)
                {
                    NPCDistances = Vector2.Distance(projectile.position, Main.npc[k].position);
                }
                if (Main.npc[k].active && NPCDistances <= 1470f)
                {
                    Main.npc[k].AddBuff(BuffID.Confused, 300);
                    Main.npc[k].AddBuff(BuffID.OnFire, 300);
                }
            }
            if (modPlayer.RoseColoredSunActive)
            {
                projectile.timeLeft = 2;
            }
        }
    }
}