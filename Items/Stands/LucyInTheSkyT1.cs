using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class LucyInTheSkyT1 : FanStandItemClass
    {
        public override int StandSpeed => 12;
        public override int StandType => 1;
        public override string StandIdentifierName => "LucyInTheSky";
        public override int StandTier => 1;
        public override Color StandTierDisplayColor => Color.LightGreen;
        public override bool FanStandItem => true;
        public static readonly SoundStyle lucyInTheSkySpawnSound = new SoundStyle("JoJoFanStands/Sounds/SummonCries/LucyInTheSky").WithVolumeScale(JoJoStands.JoJoStands.ModSoundsVolume);

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lucy in the Sky (Tier 1)");
            /* Tooltip.SetDefault("Left-click to barrage the life out of enemies and right-click on a light source to create a marker!" +
                "\nSecond Special: Show/Hide the Marker Menu" +
                "\nNote: Up to 3 Light Markers can be placed." + 
                "\nUser Name: Archerous" +
                "\nReference: Lucy in the Sky by The Beatles"); */
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
            player.GetModPlayer<FanPlayer>().SpawnFanStand();
            return true;
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