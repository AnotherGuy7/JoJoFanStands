using JoJoFanStands.Buffs;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Armor
{
    public class BrianEnoAct1 : StandItemClass
    {
        public override int standType => 2;
        public override int standTier => 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brian Eno (Act 1, The True Wheel)");
            Tooltip.SetDefault("While using any mount gain +10% movement speed, +5% chance to dodge attacks, and a +10% critical strike chance!\nPress special while in a mount to move faster and dodge all attacks!\nPress special while there is no mount to have Brain Eno carry you in a box!");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 48;
            item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            fPlayer.BrianEnoAct1 = true;
            player.AddBuff(BuffType<BrianEnoActiveBuff>(), 2);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<StandArrow>());
            recipe.AddIngredient(ItemType<SunDroplet>(), 10);
            recipe.AddIngredient(ItemID.CopperBar, 15);
            //recipe.AddIngredient(ItemType<WillToEscape>());
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<StandArrow>());
            recipe.AddIngredient(ItemType<SunDroplet>(), 10);
            recipe.AddIngredient(ItemID.TinBar, 15);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}