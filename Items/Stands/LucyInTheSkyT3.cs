using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class LucyInTheSkyT3 : FanStandItemClass
    {
        public override int StandSpeed => 10;
        public override int StandType => 2;
        public override string StandProjectileName => "LucyInTheSky";
        public override int StandTier => 3;
        public override Color StandTierDisplayColor => Color.LightGreen;
        public override bool FanStandItem => true;
        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/LucyInTheSkyT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lucy in the Sky (Tier 3)");
            Tooltip.SetDefault("Left-click to barrage the life out of enemies and right-click on a light source to create a marker!" +
                "\nHold right-click on a light source to hide inside of the network!" +
                "\nSpecial: Electrify all active highways!" +
                "\nSecond Special: Show/Hide the Marker Menu" +
                "\nNote: Up to 8 Light Markers can be placed." +
                "\nUser Name: Archerous" +
                "\nReference: Lucy in the Sky by The Beatles");
        }

        public override void SetDefaults()
        {
            Item.damage = 78;
            Item.width = 50;
            Item.height = 50;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            SoundEngine.PlaySound(LucyInTheSkyT1.lucyInTheSkySpawnSound);
            player.GetModPlayer<FanPlayer>().SpawnFanStand();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LucyInTheSkyT2>())
                .AddIngredient(ItemID.AlchemyLantern, 14)
                .AddIngredient(ItemID.HallowedBar, 6)
                .AddIngredient(ModContent.ItemType<WillToEscape>(), 2)
                .AddIngredient(ModContent.ItemType<WillToChange>(), 3)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}