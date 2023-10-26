using JoJoStands;
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
    public class TheWorldOverHeavenFinal : FanStandItemClass
    {
        public override int StandSpeed => 8;
        public override int StandType => 1;
        public override string StandProjectileName => "TheWorldOverHeaven";
        public override int StandTier => 5;
        public override Color StandTierDisplayColor => Color.WhiteSmoke;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blur (Tier 1)");
            // Tooltip.SetDefault("Left-click shoot a random enemy.\nUser Name: Pauline \nReference: Blur by Lincoln");
        }

        public override void SetDefaults()
        {
            Item.damage = 148;
            Item.width = 38;
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

        public override void UpdateInventory(Player player)
        {
            int highestDamage = 148;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i] != null)
                {
                    if (player.inventory[i].damage > highestDamage)
                        highestDamage = player.inventory[i].damage;
                }
            }
            Item.damage = (int)(highestDamage * player.GetModPlayer<MyPlayer>().standDamageBoosts);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<StandArrow>())
                .AddIngredient(ItemType<WillToControl>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}