using JoJoFanStands.Projectiles.PlayerStands.RoseColoredBoy;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class RoseColoredBoy : FanStandItemClass
    {
        public override int standSpeed => 10;
        public override int standType => 1;
        public override int standTier => 1;
        public override bool fanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rose Colored Boy");
            Tooltip.SetDefault("Left-click to punch and right-click to shoot burning rose petals!\nLook on the bright side, if you want to go blind.\nUser Name: Placement \nReference: Rose Colored Boy by Paramore");
        }

        public override void SetDefaults()
        {
            item.damage = 62;
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

        public override bool ManualStandSpawning(Player player)
        {
            Projectile.NewProjectile(player.position, player.velocity, ProjectileType<RoseColoredBoyStand>(), 0, 0f, Main.myPlayer);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<StandArrow>());
            recipe.AddIngredient(ItemType<WillToFight>());
            recipe.AddIngredient(ItemID.HallowedBar, 8);
            recipe.AddIngredient(ItemID.Fireblossom, 5);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}