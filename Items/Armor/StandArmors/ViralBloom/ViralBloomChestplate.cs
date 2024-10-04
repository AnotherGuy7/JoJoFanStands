using JoJoFanStands.Buffs;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Armor.StandArmors.ViralBloom
{
    [AutoloadEquip(EquipType.Body)]
    public class ViralBloomChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("BiralBloom Chestplate");
            // Tooltip.SetDefault("A chestplate made with the finest alloy of Uelibloom and Viral Meteorite.\n+15% Stand Crit Chance\n+4 Stand Speed\n30% Incoming Damage Reduction");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.value = Item.buyPrice(gold: 80);
            Item.rare = ItemRarityID.Red;
            Item.defense = 34;
        }

        public override void UpdateEquip(Player player)
        {
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            mPlayer.standCritChangeBoosts += 15f;
            mPlayer.standSpeedBoosts += 4;
            player.endurance += 0.30f;
        }

        public override void AddRecipes()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
                return;

            Recipe recipe = CreateRecipe()
                .AddIngredient(ItemType<ViralMeteoriteBar>(), 5)
                .AddTile(TileID.LunarCraftingStation)

            if (calamityMod.TryFind<ModItem>("UelibloomBar", out ModItem uelibloomBar))
            {
                recipe.AddIngredient(uelibloomBar.Type, 20);
            }
            recipe.Register();
        }
    }
}