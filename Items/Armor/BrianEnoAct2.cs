using JoJoFanStands.Buffs;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Armor
{
    public class BrianEnoAct2 : StandItemClass
    {
        public override int standType => 2;
        public override int standTier => 2;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brian Eno (Act 2, Mother Whale Eyeless)");
            Tooltip.SetDefault("While using any mount gain +10% movement speed, +5% chance to dodge attacks, and a +10% critical strike chance!\nPress special while in a mount to move faster and dodge all attacks!\nPress special while there is no mount to have Brain Eno carry you!");
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 56;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            fPlayer.BrianEnoAct2 = true;
            player.AddBuff(BuffType<BrianEnoActiveBuff>(), 2);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<BrianEnoAct1>())
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddIngredient(ItemID.Coral, 5)
                .AddIngredient(ItemType<SunDroplet>(), 5)
                .AddIngredient(ItemID.Bone, 30)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}