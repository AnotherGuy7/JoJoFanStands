using JoJoFanStands.Projectiles.PlayerStands.TheFates;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class TheFatesT1 : FanStandItemClass
    {
        public override int standSpeed => 12;
        public override int standType => 1;
        public override int standTier => 1;
        public override bool fanStandItem => true;

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
            Projectile.NewProjectile(player.position, player.velocity, ProjectileType<TheFates>(), 0, 0f, Main.myPlayer);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<StandArrow>());
            recipe.AddIngredient(ItemType<WillToControl>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
