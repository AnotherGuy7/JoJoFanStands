using JoJoStands;
using JoJoStands.Buffs;
using JoJoStands.Buffs.Debuffs;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Buffs
{
    public class InfiniteVelocity : JoJoBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Brian Eno Active");
            // Description.SetDefault("Brian Eno is active!");
        }

        public override void UpdateBuffOnPlayer(Player player)
        {
            player.armorEffectDrawShadow = true;
            player.GetModPlayer<FanPlayer>().blurInfiniteVelocity = true;
        }

        public override void OnBuffEnd(Player player)
        {
            player.AddBuff(ModContent.BuffType<AbilityCooldown>(), player.GetModPlayer<MyPlayer>().AbilityCooldownTime(30));
        }
    }
}