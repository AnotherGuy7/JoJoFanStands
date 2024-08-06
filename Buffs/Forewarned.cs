using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Buffs
{
    public class Forewarned : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Forwarned");
            // Description.SetDefault("You've already seen everything in the next 3 seconds...");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HasBuff(Mod.Find<ModBuff>(Name).Type))
            {
                //forseeing stuff here
            }
            else
            {
                player.AddBuff(ModContent.BuffType<SoreEye>(), 1800);
            }
        }
    }
}