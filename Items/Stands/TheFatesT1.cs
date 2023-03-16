using JoJoFanStands.Projectiles.PlayerStands.TheFates;
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
    public class TheFatesT1 : FanStandItemClass
    {
        public override int StandSpeed => 12;
        public override int StandType => 1;
        public override int StandTier => 1;
        public override Color StandTierDisplayColor => Color.Brown;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Fates (Tier 1)");
            Tooltip.SetDefault("Left-click to ??? and right-click foresee future attacks! \nUser Name: StringsArn'tRealNumbers \nReference: Emerson Lake and Palmer song, 'The Three Fates'");
        }

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            Projectile.NewProjectile(Item.GetSource_FromThis(), player.position, player.velocity, ProjectileType<TheFates>(), 0, 0f, Main.myPlayer);
            return true;
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
