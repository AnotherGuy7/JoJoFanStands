using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class RoseColoredBoy : StandItemClass
    {
        public override int standSpeed => 12;
        public override int standType => 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rose Colored Boy");
            Tooltip.SetDefault("Left-click to punch and right-click to shoot cursed flowers!\n Look on the bright side, if you want to go blind.\nUser Name: Placement \nReference: Rose Colored Boy by Paramore");
        }

        public override void SetDefaults()
        {
            item.damage = 62;
            item.knockBack = 2f;
            item.useStyle = 5;
            item.width = 30;
            item.height = 30;
            item.autoReuse = true;
            item.useTurn = true;
            item.useTime = 20;
            item.useAnimation = 20;
            item.shoot = mod.ProjectileType("RoseColoredBoyFist");
            item.shootSpeed = 50f;
        }

        public override void HoldItem(Player player)
        {
            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed)
            {
                player.AddBuff(mod.BuffType("RoseColoredSunBuff"), 300);
            }
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                return true;
            }
            if (player.altFunctionUse != 2)
            {
                float numberProjectiles = 3 + Main.rand.Next(5);
                float rotation = MathHelper.ToRadians(45);
                position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f;
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
                }
                return false;
            }
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.damage = 41;
                item.knockBack = 3f;
                item.useStyle = 5;
                item.autoReuse = true;
                item.useTurn = true;
                item.useTime = 30;
                item.useAnimation = 30;
                item.shoot = mod.ProjectileType("RosePetal");
                item.shootSpeed = 50f;
            }
            if (player.altFunctionUse != 2)
            {
                item.damage = 62;
                item.knockBack = 2f;
                item.useStyle = 5;
                item.autoReuse = true;
                item.useTurn = true;
                item.useTime = 12;
                item.useAnimation = 12;
                item.shoot = mod.ProjectileType("RoseColoredBoyFist");
                item.shootSpeed = 50f;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<StandArrow>());
            recipe.AddIngredient(ItemType<WillToFight>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}