using JoJoStands.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class SlavesOfFearT1 : FanStandItemClass
    {
        public override int standSpeed => 13;
        public override int standType => 1;
        public override string standProjectileName => "SlavesOfFear";
        public override int standTier => 1;
        public override bool fanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slaves Of Fear (Tier 1)");
            Tooltip.SetDefault("Left-click to punch enemies at a really fast rate!\nUser Name: The Phantom One \nReference: SLAVES OF FEAR by HEALTH");
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
            recipe.AddIngredient(JoJoFanStands.JoJoStandsMod.ItemType("StandArrow"));
            recipe.AddIngredient(JoJoFanStands.JoJoStandsMod.ItemType("WillToFight"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
