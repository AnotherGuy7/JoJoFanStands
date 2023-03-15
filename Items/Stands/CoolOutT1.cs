using JoJoStands;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class CoolOutT1 : FanStandItemClass
    {
        public override int StandSpeed => 40;
        public override int StandType => 2;
        public override string StandProjectileName => "CoolOut";
        public override int StandTier => 1;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Tier 1)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and right-click to charge and throw an Ice Spear!\nSpecial: Summon an infinite Ice Wave!\nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.width = 36;
            Item.height = 42;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            player.GetModPlayer<MyPlayer>().standDefenseToAdd = 3;
            player.GetModPlayer<FanPlayer>().SpawnFanStand();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<StandArrow>())
                .AddIngredient(ItemID.IceBlock, 15)
                .AddIngredient(ItemID.Shiverthorn, 3)
                .AddIngredient(ModContent.ItemType<WillToProtect>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}