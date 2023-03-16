using JoJoStands;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class ExpansesFinal : FanStandItemClass
    {
        public override int StandSpeed => 7;
        public override int StandType => 2;
        public override string StandProjectileName => "Expanses";
        public override int StandTier => 4;
        public override Color StandTierDisplayColor => Color.Blue;
        public override bool FanStandItem => true;

        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/ExpansesT1"; }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Expanses (Final Tier)");
            Tooltip.SetDefault("Left-click to shoot sharp, glass-like crystals and hold right to launch a 4 piercing columns.\nSpecial: Crystallize the space in front of you and get through it. \nSecondary Special: Crystallize yourself. \nUser Name: Prime Emperor \nReference: Expanses, album by The Green Kingdom");
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.width = 26;
            Item.height = 50;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            player.GetModPlayer<MyPlayer>().standDefenseToAdd = 3;
            player.GetModPlayer<FanPlayer>().SpawnFanStand();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ExpansesT3>())
                .AddIngredient(518, 1)
                .AddIngredient(3459, 6)
                .AddIngredient(ModContent.ItemType<RighteousLifeforce>(), 1)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}