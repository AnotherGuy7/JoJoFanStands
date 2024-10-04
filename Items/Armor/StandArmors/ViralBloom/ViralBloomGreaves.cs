using JoJoFanStands.Buffs;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items.Armor.StandArmors.ViralBloom
{
    [AutoloadEquip(EquipType.Legs)]
    public class ViralBloomGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("ViralBloom Greaves");
            // Tooltip.SetDefault("Greaves made with the finest metal out there, enchanced with Viral Meteorite.\n+16% Stand Crit Chance\n+30% Movement Speed");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.value = Item.buyPrice(gold: 76);
            Item.rare = ItemRarityID.Red;
            Item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            mPlayer.standCritChangeBoosts += 16f;
            player.moveSpeed *= 1.30f;
        }


        public override void AddRecipes()
        {
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
                return;

            Recipe recipe = CreateRecipe()
                .AddIngredient(ItemType<ViralMeteoriteBar>(), 3)
                .AddTile(TileID.LunarCraftingStation)

            if (calamityMod.TryFind<ModItem>("UelibloomBar", out ModItem uelibloomBar))
            {
                recipe.AddIngredient(uelibloomBar.Type, 7);
            }
            recipe.Register();
        }
    }
}