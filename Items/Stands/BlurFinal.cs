using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class BlurFinal : FanStandItemClass
    {
        public override int StandSpeed => 5;
        public override int StandType => 1;
        public override string StandIdentifierName => "Blur";
        public override int StandTier => 4;
        public override Color StandTierDisplayColor => BlurStandTierColor;
        public override bool FanStandItem => true;
        public override string Texture => Mod.Name + "/Items/Stands/BlurT1";

        public static readonly Color BlurStandTierColor = new Color(61, 79, 97);

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blur (Tier 1)");
            // Tooltip.SetDefault("Left-click shoot a random enemy.\nUser Name: Pauline \nReference: Blur by Lincoln");
        }

        public override void SetDefaults()
        {
            Item.damage = 68;
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
                .AddIngredient(ModContent.ItemType<BlurT3>())
                .AddIngredient(ItemID.ShroomiteBar, 12)
                .AddIngredient(ItemID.IronBar, 12)
                .AddIngredient(ModContent.ItemType<DeterminedLifeforce>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

            CreateRecipe()
                .AddIngredient(ModContent.ItemType<BlurT3>())
                .AddIngredient(ItemID.ShroomiteBar, 12)
                .AddIngredient(ItemID.LeadBar, 12)
                .AddIngredient(ModContent.ItemType<DeterminedLifeforce>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}