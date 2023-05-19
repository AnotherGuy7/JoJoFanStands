using JoJoStands;
using JoJoStands.Buffs;
using JoJoStands.Buffs.Debuffs;
using Terraria;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Buffs
{
    public class LightningFastReflex : JoJoBuff
    {
        public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Brian Eno Active");
            // Description.SetDefault("Brian Eno is active!");
        }

        public override void UpdateBuffOnPlayer(Player player)
        {
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            fPlayer.blurLightningFastReflexes = true;
        }

        public override void OnBuffEnd(Player player)
        {
            float reduction = 7.5f * (player.GetModPlayer<MyPlayer>().standTier - 2);
            player.AddBuff(ModContent.BuffType<AbilityCooldown>(), player.GetModPlayer<MyPlayer>().AbilityCooldownTime(30 - (int)reduction));
        }
    }
}