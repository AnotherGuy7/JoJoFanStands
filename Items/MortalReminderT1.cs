using System;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using JoJoStands;

namespace JoJoFanStands.Items
{
	public class MortalReminderT1 : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mortal Reminder (Tier 1)");
            Tooltip.SetDefault("Left-click to punch enemies at a really fast rate!\nUser Name: Benney \nReference: Mortal Reminder by Pentakill");
        }

        public override void SetDefaults()
        {
            item.damage = 18;
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
            MyPlayer.standTier1List.Add(mod.ItemType(Name));
        }

        public override void HoldItem(Player player)
        {
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
                mPlayer.StandOut = true;
                if (player.ownedProjectileCounts[mod.ProjectileType("MortalReminderStandT1")] <= 0 && mPlayer.StandOut)
                {
                    Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("MortalReminderStandT1"), 0, 0f, Main.myPlayer);
                }
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
