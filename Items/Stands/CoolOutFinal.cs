using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using System.Collections.Generic;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
	public class CoolOutFinal : ModItem
	{
        public override string Texture
        {
            get { return mod.Name + "/Items/Stands/CoolOutT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Final Tier)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and hold right-click to charge up a spear!\nSpecial: Send out an ice wave!\nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            item.damage = 51;
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
            TooltipLine tooltipAddition = new TooltipLine(mod, "Speed", "Shoot Speed: " + (20 - mPlayer.standSpeedBoosts));
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
            recipe.AddIngredient(ItemType<CoolOutT3>());
            recipe.AddIngredient(ItemID.PearlstoneBlock, 20);
            recipe.AddIngredient(ItemID.FrostCore);
            recipe.AddIngredient(ItemID.CrystalShard, 8);
            recipe.AddIngredient(JoJoStands.ItemType("WillToProtect"), 2);
            recipe.AddIngredient(JoJoStands.ItemType("WillToControl"), 2);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}