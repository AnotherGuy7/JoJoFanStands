using JoJoFanStands.Buffs;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Armor.StandArmors.ViralBloom
{
    [AutoloadEquip(EquipType.Head)]
    public class ViralBloomHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("ViralBloom Helmet");
            // Tooltip.SetDefault("A helmet made from the finest materials that has been offered by gods so far.");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.value = Item.buyPrice(gold: 17, silver: 50);
            Item.rare = ItemRarityID.Red;
            Item.defense = 32;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ViralBloomChestplate>() && legs.type == ModContent.ItemType<ViralBloomGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Your armor hardens whenever you receive a hit, giving you much more defense!";
            player.AddBuff(ModContent.BuffType<ViralBeetleBuff>(), 2);
        }

        public override void UpdateEquip(Player player)
        {
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            if (mPlayer.standType == 2)
            {
                Item.type = ModContent.ItemType<RequiemCrownLong>();
                Item.SetDefaults(ModContent.ItemType<RequiemCrownLong>());
            }
            if (mPlayer.standType == 1)
            {
                Item.type = ModContent.ItemType<RequiemCrownShort>();
                Item.SetDefaults(ModContent.ItemType<RequiemCrownShort>());
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 17)
                .AddIngredient(ItemID.FragmentStardust, 6)
                .AddIngredient(ModContent.ItemType<ViralMeteoriteBar>(), 6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}