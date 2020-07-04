using Terraria;
using Terraria.ID;
using JoJoStands;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace JoJoFanStands.Items.Stands
{
	public class BackInBlack : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Back In Black");
            Tooltip.SetDefault("Left-click to shoot enemy-chasing shots and right-click to save a position.\nUser Name: AnotherGuy");
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
            TooltipLine tooltipAddition = new TooltipLine(mod, "Speed", "Shoot Speed: " + (40 - mPlayer.standSpeedBoosts));
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
            recipe.AddIngredient(ItemID.HallowedBar, 9);
            recipe.AddIngredient(ItemID.CursedFlame, 12);
            recipe.AddIngredient(ItemID.DemoniteBar, 15);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(JoJoStands.ItemType("StandArrow"));
            recipe.AddIngredient(ItemID.HallowedBar, 9);
            recipe.AddIngredient(ItemID.Ichor, 12);
            recipe.AddIngredient(ItemID.CrimtaneBar, 15);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}