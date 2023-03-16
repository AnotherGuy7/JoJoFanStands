using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class SlavesOfFearFinal : FanStandItemClass
    {
        public override int StandSpeed => 10;
        public override int StandType => 1;
        public override string StandProjectileName => "SlavesOfFear";
        public override int StandTier => 4;
        public override Color StandTierDisplayColor => Color.ForestGreen;
        public override bool FanStandItem => true;

        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/SlavesOfFearT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slaves Of Fear (Final Tier)");
            Tooltip.SetDefault("Left-click to punch enemies at a really fast rate and right click to shunt into enemies!\nSpecial: Weld an enemy to tiles!\nUser Name: The Phantom One \nReference: SLAVES OF FEAR by HEALTH");
        }

        public override void SetDefaults()
        {
            Item.damage = 98;
            Item.width = 38;
            Item.height = 48;
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
                .AddIngredient(ItemID.AsphaltBlock, 10)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.TopHat)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}
