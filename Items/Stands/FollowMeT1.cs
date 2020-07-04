using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace JoJoFanStands.Items.Stands
{
	public class FollowMeT1 : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Follow Me (Tier 1)");
            Tooltip.SetDefault("Left-click to wind-up a punch and Right-click to grab enemies!\nSpecial: Intangible\nUser Name: Agatha/Betty/Thabita/Hot Pants \nReference: ???");
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

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            MyPlayer mPlayer = Main.player[Main.myPlayer].GetModPlayer<MyPlayer>();
            TooltipLine tooltipAddition = new TooltipLine(mod, "Speed", "Punch Speed: " + (12 - mPlayer.standSpeedBoosts));
            tooltips.Add(tooltipAddition);
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            mult *= (float)player.GetModPlayer<MyPlayer>().standDamageBoosts;
        }

        public override void AddRecipes()
        {
            Mod JoJoStands = ModLoader.GetMod("JoJoStands");
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(JoJoStands.ItemType("StandArrow"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}