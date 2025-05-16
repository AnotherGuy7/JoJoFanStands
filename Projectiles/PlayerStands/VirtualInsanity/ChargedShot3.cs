using JoJoFanStands.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity
{
    public class ChargedShot3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 188;
            Projectile.height = 120;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 7)
                    Projectile.frame = 0;
            }
            if (Projectile.timeLeft % 30 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(32f, 0f), Projectile.velocity, ModContent.ProjectileType<Plasma>(), Projectile.damage / 2, Projectile.knockBack / 2f, Projectile.owner);

            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3() * 3f);
            int amountOfDust = Main.rand.Next(2, 6 + 1);
            for (int i = 0; i < amountOfDust; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ChargedShotTrail>(), Projectile.velocity.X, Projectile.velocity.Y / 2f);

            Point16 projectileCenterPoint = Projectile.Center.ToTileCoordinates16();
            if (projectileCenterPoint.X >= 0 && projectileCenterPoint.X < Main.maxTilesX && projectileCenterPoint.Y >= 0 && projectileCenterPoint.Y < Main.maxTilesY)
            {
                if (Main.tile[projectileCenterPoint.X, projectileCenterPoint.Y].HasTile)
                {
                    Projectile.scale -= 0.05f;
                    if (Projectile.scale <= 0f)
                        Projectile.Kill();
                }
            }
            else
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.scale -= 0.05f;
            if (Projectile.scale <= 0f)
                Projectile.Kill();
        }
    }
}