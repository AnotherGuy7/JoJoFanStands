using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands
{
    public static class UniqueItemHelper
    {
        public static string StampItem(Item item)
        {
            string key = Guid.NewGuid().ToString();
            item.GetGlobalItem<UniqueItemTag>().UniqueKey = key;
            return key;
        }

        public static Item FindItem(Player player, string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            foreach (Item item in player.inventory)
                if (!item.IsAir && item.GetGlobalItem<UniqueItemTag>().UniqueKey == key)
                    return item;
            foreach (Item item in player.miscEquips)
                if (!item.IsAir && item.GetGlobalItem<UniqueItemTag>().UniqueKey == key)
                    return item;
            Item[][] banks = { player.bank.item, player.bank2.item, player.bank3.item, player.bank4.item };
            foreach (Item[] bank in banks)
                foreach (Item item in bank)
                    if (!item.IsAir && item.GetGlobalItem<UniqueItemTag>().UniqueKey == key)
                        return item;
            foreach (Chest chest in Main.chest)
            {
                if (chest == null) continue;
                foreach (Item item in chest.item)
                    if (!item.IsAir && item.GetGlobalItem<UniqueItemTag>().UniqueKey == key)
                        return item;
            }
            return null;
        }
    }
}