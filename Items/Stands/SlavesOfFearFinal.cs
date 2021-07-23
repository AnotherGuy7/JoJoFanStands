using JoJoStands.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class SlavesOfFearFinal : FanStandItemClass
    {
        public override int standSpeed => 10;
        public override int standType => 1;
        public override string standProjectileName => "SlavesOfFear";
        public override int standTier => 4;
        public override bool fanStandItem => true;

        public override string Texture
        {
            get { return mod.Name + "/Items/Stands/SlavesOfFearT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slaves Of Fear (Final Tier)");
            Tooltip.SetDefault("Left-click to punch enemies at a really fast rate and right click to shunt into enemies!\nSpecial: Weld an enemy to tiles!\nUser Name: The Phantom One \nReference: SLAVES OF FEAR by HEALTH");
        }

        public override void SetDefaults()
        {
            item.damage = 98;
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
            recipe.AddIngredient(ItemID.AsphaltBlock, 10);
            recipe.AddIngredient(ItemID.HallowedBar, 10);
            recipe.AddIngredient(ItemID.TopHat);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
