using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class SlavesOfFearT3 : FanStandItemClass
    {
        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/SlavesOfFearT1"; }
        }

        public override int StandSpeed => 11;
        public override int StandType => 1;
        public override string StandProjectileName => "SlavesOfFear";
        public override int StandTier => 3;
        public override Color StandTierDisplayColor => Color.ForestGreen;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slaves Of Fear (Tier 3)");
            Tooltip.SetDefault("Left-click to punch enemies at a really fast rate and right click to shunt into enemies!\nSpecial: Weld an enemy to tiles!\nUser Name: The Phantom One \nReference: SLAVES OF FEAR by HEALTH");
        }

        public override void SetDefaults()
        {
            Item.damage = 68;
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
                .AddIngredient(ItemID.Hellstone, 10)
                .AddIngredient(ItemID.Obsidian, 10)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}
