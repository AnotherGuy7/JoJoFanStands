using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class LucyInTheSkyT3 : FanStandItemClass
    {
        public override int standSpeed => 10;
        public override int standType => 2;
        public override string standProjectileName => "LucyInTheSky";
        public override int standTier => 3;
        public override bool FanStandItem => true;
        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/LucyInTheSkyT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lucy in the Sky (Tier 3)");
            Tooltip.SetDefault("Left-click to barrage the life out of enemies and right-click on a light source to create a marker!\nRight-click anywhere to bring up the Marker Menu\nUser Name: Archerous \nReference: Lucy in the Sky by The Beatles");
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
            return false;
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