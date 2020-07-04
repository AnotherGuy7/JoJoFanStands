using System;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using JoJoStands;
using System.Collections.Generic;

namespace JoJoFanStands.Items.Stands
{
	public class SlavesOfFearT1 : ModItem
	{
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

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            MyPlayer mPlayer = Main.player[Main.myPlayer].GetModPlayer<MyPlayer>();
            TooltipLine tooltipAddition = new TooltipLine(mod, "Speed", "Punch Speed: " + (13 - mPlayer.standSpeedBoosts));
            tooltips.Add(tooltipAddition);
        }

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
        {
            mult *= (float)player.GetModPlayer<MyPlayer>().standDamageBoosts;
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
