using System;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Buffs
{
    public class RoseColoredSunBuff : ModBuff
    {
        public override void SetDefaults()
        {
			DisplayName.SetDefault("Rose Colored Sun");
            Description.SetDefault("A Rose Colored Sun will burn all your enemies away!");
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
 
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[mod.ProjectileType("RoseColoredSun")] == 0)
            {
                Projectile.NewProjectile(player.position, Vector2.Zero, mod.ProjectileType("RoseColoredSun"), 999, 2f, Main.myPlayer);
            }
            player.GetModPlayer<FanPlayer>().RoseColoredSunActive = true;
        }
    }
}