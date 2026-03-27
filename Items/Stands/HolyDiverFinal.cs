using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace JoJoFanStands.Items.Stands
{
    public class HolyDiverFinal : FanStandItemClass
    {
        public override int StandSpeed => 7;
        public override int StandType => 1;
        public override string StandIdentifierName => "HolyDiver";
        public override int StandTier => 4;
        public override Color StandTierDisplayColor => HolyDiverStandTierColor;
        public override bool FanStandItem => true;
        public override string Texture => Mod.Name + "/Items/Stands/HolyDiverT1";
        public static readonly Color HolyDiverStandTierColor = new Color(30, 100, 180);

        private Item _waterKatana;
        private Item _waterHook;
        private string _katanaKey = "";
        private string _hookKey = "";

        public override void SetStaticDefaults() { }

        public override void SetDefaults()
        {
            Item.damage = 56;
            Item.width = 38;
            Item.height = 46;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        private Item GiveItemToSlotOrInventory(Player player, int itemType, Item[] slotArray, int slotIndex, out string key)
        {
            Item item = new Item();
            item.SetDefaults(itemType);
            key = UniqueItemHelper.StampItem(item);
            if (slotArray != null && slotArray[slotIndex].IsAir)
            {
                slotArray[slotIndex] = item;
                return item;
            }
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].IsAir)
                {
                    player.inventory[i] = item;
                    return item;
                }
            }
            player.QuickSpawnItem(player.GetSource_FromThis(), item.type);
            return null;
        }

        public override void OnEquip(Player player)
        {
            _waterKatana = GiveItemToSlotOrInventory(player, ModContent.ItemType<WaterKatanaFinal>(), null, 0, out _katanaKey);
            _waterHook = GiveItemToSlotOrInventory(player, ModContent.ItemType<WaterHook>(), player.miscEquips, 4, out _hookKey);
        }

        public override void OnUnequip(Player player)
        {
            TryRemoveTrackedItem(player, ref _waterKatana, ref _katanaKey);
            TryRemoveTrackedItem(player, ref _waterHook, ref _hookKey);
        }

        private void TryRemoveTrackedItem(Player player, ref Item trackedItem, ref string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (trackedItem == null || trackedItem.IsAir)
                trackedItem = UniqueItemHelper.FindItem(player, key);
            trackedItem?.TurnToAir();
            trackedItem = null;
            key = "";
        }

        public override bool ManualStandSpawning(Player player)
        {
            player.GetModPlayer<FanPlayer>().SpawnFanStand();
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<HolyDiverT3>())
                .AddIngredient(ItemID.SharkFin, 12)
                .AddIngredient(ItemID.Ectoplasm, 15)
                .AddIngredient(ModContent.ItemType<RighteousLifeforce>())
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["katanaKey"] = _katanaKey;
            tag["hookKey"] = _hookKey;
        }

        public override void LoadData(TagCompound tag)
        {
            _katanaKey = tag.GetString("katanaKey");
            _hookKey = tag.GetString("hookKey");
        }
    }
}