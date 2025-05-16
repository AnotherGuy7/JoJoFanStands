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
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void UpdateBuffOnPlayer(Player player)
        {
            player.statLifeMax2 = (int)(player.statLifeMax2 * 0.92);
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (!mPlayer.standOut)
                player.ClearBuff(Type);
        }

        public override void OnBuffEnd(Player player)
        {
            float powerInstallCompletion = ((2f * 60f * 60f) - player.buffTime[player.FindBuffIndex(Type)]) / (2f * 60f * 60f);
            player.AddBuff(ModContent.BuffType<AbilityCooldown>(), (int)(5 * 60 * 60 * powerInstallCompletion));
        }
    }
}