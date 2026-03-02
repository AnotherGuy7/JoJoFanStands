using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Stands
{
    public class HolyDiverFinal : FanStandItemClass
    {
        public override int StandSpeed => 5;
        public override int StandType => 1;
        public override string StandIdentifierName => "HolyDiver";
        public override int StandTier => 4;
        public override Color StandTierDisplayColor => HolyDiverStandTierColor;
        public override bool FanStandItem => true;
        public override string Texture => Mod.Name + "/Items/Stands/HolyDiverT1";

        public static readonly Color HolyDiverStandTierColor = new Color(30, 100, 180);

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Holy Diver (Tier 4)");
            //Tooltip.SetDefault("Left-click to unleash a scorching torrent barrage.\nUser Name: Some long name\nReference: Holy Diver by Ronnie James Dio");
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
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

        public override void AddRecipes()
        {
            // TODO: Replace HolyDiverT3 with actual T3 item type once created
            // CreateRecipe()
            //     .AddIngredient(ModContent.ItemType<HolyDiverT3>())
            //     .AddIngredient(ItemID.SharkFin, 12)
            //     .AddIngredient(ItemID.IronBar, 12)
            //     .AddIngredient(ModContent.ItemType<DeterminedLifeforce>())
            //     .AddTile(ModContent.TileType<RemixTableTile>())
            //     .Register();
        }
    }
}