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

        private Item _waterKatana;

        public override void SetStaticDefaults() { }

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

        public override void OnEquip(Player player)
        {
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].IsAir)
                {
                    _waterKatana = new Item();
                    _waterKatana.SetDefaults(ModContent.ItemType<WaterKatanaFinal>());
                    player.inventory[i] = _waterKatana;
                    return;
                }
            }
            player.QuickSpawnItem(player.GetSource_FromThis(), ModContent.ItemType<WaterKatanaFinal>());
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (!player.armor[HookSlotIndex].IsAir)
            {
                hPlayer.boostingExistingHook = true;
                _waterHook = null;
            }
            else
            {
                _waterHook = new Item();
                _waterHook.SetDefaults(ModContent.ItemType<WaterHook>());
                player.armor[HookSlotIndex] = _waterHook;
            }
        }

        public override void OnUnequip(Player player)
        {
            if (_waterKatana == null)
                return;
            _waterKatana.TurnToAir();
            _waterKatana = null;
            HolyDiverPlayer hPlayer = player.GetModPlayer<HolyDiverPlayer>();
            hPlayer.boostingExistingHook = false;

            if (_waterHook == null)
            {
                return;
            }
            Item currentHookSlot = player.armor[HookSlotIndex];
            if (currentHookSlot == _waterHook)
                _waterHook.TurnToAir();
            else if (currentHookSlot.IsAir)
            {

            }
            else
                _waterHook.TurnToAir();
            _waterHook = null;
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