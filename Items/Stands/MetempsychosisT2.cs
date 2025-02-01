using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class MetempsychosisT2 : FanStandItemClass
    {
        public override int StandSpeed => 11;
        public override int StandType => 1;
        public override string StandIdentifierName => "Metempsychosis";
        public override int StandTier => 2;
        public override Color StandTierDisplayColor => Color.Magenta;
        public override bool FanStandItem => true;
        public override string Texture => Mod.Name + "/Items/Stands/MetempsychosisT1";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slaves Of Fear (Tier 1)");
            // Tooltip.SetDefault("Left-click to punch enemies at a really fast rate!\nUser Name: The Phantom One \nReference: SLAVES OF FEAR by HEALTH");
        }

        public override void SetDefaults()
        {
            Item.damage = 43;
            Item.width = 50;
            Item.height = 50;
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
                .AddIngredient(ItemType<MetempsychosisT1>())
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddIngredient(ItemID.LifeCrystal, 1)
                .AddIngredient(ItemType<WillToProtect>(), 2)
                .AddIngredient(ItemType<WillToFight>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}
