using JoJoFanStands.Projectiles.PlayerStands.WaywardSon;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace JoJoFanStands.Projectiles
{
    public class FanStandFists : ModProjectile
    {
        public const int TheWorldOverHeavenFists = 0;
        public const int WaywardSonFists = 1;

        private int standType = 0;
        private int standTier = 0;
        public ModProjectile standInstance;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 4;
            Projectile.alpha = 255;     //completely transparent
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            standType = mPlayer.standFistsType;
            standTier = mPlayer.standTier;

            if (standType == WaywardSonFists)
            {
                if (standInstance != null && standTier == 3)
                    (standInstance as WaywardSonStandT3).standAbilities.AttackEffects(Projectile);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (Main.rand.NextFloat(0, 101) <= mPlayer.standCritChangeBoosts)
                modifiers.SetCrit();

            if (mPlayer.crackedPearlEquipped)
            {
                if (Main.rand.NextFloat(0, 101) >= 60)
                    target.AddBuff(ModContent.BuffType<Infected>(), 10 * 60);
            }

            if (standType == WaywardSonFists)
            {
                if (standInstance != null)
                {
                    if (standTier == 3)
                        (standInstance as WaywardSonStandT3).standAbilities.AttackModifiers(target, ref modifiers);
                    //else if (standTier == 4)
                        //(standInstance as WaywardSonStandFinal).standAbilities.AttackModifiers(target, ref modifiers);
                }
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (modifiers.PvP)
            {
                Player player = Main.player[Projectile.owner];
                MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
                if (mPlayer.crackedPearlEquipped && Main.rand.NextFloat(0, 101) >= 60)
                    target.AddBuff(ModContent.BuffType<Infected>(), 10 * 60);
            }
        }
    }
}