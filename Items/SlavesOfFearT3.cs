using System;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using JoJoStands;

namespace JoJoFanStands.Items
{
	public class SlavesOfFearT3 : ModItem
	{
        public override string Texture
        {
            get { return mod.Name + "/Items/SlavesOfFearT1"; }
        }
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Slaves Of Fear (Tier 3)");
            Tooltip.SetDefault("Left-click to punch enemies at a really fast rate and right click to shunt into enemies!\nSpecial: Weld an enemy to tiles!\nUser Name: The Phantom One \nReference: SLAVES OF FEAR by HEALTH");
        }

        public override void SetDefaults()
        {
            item.damage = 68;
            item.width = 32;
            item.height = 32;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = 5;
            item.maxStack = 1;
            item.knockBack = 2f;
            item.value = 0;
            item.noUseGraphic = true;
            item.rare = 6;
        }

        public override void HoldItem(Player player)
        {
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
                mPlayer.StandOut = true;
                if (player.ownedProjectileCounts[mod.ProjectileType("SlavesOfFearStandT3")] <= 0 && mPlayer.StandOut)
                {
                    Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("SlavesOfFearStandT3"), 0, 0f, Main.myPlayer);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Hellstone, 10);
            recipe.AddIngredient(ItemID.Obsidian, 10);
            recipe.AddIngredient(ItemID.HallowedBar, 10);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
