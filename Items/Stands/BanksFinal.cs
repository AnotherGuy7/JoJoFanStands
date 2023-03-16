using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class BanksFinal : FanStandItemClass
    {
        public override int StandSpeed => 6;           //9-10 shots/s
        public override int StandType => 2;
        public override string StandProjectileName => "Banks";
        public override int StandTier => 4;
        public override Color StandTierDisplayColor => Color.Gold;
        public override bool FanStandItem => true;

        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/BanksT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Banks (Final Tier)");
            Tooltip.SetDefault("Left-click shoot a random enemy and right-click to use a shotgun.\nSpecial: Enemies killed drop 2.0x the amount of coins. Defense reduced by 10.\nUser Name: Pauline \nReference: Banks by Lincoln");
        }

        public override void SetDefaults()
        {
            Item.damage = 17;
            Item.width = 30;
            Item.height = 36;
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
                .AddIngredient(ItemID.GoldBar, 50)
                .AddIngredient(ItemID.Ruby, 10)
                .AddIngredient(ItemID.Ectoplasm, 30)
                .AddIngredient(ItemID.SoulofLight, 30)
                .AddIngredient(ItemID.SoulofNight, 50)
                .AddIngredient(ItemType<WillToChange>(), 2)
                .AddIngredient(ItemType<WillToProtect>(), 2)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

        }
    }
}