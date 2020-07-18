using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
 
namespace JoJoFanStands.Buffs
{
    public class BrianEnoActiveBuff : ModBuff
    {
        public override void SetDefaults()
        {
			DisplayName.SetDefault("Brian Eno Active");
            Description.SetDefault("Brian Eno is active!");
            Main.buffNoTimeDisplay[Type] = true;
        }
 
        public override void Update(Player player, ref int buffIndex)
        {
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (fPlayer.anyBrianEno)
            {
                player.AddBuff(BuffType<BrianEnoActiveBuff>(), 2);
            }
        }
    }
}