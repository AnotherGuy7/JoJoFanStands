using JoJoFanStands.Buffs;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Armor
{
    public class BrianEnoAct3 : StandItemClass
    {
        public override int StandType => 2;
        public override int StandTier => 3;
        public override Color StandTierDisplayColor => Color.DarkOrange;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Brian Eno (Act 3, Mother Whale Eyeless)");
            // Tooltip.SetDefault("While using any mount gain +10% movement speed, +5% chance to dodge attacks, and a +10% critical strike chance!\nPress special while in a mount to move faster and dodge all attacks!\nPress special while there is no mount to have Brain Eno carry you!");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 64;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            fPlayer.brianEnoAct3 = true;
            player.AddBuff(BuffType<BrianEnoActiveBuff>(), 2);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<BrianEnoAct2>())
                .AddIngredient(ItemID.PalladiumBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 20)
                .AddIngredient(ItemType<SunDroplet>(), 5)
                .AddIngredient(ItemType<WillToEscape>(), 2)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

            CreateRecipe()
                .AddIngredient(ItemType<BrianEnoAct2>())
                .AddIngredient(ItemID.CobaltBar, 12)
                .AddIngredient(ItemID.SoulofFlight, 20)
                .AddIngredient(ItemType<SunDroplet>(), 5)
                .AddIngredient(ItemType<WillToEscape>(), 2)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}