using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class MortalReminderT1 : FanStandItemClass
    {
        public override int StandSpeed => 17;
        public override int StandType => 1;
        public override int StandTier => 1;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mortal Reminder (Tier 1)");
            Tooltip.SetDefault("Left-click to punch enemies at a really fast rate!\nUser Name: Benney \nReference: Mortal Reminder by Pentakill");
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.width = 34;
            Item.height = 44;
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
                .AddIngredient(ItemType<StandArrow>())
                .AddIngredient(ItemType<WillToChange>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}
