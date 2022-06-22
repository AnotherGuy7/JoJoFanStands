using JoJoFanStands.Projectiles.PlayerStands.BackInBlack;
using JoJoStands.Items;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class BackInBlack : FanStandItemClass
    {
        public override int standSpeed => 40;
        public override int standType => 2;
        public override int standTier => 1;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Back In Black");
            Tooltip.SetDefault("Left-click to shoot enemy-chasing shots and right-click to save a position.\nUser Name: AnotherGuy");
        }

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.width = 38;
            Item.height = 48;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            Projectile.NewProjectile(Item.GetSource_FromThis(), player.position, player.velocity, ProjectileType<BackInBlackStand>(), 0, 0f, Main.myPlayer);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<StandArrow>())
                .AddIngredient(ItemID.HallowedBar, 9)
                .AddIngredient(ItemID.CursedFlame, 12)
                .AddIngredient(ItemID.DemoniteBar, 15)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

            CreateRecipe()
                .AddIngredient(ItemType<StandArrow>())
                .AddIngredient(ItemID.HallowedBar, 9)
                .AddIngredient(ItemID.Ichor, 12)
                .AddIngredient(ItemID.CrimtaneBar, 15)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}