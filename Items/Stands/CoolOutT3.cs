using JoJoStands;
using JoJoStands.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class CoolOutT3 : StandItemClass
    {
        public override int standSpeed => 30;
        public override int standType => 2;

        public override string Texture
        {
            get { return mod.Name + "/Items/Stands/CoolOutT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Tier 3)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and press right-click to use the selected ability!\nSpecial: Cycle throgh abilities!\nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            item.damage = 39;
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
            recipe.AddIngredient(ItemType<CoolOutT2>());
            recipe.AddIngredient(ItemID.DemoniteBar, 6);
            recipe.AddIngredient(ItemID.Bone, 20);
            recipe.AddIngredient(JoJoStands.ItemType("WillToProtect"), 2);
            recipe.AddIngredient(JoJoStands.ItemType("WillToControl"));
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<CoolOutT2>());
            recipe.AddIngredient(ItemID.CrimtaneBar, 6);
            recipe.AddIngredient(ItemID.Bone, 20);
            recipe.AddIngredient(JoJoStands.ItemType("WillToProtect"), 2);
            recipe.AddIngredient(JoJoStands.ItemType("WillToControl"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}