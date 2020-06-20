using Terraria;
using Terraria.ID;
using JoJoStands;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace JoJoFanStands.Items
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
            if (player.ownedProjectileCounts[mod.ProjectileType("MegalovaniaStand")] == 0)
            {
                Projectile.NewProjectile(player.position, Vector2.Zero, mod.ProjectileType("MegalovaniaStand"), 0, 0f, Main.myPlayer);
            }
            base.HoldItem(player);
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