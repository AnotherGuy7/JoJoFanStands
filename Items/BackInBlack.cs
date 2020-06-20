using Terraria;
using Terraria.ID;
using JoJoStands;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace JoJoFanStands.Items
{
	public class BackInBlack : ModItem
	{
        public static bool teleporting = false;
        public int teleportTime = 0;
        public Vector2 savedPosition1 = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Back In Black");
            Tooltip.SetDefault("Left-click to shoot enemy-chasing shots and right-click to save a position.\nUser Name: AnotherGuy");
        }

        public override void SetDefaults()
        {
            item.damage = 62;
            item.knockBack = 2f;
            item.useStyle = 5;
            item.width = 30;
            item.height = 30;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useTurn = true;
        }

        public override void HoldItem(Player player)
        {
            FanPlayer modPlayer = player.GetModPlayer<FanPlayer>();
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (player.ownedProjectileCounts[mod.ProjectileType("BackInBlackStand")] == 0)
            {
                Projectile.NewProjectile(player.position, Vector2.Zero, mod.ProjectileType("BackInBlackStand"), 0, 0f, Main.myPlayer);
            }
            base.HoldItem(player);
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