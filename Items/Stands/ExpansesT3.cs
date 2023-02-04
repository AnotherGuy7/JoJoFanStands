using JoJoStands;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class ExpansesT3 : FanStandItemClass
    {
        public override int standSpeed => 7;
        public override int standType => 2;
        public override string standProjectileName => "Expanses";
        public override int standTier => 3;
        public override bool FanStandItem => true;

        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/ExpansesT1"; }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Expanses (Tier 3)");
            Tooltip.SetDefault("Left-click to shoot sharp, glass-like crystals and hold right to launch a 3 piercing columns.\nSpecial: Crystallize the space in front of you and get through it.\nUser Name: WarOn \nReference: Prime Emperor, album by The Green Kingdom");
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
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
                .AddIngredient(ModContent.ItemType<ExpansesT2>())
                .AddIngredient(502, 15)
                .AddIngredient(496, 1)
                .AddIngredient(1225, 8)
                .AddIngredient(ModContent.ItemType<WillToChange>(), 2)
                .AddIngredient(ModContent.ItemType<WillToEscape>(), 2)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}