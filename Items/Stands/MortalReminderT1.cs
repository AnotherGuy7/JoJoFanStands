using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class MortalReminderT1 : FanStandItemClass
    {
        public override int standSpeed => 17;
        public override int standType => 1;
        public override int standTier => 1;
        public override bool fanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mortal Reminder (Tier 1)");
            Tooltip.SetDefault("Left-click to punch enemies at a really fast rate!\nUser Name: Benney \nReference: Mortal Reminder by Pentakill");
        }

        public override void SetDefaults()
        {
            item.damage = 18;
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<StandArrow>());
            recipe.AddIngredient(ItemType<WillToChange>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
