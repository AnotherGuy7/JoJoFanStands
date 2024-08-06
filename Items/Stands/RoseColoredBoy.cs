using JoJoFanStands.Projectiles.PlayerStands.RoseColoredBoy;
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
    public class RoseColoredBoy : FanStandItemClass
    {
        public override int StandSpeed => 10;
        public override int StandType => 1;
        public override int StandTier => 1;
        public override string StandIdentifierName => "RoseColoredBoy";
        public override Color StandTierDisplayColor => Color.Orange;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rose Colored Boy");
            // Tooltip.SetDefault("Left-click to punch and right-click to shoot burning rose petals!\nLook on the bright side, if you want to go blind.\nUser Name: Placement \nReference: Rose Colored Boy by Paramore");
        }

        public override void SetDefaults()
        {
            Item.damage = 62;
            Item.width = 56;
            Item.height = 48;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            Projectile.NewProjectile(Item.GetSource_FromThis(), player.position, player.velocity, ProjectileType<RoseColoredBoyStand>(), 0, 0f, Main.myPlayer);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<StandArrow>())
                .AddIngredient(ItemType<WillToFight>())
                .AddIngredient(ItemID.HallowedBar, 8)
                .AddIngredient(ItemID.Fireblossom, 5)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}