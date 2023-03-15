using JoJoStands;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class FollowMeT1 : FanStandItemClass
    {
        public override int StandSpeed => 12;
        public override int StandType => 1;
        public override string StandProjectileName => "FollowMe";
        public override int StandTier => 1;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Follow Me (Tier 1)");
            Tooltip.SetDefault("Left-click to wind-up a punch and Right-click to grab enemies!\nUser Name: Agatha/Betty/Thabita/Mrs Destiny/Hot Pants \nReference: ???");
        }

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.width = 30;
            Item.height = 28;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            player.GetModPlayer<MyPlayer>().standDefenseToAdd = 6;
            player.GetModPlayer<FanPlayer>().SpawnFanStand();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<StandArrow>())
                .AddIngredient(ItemType<WillToChange>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}