using JoJoStands;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class LucyInTheSkyT1 : FanStandItemClass
    {
        public override int standSpeed => 12;
        public override int standType => 2;
        public override string standProjectileName => "LucyInTheSky";
        public override int standTier => 1;
        public override bool FanStandItem => true;
        public static readonly SoundStyle lucyInTheSkySpawnSound = new SoundStyle("JoJoFanStands/Sounds/SummonCries/LucyInTheSky").WithVolumeScale(MyPlayer.ModSoundsVolume);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lucy in the Sky (Tier 1)");
            Tooltip.SetDefault("Left-click to barrage the life out of enemies and right-click on a light source to create a marker!\nRight-click anywhere to bring up the Marker Menu\nUser Name: Archerous \nReference: Lucy in the Sky by The Beatles");
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.width = 50;
            Item.height = 50;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            SoundEngine.PlaySound(lucyInTheSkySpawnSound);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<StandArrow>())
                .AddIngredient(ModContent.ItemType<WillToEscape>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}