using JoJoStands;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Buffs
{
    public class RiskyRewards : ModBuff
    {
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Risky Rewards");
            Description.SetDefault("Enemies now drop more loot in exchange for lower defense.");
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
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