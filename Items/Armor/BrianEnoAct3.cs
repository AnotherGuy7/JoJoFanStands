using Terraria.ModLoader;
using Terraria.ID;
using JoJoStands.Items.CraftingMaterials;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Armor
{
    public class BrianEnoAct3 : ModItem
    {
        public override void SetStaticDefault()
        {
            DisplayName.SetDefault("Brian Eno (Act 3, Mother Whale Eyeless)");
            Tooltip.SetDefault("While using any mount gain +10% movement speed, +5% chance to dodge attacks, and a +10% critical strike chance!\nPress special while in a mount to move faster and dodge all attacks!\nPress special while there is no mount to have Brain Eno carry you!");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.accessory = true;
            item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemType<BrianEnoAct2>());
            recipe.AddIngredient(ItemID.PalladiumBar, 12);
            recipe.AddIngredient(ItemType<SunDroplet>(), 5);
            recipe.AddIngredient(ItemID.SoulofFlight, 20);
            recipe.AddIngredient(ItemType<WillToEscape>(), 2);
			recipe.SetResult(this);
			recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<BrianEnoAct2>());
            recipe.AddIngredient(ItemID.CobaltBar, 12);
            recipe.AddIngredient(ItemType<SunDroplet>(), 5);
            recipe.AddIngredient(ItemID.SoulofFlight, 20);
            recipe.AddIngredient(ItemType<WillToEscape>(), 2);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}