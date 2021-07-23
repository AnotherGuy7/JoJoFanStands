using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class BanksFinal : FanStandItemClass
    {
        public override int standSpeed => 6;           //9-10 shots/s
        public override int standType => 2;
        public override string standProjectileName => "Banks";
        public override int standTier => 4;
        public override bool fanStandItem => true;

        public override string Texture
        {
            get { return mod.Name + "/Items/Stands/BanksT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Banks (Final Tier)");
            Tooltip.SetDefault("Left-click shoot a random enemy and right-click to use a shotgun.\nSpecial: Enemies killed drop 2.0x the amount of coins. Defense reduced by 10.\nUser Name: Pauline \nReference: Banks by Lincoln");
        }

        public override void SetDefaults()
        {
            item.damage = 17;
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
            recipe.AddIngredient(ItemID.GoldBar, 50);
            recipe.AddIngredient(ItemID.Ruby, 10);
            recipe.AddIngredient(ItemID.Ectoplasm, 30);
            recipe.AddIngredient(ItemID.SoulofLight, 30);
            recipe.AddIngredient(ItemID.SoulofNight, 50);
            recipe.AddIngredient(ItemType<WillToChange>(), 2);
            recipe.AddIngredient(ItemType<WillToProtect>(), 2);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}