using JoJoStands;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class CoolOutFinal : FanStandItemClass
    {
        public override int StandSpeed => 20;
        public override int StandType => 2;
        public override string StandIdentifierName => "CoolOut";
        public override int StandTier => 4;
        public override Color StandTierDisplayColor => Color.LightBlue;
        public override bool FanStandItem => true;

        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/CoolOutT1"; }
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cool Out (Final Tier)");
            // Tooltip.SetDefault("Left-click to shoot an Ice Bolt and hold right-click to charge up a spear!\nSpecial: Send out an ice wave!\nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            Item.damage = 51;
            Item.width = 36;
            Item.height = 40;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            player.GetModPlayer<MyPlayer>().standDefenseToAdd = 4;
            player.GetModPlayer<FanPlayer>().SpawnFanStand();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<CoolOutT3>())
                .AddIngredient(ItemID.PearlstoneBlock, 20)
                .AddIngredient(ItemID.FrostCore)
                .AddIngredient(ItemID.CrystalShard, 8)
                .AddIngredient(ItemType<WillToProtect>(), 2)
                .AddIngredient(ItemType<WillToControl>(), 2)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}