using System;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Buffs
{
    public class SoreEye : ModBuff
    {
        public override void SetDefaults()
        {
			DisplayName.SetDefault("Sore Eye");
            Description.SetDefault("Your eyes are in pain.");
            Main.debuff[Type] = true;
        }
    }
}