using JoJoStands;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class CoolOutT3 : FanStandItemClass
    {
        public override int StandSpeed => 30;
        public override int StandType => 2;
        public override string StandProjectileName => "CoolOut";
        public override int StandTier => 3;
        public override bool FanStandItem => true;

        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/CoolOutT1"; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Out (Tier 3)");
            Tooltip.SetDefault("Left-click to shoot an Ice Bolt and press right-click to use the selected ability!\nSpecial: Cycle throgh abilities!\nUser Name: NekroSektor \nReference: Cool Out and Natural by Imagine Dragons");
        }

        public override void SetDefaults()
        {
            Item.damage = 39;
            Item.width = 36;
            Item.height = 42;
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
                .AddIngredient(ItemType<CoolOutT2>())
                .AddIngredient(ItemID.DemoniteBar, 6)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemType<WillToProtect>(), 2)
                .AddIngredient(ItemType<WillToControl>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();

            CreateRecipe()
                .AddIngredient(ItemType<CoolOutT2>())
                .AddIngredient(ItemID.CrimtaneBar, 6)
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemType<WillToProtect>(), 2)
                .AddIngredient(ItemType<WillToControl>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}