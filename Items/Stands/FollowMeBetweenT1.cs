using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;
using JoJoStands.Items;

namespace JoJoFanStands.Items.Stands
{
	public class FollowMeBetweenT1 : StandItemClass
	{
        public override string Texture => mod.Name + "/Items/Stands/FollowMeT1";
        public override int standSpeed => 12;
        public override int standType => 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Follow Me: Between (Tier 1)");
            Tooltip.SetDefault("Left-click to wind-up a punch and Right-click to grab enemies!\nSpecial: Intangible\nUser Name: Agatha/Betty/Thabita/Mrs Destiny/Hot Pants \nReference: ???");
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
            recipe.AddIngredient(ItemType<FollowMeT2>());
            recipe.AddIngredient(ItemID.SoulofFright, 16);
            recipe.AddIngredient(JoJoStands.ItemType("StoneMask"));
            recipe.AddIngredient(ItemID.BrokenBatWing);
            recipe.AddIngredient(JoJoStands.ItemType("WillToChange"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}