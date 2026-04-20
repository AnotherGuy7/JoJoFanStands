using JoJoFanStands.Buffs;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Buffs.AccessoryBuff
{
    public class ViralBloomBuff : JoJoBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Living ViralBloom Alloy");
            // Description.SetDefault("The Uelibloom in your armor has fused with the viral virus, and now it hardens whenever you receive a hit, like a being dedicated to protect you");
            Main.buffNoTimeDisplay[Type] = true;
        }


        public override void UpdateBuffOnPlayer(Player player)
        {
            player.defense += 20
        }
    }
}