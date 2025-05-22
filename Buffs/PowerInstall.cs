using JoJoStands;
using JoJoStands.Buffs;
using JoJoStands.Buffs.Debuffs;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Buffs
{
    public class PowerInstall : JoJoBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Brian Eno Active");
            // Description.SetDefault("Brian Eno is active!");
            //Main.buffNoTimeDisplay[Type] = true;
        }

        public override void UpdateBuffOnPlayer(Player player)
        {
            player.statLifeMax2 = (int)(player.statLifeMax2 * 0.82);
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (!mPlayer.standOut)
                player.ClearBuff(Type);
        }

        public override void OnBuffEnd(Player player)
        {
            player.AddBuff(ModContent.BuffType<AbilityCooldown>(), 5 * 60 * 60);
        }
    }
}