using JoJoStands;
using JoJoStands.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class CoolOutT1 : FanStandItemClass
    {
        public override int standSpeed => 40;
        public override int standType => 2;
        public override string standProjectileName => "CoolOut";
        public override int standTier => 1;
        public override bool fanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Tier 1)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and right-click to charge and throw an Ice Spear!\nSpecial: Summon an infinite Ice Wave!\nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            item.damage = 15;
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
            Mod JoJoStands = JoJoFanStands.JoJoStandsMod;
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(JoJoStands.ItemType("StandArrow"));
            recipe.AddIngredient(ItemID.IceBlock, 15);
            recipe.AddIngredient(ItemID.Shiverthorn, 3);
            recipe.AddIngredient(JoJoStands.ItemType("WillToProtect"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}