using JoJoStands;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace JoJoFanStands.Items.Stands
{
    public class ExpansesT1 : FanStandItemClass
    {
        public override int StandSpeed => 12;
        public override int StandType => 2;
        public override string StandProjectileName => "Expanses";
        public override int StandTier => 1;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Expanses (Tier 1)");
            Tooltip.SetDefault("Left-click to shoot sharp, glass-like crystals and right-click to launch a 1 piercing column. \nUser Name: Prime Emperor \nReference: Expanses, album by The Green Kingdom");
        }

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.width = 26;
            Item.height = 50;
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
                .AddIngredient(ModContent.ItemType<WillToEscape>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}