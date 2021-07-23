using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Projectiles
{
    public class Fists : ModProjectile
    {
        public override void SetDefaults()      //0 = SP; 1 = TW; 2 = GE; 3 = GER; 4 = SF's; 5 = KQ (Stand); 6 = KC; 7 = TH; 8 = GD;
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 4;
            projectile.alpha = 255;     //completely transparent
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (Main.rand.NextFloat(0, 101) <= mPlayer.standCritChangeBoosts)
            {
                crit = true;
            }

            if (mPlayer.destroyAmuletEquipped)
            {
                if (Main.rand.NextFloat(0, 101) >= 93)
                {
                    target.AddBuff(BuffID.OnFire, 60 * 3);
                }
            }
            if (mPlayer.greaterDestroyEquipped)
            {
                if (Main.rand.NextFloat(0, 101) >= 80)
                {
                    target.AddBuff(BuffID.CursedInferno, 60 * 10);
                }
            }
            if (mPlayer.crackedPearlEquipped)
            {
                if (Main.rand.NextFloat(0, 101) >= 60)
                {
                    target.AddBuff(mod.BuffType("Infected"), 10 * 60);
                }
            }
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (Main.rand.NextFloat(0, 101) <= mPlayer.standCritChangeBoosts)
            {
                crit = true;
            }

            if (mPlayer.destroyAmuletEquipped)
            {
                if (Main.rand.NextFloat(0, 101) >= 93)
                {
                    target.AddBuff(BuffID.OnFire, 3 * 60);
                }
            }
            if (mPlayer.greaterDestroyEquipped)
            {
                if (Main.rand.NextFloat(0, 101) >= 80)
                {
                    target.AddBuff(BuffID.CursedInferno, 10 * 60);
                }
            }
            if (mPlayer.crackedPearlEquipped)
            {
                if (Main.rand.NextFloat(0, 101) >= 60)
                {
                    target.AddBuff(mod.BuffType("Infected"), 10 * 60);
                }
            }
        }
    }
}