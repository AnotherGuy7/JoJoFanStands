using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.Minions
{
    public class RoseColoredSun : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 80;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FanPlayer modPlayer = player.GetModPlayer<FanPlayer>();
            if (modPlayer.RoseColoredSunActive)
                Projectile.timeLeft = 2;

            Projectile.position.Y = player.position.Y - 240f;
            Projectile.position.X = player.position.X + 5f;
            Projectile.rotation += (float)Projectile.direction * 0.01f;
            Lighting.AddLight(Projectile.position, 255f, 84f, 161f);

            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC npc = Main.npc[n];
                if (npc.active && Projectile.Distance(npc.position) <= 30f * 16f)
                {
                    npc.AddBuff(BuffID.Confused, 300);
                    npc.AddBuff(BuffID.OnFire, 300);
                }
            }
        }
    }
}