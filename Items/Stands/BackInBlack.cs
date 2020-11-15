using JoJoStands.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class BackInBlack : StandItemClass
    {
        public override int standSpeed => 40;
        public override int standType => 2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Back In Black");
            Tooltip.SetDefault("Left-click to shoot enemy-chasing shots and right-click to save a position.\nUser Name: AnotherGuy");
        }

        public override void SetDefaults()
        {
            item.damage = 16;
            item.width = 32;
            item.height = 32;
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
            Mod JoJoStands = ModLoader.GetMod("JoJoStands");
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(JoJoStands.ItemType("StandArrow"));
            recipe.AddIngredient(ItemID.HallowedBar, 9);
            recipe.AddIngredient(ItemID.CursedFlame, 12);
            recipe.AddIngredient(ItemID.DemoniteBar, 15);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(JoJoStands.ItemType("StandArrow"));
            recipe.AddIngredient(ItemID.HallowedBar, 9);
            recipe.AddIngredient(ItemID.Ichor, 12);
            recipe.AddIngredient(ItemID.CrimtaneBar, 15);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}