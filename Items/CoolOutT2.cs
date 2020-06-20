using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using Microsoft.Xna.Framework;

namespace JoJoFanStands.Items
{
	public class CoolOutT2 : ModItem
	{
        public override string Texture
        {
            get { return mod.Name + "/Items/CoolOutT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Tier 2)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and hold right-click to charge up a spear!\nSpecial: Send out an ice wave!\nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            item.damage = 35;
            item.knockBack = 10f;
            item.useStyle = 5;
            item.width = 30;
            item.height = 30;
            item.autoReuse = true;
            item.useTurn = true;
            item.useTime = 20;
            item.useAnimation = 20;
            item.shootSpeed = 50f;
        }

        public override void HoldItem(Player player)
        {
            FanPlayer Fplayer = player.GetModPlayer<FanPlayer>();
            Fplayer.CoolOutActive = true;
            if (player.ownedProjectileCounts[mod.ProjectileType("CoolOutStandT2")] == 0)
            {
                Projectile.NewProjectile(player.position, Vector2.Zero, mod.ProjectileType("CoolOutStandT2"), 0, 0f, Main.myPlayer);
            }
            base.HoldItem(player);
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            Mod JoJoStands = ModLoader.GetMod("JoJoStands");
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