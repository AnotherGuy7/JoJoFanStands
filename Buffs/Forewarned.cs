using System;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Buffs
{
    public class Forewarned : ModBuff
    {
        public override void SetDefaults()
        {
			DisplayName.SetDefault("Forwarned");
            Description.SetDefault("You've already seen everything in the next 3 seconds...");
        }
 
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HasBuff(mod.BuffType(Name)))
            {}
            else
            {
                player.AddBuff(mod.BuffType("SoreEye"), 1800);
            }
        }
    }
}