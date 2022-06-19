using JoJoStands.Items.Armor.VampirismArmors;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class FollowMeBetweenT1 : FanStandItemClass
    {
        public override string Texture => Mod.Name + "/Items/Stands/FollowMeT1";
        public override int standSpeed => 12;
        public override int standType => 1;
        public override bool FanStandItem => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Follow Me: Between (Tier 1)");
            Tooltip.SetDefault("Left-click to wind-up a punch and Right-click to grab enemies!\nSpecial: Intangible\nUser Name: Agatha/Betty/Thabita/Mrs Destiny/Hot Pants \nReference: ???");
        }

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.width = 30;
            Item.height = 28;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<FollowMeT2>())
                .AddIngredient(ItemID.SoulofFright, 16)
                .AddIngredient(ItemID.BrokenBatWing)
                .AddIngredient(ItemType<StoneMask>())
                .AddIngredient(ItemType<WillToChange>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}