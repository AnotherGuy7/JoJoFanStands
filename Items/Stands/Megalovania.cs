using Terraria;
using Terraria.ID;
using JoJoStands;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace JoJoFanStands.Items.Stands
{
	public class  Megalovania : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Megalovania");
            Tooltip.SetDefault("Left-click to stare at an enemy and right-click to choose an ability.\nUser Name: AnotherGuy");
        }

        public override void SetDefaults()
        {
            item.damage = 97;
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
            TooltipLine tooltipAddition = new TooltipLine(mod, "Speed", "Attack? Speed: " + (60 - mPlayer.standSpeedBoosts));
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
            recipe.AddIngredient(ItemID.Hellstone, 50);
            recipe.AddIngredient(ItemID.HallowedBar, 25);
            recipe.AddIngredient(ItemID.ChlorophyteOre, 50);
            recipe.AddIngredient(ItemID.GlowingMushroom, 5);
            recipe.AddIngredient(ItemID.LifeCrystal, 6);
            recipe.AddIngredient(JoJoStands.ItemType("WillToFight"), 20);
            recipe.AddIngredient(JoJoStands.ItemType("WillToControl"), 20);
            recipe.AddIngredient(JoJoStands.ItemType("WillToDestroy"), 20);
            recipe.AddTile(JoJoStands.TileType("RemixTableTile"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}