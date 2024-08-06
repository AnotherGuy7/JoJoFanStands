using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class WaywardSonT2 : FanStandItemClass
    {
        public override int StandSpeed => 13;
        public override int StandType => 1;
        public override string StandIdentifierName => "WaywardSon";
        public override int StandTier => 2;
        public override Color StandTierDisplayColor => Color.RosyBrown;
        public override bool FanStandItem => true;
        public override string Texture => Mod.Name + "/Items/Stands/WaywardSonT1";


        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slaves Of Fear (Tier 1)");
            // Tooltip.SetDefault("Left-click to punch enemies at a really fast rate!\nUser Name: The Phantom One \nReference: SLAVES OF FEAR by HEALTH");
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.width = 38;
            Item.height = 48;
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
                .AddIngredient(ItemType<WaywardSonT1>())
                .AddIngredient(ItemID.GoldBar, 15)
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.Topaz, 2)
                .AddIngredient(ItemID.ManaCrystal, 5)
                .AddIngredient(ItemType<WillToChange>(), 2)
                .AddIngredient(ItemType<WillToDestroy>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

            CreateRecipe()
                .AddIngredient(ItemType<WaywardSonT1>())
                .AddIngredient(ItemID.GoldBar, 15)
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.Topaz, 2)
                .AddIngredient(ItemID.ManaCrystal, 5)
                .AddIngredient(ItemType<WillToChange>(), 2)
                .AddIngredient(ItemType<WillToDestroy>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

            CreateRecipe()
                .AddIngredient(ItemType<WaywardSonT1>())
                .AddIngredient(ItemID.PlatinumBar, 15)
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.Topaz, 2)
                .AddIngredient(ItemID.ManaCrystal, 5)
                .AddIngredient(ItemType<WillToChange>(), 2)
                .AddIngredient(ItemType<WillToDestroy>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

            CreateRecipe()
                .AddIngredient(ItemType<WaywardSonT1>())
                .AddIngredient(ItemID.PlatinumBar, 15)
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.Topaz, 2)
                .AddIngredient(ItemID.ManaCrystal, 5)
                .AddIngredient(ItemType<WillToChange>(), 2)
                .AddIngredient(ItemType<WillToDestroy>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}
