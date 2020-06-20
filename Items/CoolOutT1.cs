using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using Microsoft.Xna.Framework;

namespace JoJoFanStands.Items
{
	public class CoolOutT1 : ModItem
	{

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Tier 1)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and right-click to shoot Ice Weapons! \nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            item.damage = 17;
            item.knockBack = 2f;
            item.useStyle = 5;
            item.width = 30;
            item.height = 30;
            item.autoReuse = true;
            item.useTurn = true;
            item.useTime = 20;
            item.useAnimation = 20;
            item.shootSpeed = 50f;
            item.rare = ItemRarityID.Blue;
        }

        public override void HoldItem(Player player)
        {
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (player.whoAmI == Main.myPlayer)
            {
                mPlayer.StandOut = true;        //so people can't have 2 stands out at the same time
                if (player.ownedProjectileCounts[mod.ProjectileType("CoolOutStand")] == 0 && mPlayer.StandOut)
                {
                    Projectile.NewProjectile(player.position, Vector2.Zero, mod.ProjectileType("CoolOutStand"), 0, 0f, Main.myPlayer);
                }
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