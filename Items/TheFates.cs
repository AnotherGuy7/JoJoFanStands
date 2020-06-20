using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace JoJoFanStands.Items
{
    public class TheFates : ModItem
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
            item.maxStack = 1;
            item.knockBack = 2f;
            item.value = 0;
            item.noUseGraphic = true;
            item.rare = 6;
        }

        public override void HoldItem(Player player)
        {
            FanPlayer Fplayer = player.GetModPlayer<FanPlayer>();
            Fplayer.TheFatesActive = true;
            if (player.ownedProjectileCounts[mod.ProjectileType("TheFates")] == 0)
            {
                Projectile.NewProjectile(player.position, Vector2.Zero, mod.ProjectileType("TheFates"), 0, 0f, Main.myPlayer);
            }
            base.HoldItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("StandArrow"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
