using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class BanksT3 : FanStandItemClass
    {
        public override int standSpeed => 8;           //7-8 shots/s
        public override int standType => 2;
        public override string standProjectileName => "Banks";
        public override int standTier => 3;
        public override bool fanStandItem => true;

        public override string Texture
        {
            get { return mod.Name + "/Items/Stands/BanksT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Banks (Tier 3)");
            Tooltip.SetDefault("Left-click shoot a random enemy and right-click to use a shotgun.\nSpecial: Enemies killed drop 1.5x the amount of coins. Defense reduced by 6.\nUser Name: Pauline \nReference: Banks by Lincoln");
        }

        public override void SetDefaults()
        {
            item.damage = 10;
            item.width = 30;
            item.height = 36;
            item.useTime = 12;
            item.useAnimation = 12;
            item.maxStack = 1;
            item.knockBack = 2f;
            item.value = 0;
            item.noUseGraphic = true;
            item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldCoin, 50);
            recipe.AddIngredient(ItemID.GoldBar, 20);
            recipe.AddIngredient(ItemID.MarbleBlock, 50);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddIngredient(ItemType<WillToChange>(), 2);
            recipe.AddIngredient(ItemType<WillToProtect>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}