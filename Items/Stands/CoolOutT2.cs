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
    public class CoolOutT2 : FanStandItemClass
    {
        public override int StandSpeed => 35;
        public override int StandType => 2;
        public override string StandProjectileName => "CoolOut";
        public override int StandTier => 2;
        public override Color StandTierDisplayColor => Color.LightBlue;
        public override bool FanStandItem => true;

        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/CoolOutT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Tier 2)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and hold right-click to charge up a spear!\nSpecial: Send out an ice wave!\nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.width = 36;
            Item.height = 40;
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
                .AddIngredient(ItemType<CoolOutT1>())
                .AddIngredient(ItemID.DemoniteOre, 20)
                .AddIngredient(ItemID.Shiverthorn, 5)
                .AddIngredient(ItemType<WillToProtect>())
                .AddIngredient(ItemType<WillToControl>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

            CreateRecipe()
                .AddIngredient(ItemType<CoolOutT1>())
                .AddIngredient(ItemID.CrimtaneOre, 20)
                .AddIngredient(ItemID.Shiverthorn, 5)
                .AddIngredient(ItemType<WillToProtect>())
                .AddIngredient(ItemType<WillToControl>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}