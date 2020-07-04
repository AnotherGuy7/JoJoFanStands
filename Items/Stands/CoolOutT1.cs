using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using System.Collections.Generic;

namespace JoJoFanStands.Items.Stands
{
	public class CoolOutT1 : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Tier 1)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and right-click to shoot Ice Weapons! \nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            item.damage = 17;
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

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            MyPlayer mPlayer = Main.player[Main.myPlayer].GetModPlayer<MyPlayer>();
            TooltipLine tooltipAddition = new TooltipLine(mod, "Speed", "Shoot Speed: " + (12 - mPlayer.standSpeedBoosts));
            tooltips.Add(tooltipAddition);
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            mult *= (float)player.GetModPlayer<MyPlayer>().standDamageBoosts;
        }

        public override void AddRecipes()
        {
            Mod JoJoStands = JoJoFanStands.JoJoStandsMod;
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(JoJoStands.ItemType("StandArrow"));
            recipe.AddIngredient(ItemID.Shiverthorn, 7);
            recipe.AddIngredient(ItemID.IceBlock, 40);
            recipe.AddIngredient(ItemID.Snowball, 60);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}