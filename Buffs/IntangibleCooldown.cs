using System;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Buffs
{
    public class IntangibleCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
			DisplayName.SetDefault("Intangible Cooldown");
            Description.SetDefault("You got sick of going through everything.");
            Main.debuff[Type] = true;
        }
    }
}