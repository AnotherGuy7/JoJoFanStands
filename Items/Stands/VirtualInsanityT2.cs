using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class VirtualInsanityT2 : FanStandItemClass
    {
        public override int StandSpeed => 11;
        public override int StandType => 1;
        public override string StandIdentifierName => "VirtualInsanity";
        public override int StandTier => 2;
        public override Color StandTierDisplayColor => Color.OrangeRed;
        public override bool FanStandItem => true;
        public override string Texture => Mod.Name + "/Items/Stands/VirtualInsanityT1";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slaves Of Fear (Tier 1)");
            // Tooltip.SetDefault("Left-click to punch enemies at a really fast rate!\nUser Name: The Phantom One \nReference: SLAVES OF FEAR by HEALTH");
        }

        public override void SetDefaults()
        {
            Item.damage = 43;
            Item.width = 42;
            Item.height = 46;
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
                .AddIngredient(ItemType<VirtualInsanityT1>())
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddIngredient(ItemID.Bomb, 6)
                .AddIngredient(ItemID.IronBar, 9)
                .AddIngredient(ItemType<WillToFight>())
                .AddIngredient(ItemType<WillToChange>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

            CreateRecipe()
                .AddIngredient(ItemType<VirtualInsanityT1>())
                .AddIngredient(ItemID.HellstoneBar, 12)
                .AddIngredient(ItemID.Bomb, 6)
                .AddIngredient(ItemID.LeadBar, 9)
                .AddIngredient(ItemType<WillToFight>())
                .AddIngredient(ItemType<WillToChange>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}
