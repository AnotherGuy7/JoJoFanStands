using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class FollowMeT1 : StandItemClass
    {
        public override int standSpeed => 12;
        public override int standType => 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Follow Me (Tier 1)");
            Tooltip.SetDefault("Left-click to wind-up a punch and Right-click to grab enemies!\nUser Name: Agatha/Betty/Thabita/Mrs Destiny/Hot Pants \nReference: ???");
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
            Mod JoJoStands = JoJoFanStands.JoJoStandsMod;
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<StandArrow>());
            recipe.AddIngredient(ItemType<WillToChange>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}