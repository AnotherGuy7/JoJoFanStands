using JoJoStands.Items;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class FollowMeT2 : StandItemClass
    {
        public override string Texture => mod.Name + "/Items/Stands/FollowMeT1";
        public override int standSpeed => 12;
        public override int standType => 1;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Follow Me (Tier 2)");
            Tooltip.SetDefault("Left-click to wind-up a punch and Right-click to grab enemies!\nSpecial: Intangible\nUser Name: Agatha/Betty/Thabita/Mrs Destiny/Hot Pants \nReference: ???");
        }

        public override void SetDefaults()
        {
            item.damage = 28;
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
            recipe.AddIngredient(ItemType<FollowMeT1>());
            recipe.AddIngredient(ItemID.Amethyst, 4);
            recipe.AddIngredient(ItemID.SharkToothNecklace);
            recipe.AddIngredient(JoJoStands.ItemType("WillToChange"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}