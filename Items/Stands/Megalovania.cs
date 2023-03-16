using JoJoFanStands.Projectiles.PlayerStands.Megalovania;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class Megalovania : FanStandItemClass
    {
        public override int StandSpeed => 60;
        public override int StandType => 2;
        public override int StandTier => 1;
        public override Color StandTierDisplayColor => Color.Gray;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Megalovania");
            Tooltip.SetDefault("Left-click to stare at an enemy and right-click to choose an ability.\nUser Name: AnotherGuy");
        }

        public override void SetDefaults()
        {
            Item.damage = 97;
            Item.width = 32;
            Item.height = 28;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            Projectile.NewProjectile(Item.GetSource_FromThis(), player.position, player.velocity, ProjectileType<MegalovaniaStand>(), 0, 0f, Main.myPlayer);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<StandArrow>())
                .AddIngredient(ItemID.Hellstone, 50)
                .AddIngredient(ItemID.HallowedBar, 25)
                .AddIngredient(ItemID.ChlorophyteOre, 50)
                .AddIngredient(ItemID.GlowingMushroom, 5)
                .AddIngredient(ItemID.LifeCrystal, 6)
                .AddIngredient(ItemType<WillToFight>(), 20)
                .AddIngredient(ItemType<WillToControl>(), 20)
                .AddIngredient(ItemType<WillToDestroy>(), 20)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}