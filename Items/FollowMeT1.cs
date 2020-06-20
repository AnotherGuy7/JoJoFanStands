using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using Microsoft.Xna.Framework;

namespace JoJoFanStands.Items
{
	public class FollowMeT1 : ModItem
	{

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Follow Me (Tier 1)");
            Tooltip.SetDefault("Left-click to wind-up a punch and Right-click to grab enemies!\nSpecial: Intangible\nUser Name: Betty \nReference: ???");
        }

        public override void SetDefaults()
        {
            item.damage = 21;
            item.knockBack = 2f;
            item.width = 30;
            item.height = 30;
            item.noUseGraphic = true;
            item.rare = ItemRarityID.LightPurple;
        }

        public override void HoldItem(Player player)
        {
            FanPlayer Fplayer = player.GetModPlayer<FanPlayer>();
            Fplayer.StandOut = true;
            if (player.ownedProjectileCounts[mod.ProjectileType("FollowMeStandT1")] == 0)
            {
                Projectile.NewProjectile(player.position, Vector2.Zero, mod.ProjectileType("FollowMeStandT1"), 0, 0f, Main.myPlayer);
            }
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