using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using JoJoStands;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.Pets
{
    public class InTheShadowsPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 60;
            MyPlayer.stopImmune.Add(mod.ProjectileType(Name));
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FanPlayer modPlayer = player.GetModPlayer<FanPlayer>();
            projectile.frameCounter++;
            if (player.dead)
            {
                modPlayer.InTheShadowsPet = false;
            }
            if (modPlayer.InTheShadowsPet)
            {
                projectile.timeLeft = 2;
            }
            if (Main.dayTime)
            {
                projectile.alpha = 129;
            }
            if (!Main.dayTime)
            {
                projectile.alpha = 0;
            }
            Vector2 vector131 = player.Center;
            vector131.X -= (float)((12 + player.width / 2) * player.direction);
            vector131.Y -= 25f;
            projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
            projectile.velocity *= 0.8f;
            projectile.direction = (projectile.spriteDirection = player.direction);
        }
    }
}