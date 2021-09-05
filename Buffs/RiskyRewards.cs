using JoJoStands;
using Terraria;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Buffs
{
    public class RiskyRewards : ModBuff
    {
        public override void SetDefaults()
        {
			DisplayName.SetDefault("Risky Rewards");
            Description.SetDefault("Enemies now drop more loot in exchange for lower defense.");
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = false;
        }
 
        public override void Update(Player player, ref int buffIndex)
        {
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (player.GetModPlayer<MyPlayer>().standOut)
            {
                player.statDefense -= fPlayer.banksDefenseReduction;
                player.buffTime[buffIndex] = 2;
            }
        }
    }
}