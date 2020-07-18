using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System.Collections.Generic;
using JoJoStands;
using static Terraria.ModLoader.ModContent;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;

namespace JoJoFanStands.Items.Stands
{
    public class TheFatesT1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Fates (Tier 1)");
            Tooltip.SetDefault("Left-click to ??? and right-click foresee future attacks! \nUser Name: StringsArn'tRealNumbers \nReference: Emerson Lake and Palmer song, 'The Three Fates'");
        }

        public override void SetDefaults()
        {
            item.damage = 19;
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<StandArrow>());
            recipe.AddIngredient(ItemType<WillToControl>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
