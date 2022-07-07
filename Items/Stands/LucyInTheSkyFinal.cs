using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class LucyInTheSkyFinal : FanStandItemClass
    {
        public override int standSpeed => 9;
        public override int standType => 2;
        public override string standProjectileName => "LucyInTheSky";
        public override int standTier => 4;
        public override bool FanStandItem => true;
        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/LucyInTheSkyT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lucy in the Sky (Final Tier)");
            Tooltip.SetDefault("Left-click to barrage the life out of enemies and right-click on a light source to create a marker!\nRight-click anywhere to bring up the Marker Menu\nUser Name: Archerous \nReference: Lucy in the Sky by The Beatles");
        }

        public override void SetDefaults()
        {
            Item.damage = 101;
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
                .AddIngredient(ModContent.ItemType<LucyInTheSkyT3>())
                .AddIngredient(ItemID.MartianLantern, 6)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddIngredient(ItemID.ChlorophyteBar, 16)
                .AddIngredient(ModContent.ItemType<WillToEscape>(), 3)
                .AddIngredient(ModContent.ItemType<WillToChange>(), 5)
                .AddIngredient(ModContent.ItemType<DeterminedLifeforce>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}