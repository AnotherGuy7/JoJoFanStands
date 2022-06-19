using JoJoFanStands.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Buffs
{
    public class RoseColoredSunBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rose Colored Sun");
            Description.SetDefault("A Rose Colored Sun will burn all your enemies away!");
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<RoseColoredSun>()] == 0)
                Projectile.NewProjectile(player.GetSource_FromThis(), player.position, Vector2.Zero, ProjectileType<RoseColoredSun>(), 999, 2f, Main.myPlayer);

            player.GetModPlayer<FanPlayer>().RoseColoredSunActive = true;
        }
    }
}