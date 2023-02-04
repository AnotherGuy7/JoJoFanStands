using JoJoStands;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class ExpansesT2 : FanStandItemClass
    {
        public override int standSpeed => 10;
        public override int standType => 2;
        public override string standProjectileName => "Expanses";
        public override int standTier => 2;
        public override bool FanStandItem => true;

        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/ExpansesT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Expanses (Tier 2)");
            Tooltip.SetDefault("Left-click to shoot sharp, glass-like crystals and right-click to launch a 2 piercing columns. \nUser Name: Prime Emperor \nReference: Expanses, album by The Green Kingdom");
        }

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.width = 26;
            Item.height = 50;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = 3;
        }

        public override bool ManualStandSpawning(Player player)
        {
            player.GetModPlayer<MyPlayer>().standDefenseToAdd = 3;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ExpansesT1>())
                .AddIngredient(75, 3)
                .AddIngredient(177, 7)
                .AddIngredient(147, 100)
                .AddIngredient(ModContent.ItemType<WillToEscape>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}