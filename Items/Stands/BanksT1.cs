using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class BanksT1 : FanStandItemClass
    {
        public override int StandSpeed => 12;           //5 shots/s
        public override int StandType => 2;
        public override string StandProjectileName => "Banks";
        public override int StandTier => 1;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Banks (Tier 1)");
            Tooltip.SetDefault("Left-click shoot a random enemy.\nUser Name: Pauline \nReference: Banks by Lincoln");
        }

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.width = 30;
            Item.height = 36;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            player.GetModPlayer<FanPlayer>().SpawnFanStand();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<StandArrow>())
                .AddIngredient(ItemType<WillToChange>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}