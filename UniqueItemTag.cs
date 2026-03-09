using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace JoJoFanStands
{
    public class UniqueItemTag : GlobalItem
    {
        public string UniqueKey = "";

        public override bool InstancePerEntity => true;

        public override void SaveData(Item item, TagCompound tag)
        {
            if (!string.IsNullOrEmpty(UniqueKey))
                tag["uniqueKey"] = UniqueKey;
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            if (tag.ContainsKey("uniqueKey"))
                UniqueKey = tag.GetString("uniqueKey");
        }
    }
}