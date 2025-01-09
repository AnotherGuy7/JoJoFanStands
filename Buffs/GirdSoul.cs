using JoJoStands;
using JoJoStands.Buffs;
using JoJoStands.Buffs.Debuffs;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Buffs
{
    public class GirdSoul : JoJoBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Brian Eno Active");
            // Description.SetDefault("Brian Eno is active!");
        }

        public override void UpdateBuffOnPlayer(Player player)
        {
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            fPlayer.metempsychosisPoints = 100;
            player.statDefense += 200;
        }

        public override void OnBuffEnd(Player player)
        {
            player.GetModPlayer<FanPlayer>().metempsychosisPoints = 0;
            player.AddBuff(ModContent.BuffType<AbilityCooldown>(), player.GetModPlayer<MyPlayer>().AbilityCooldownTime(20));
        }
    }
}