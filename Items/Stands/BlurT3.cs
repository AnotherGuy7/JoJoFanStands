using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class BlurT3 : FanStandItemClass
    {
        public override int StandSpeed => 6;
        public override int StandType => 1;
        public override string StandProjectileName => "Blur";
        public override int StandTier => 3;
        public override Color StandTierDisplayColor => BlurFinal.BlurStandTierColor;
        public override bool FanStandItem => true;
        public override string Texture => Mod.Name + "/Items/Stands/BlurT1";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blur (Tier 1)");
            // Tooltip.SetDefault("Left-click shoot a random enemy.\nUser Name: Pauline \nReference: Blur by Lincoln");
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.width = 38;
            Item.height = 46;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            player.GetModPlayer<FanPlayer>().SpawnFanStand();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 14)
                .AddIngredient(ItemID.IronBar, 10)
                .AddIngredient(ItemType<WillToControl>(), 2)
                .AddIngredient(ItemType<WillToEscape>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 14)
                .AddIngredient(ItemID.LeadBar, 10)
                .AddIngredient(ItemType<WillToControl>(), 2)
                .AddIngredient(ItemType<WillToEscape>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}