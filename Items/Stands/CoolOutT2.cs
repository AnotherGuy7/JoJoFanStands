using JoJoStands;
using JoJoStands.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class CoolOutT2 : FanStandItemClass
    {
        public override int standSpeed => 35;
        public override int standType => 2;
        public override string standProjectileName => "CoolOut";
        public override int standTier => 2;
        public override bool fanStandItem => true;

        public override string Texture
        {
            get { return mod.Name + "/Items/Stands/CoolOutT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Tier 2)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and hold right-click to charge up a spear!\nSpecial: Send out an ice wave!\nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            item.damage = 24;
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

        public override bool ManualStandSpawning(Player player)
        {
            player.GetModPlayer<MyPlayer>().standDefenseToAdd = 3;
            return false;
        }

        public override void AddRecipes()
        {
            Mod JoJoStands = ModLoader.GetMod("JoJoStands");
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<CoolOutT1>());
            recipe.AddIngredient(ItemID.DemoniteOre, 20);
            recipe.AddIngredient(ItemID.Shiverthorn, 5);
            recipe.AddIngredient(JoJoStands.ItemType("WillToProtect"));
            recipe.AddIngredient(JoJoStands.ItemType("WillToControl"));
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<CoolOutT1>());
            recipe.AddIngredient(ItemID.CrimtaneOre, 20);
            recipe.AddIngredient(ItemID.Shiverthorn, 5);
            recipe.AddIngredient(JoJoStands.ItemType("WillToProtect"));
            recipe.AddIngredient(JoJoStands.ItemType("WillToControl"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}