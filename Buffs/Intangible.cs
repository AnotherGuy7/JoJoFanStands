using System;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Buffs
{
    public class Intangible : ModBuff
    {
        public override void SetDefaults()
        {
			DisplayName.SetDefault("Intangible!");
            Description.SetDefault("Everything goes through you and you go through everything");
        }
 
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HasBuff(mod.BuffType(Name)))
            {
                if (player.controlDown)
                {
                    player.position.Y += 1f;
                }
                if (Collision.SolidTiles((int)player.position.X / 16, (int)player.position.X / 16, (int)player.position.Y/ 16, (int)player.position.Y/ 16))       //or convert player.position into a tile coordinate then check 1 to the right and 2 to the bottom, 2x3 = 6 total checks, the suggestion by direwolf
                {
                    player.lifeRegen -= 4;
                    if (player.controlLeft)
                    {
                        player.position.X -= 1f;
                    }
                    if (player.controlRight)
                    {
                        player.position.X += 1f;
                    }
                    if (player.controlJump)
                    {
                        player.position.Y -= 1f;
                    }
                }
            }
            else
            {
                player.AddBuff(JoJoFanStands.JoJoStandsMod.BuffType("AbilityCooldown"), 1800);
            }
        }
    }
}