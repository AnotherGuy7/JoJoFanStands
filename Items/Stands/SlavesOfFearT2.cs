using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class SlavesOfFearT2 : FanStandItemClass
    {
        public override int StandSpeed => 12;
        public override int StandType => 1;
        public override string StandProjectileName => "SlavesOfFear";
        public override int StandTier => 2;
        public override bool FanStandItem => true;

        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/SlavesOfFearT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slaves Of Fear (Tier 2)");
            Tooltip.SetDefault("Left-click to punch enemies at a really fast rate and right click to shunt into enemies!\nUser Name: The Phantom One \nReference: SLAVES OF FEAR by HEALTH");
        }

        public override void SetDefaults()
        {
            Item.damage = 48;
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
                .AddIngredient(ItemID.MusicBox)
                .AddIngredient(ItemID.IronBar, 10)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}
