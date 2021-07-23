using JoJoFanStands.Projectiles.PlayerStands.Megalovania;
using JoJoStands.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class Megalovania : FanStandItemClass
    {
        public override int standSpeed => 60;
        public override int standType => 2;
        public override int standTier => 1;
        public override bool fanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Megalovania");
            Tooltip.SetDefault("Left-click to stare at an enemy and right-click to choose an ability.\nUser Name: AnotherGuy");
        }

        public override void SetDefaults()
        {
            item.damage = 97;
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
            Projectile.NewProjectile(player.position, player.velocity, ProjectileType<MegalovaniaStand>(), 0, 0f, Main.myPlayer);
            return true;
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