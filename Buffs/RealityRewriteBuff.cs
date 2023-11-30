using JoJoStands.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Buffs
{
    public class RealityRewriteBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Brian Eno Active");
            // Description.SetDefault("Brian Eno is active!");
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (fPlayer.realityRewriteActive && !player.HasBuff<AbilityCooldown>())
                player.buffTime[buffIndex] = 2;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.Next(0, 100 + 1) <= 6)
            {
                int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, Main.rand.Next(0, 1 + 1) == 0 ? DustID.IchorTorch : DustID.WhiteTorch, Scale: 1.6f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].fadeIn = 2f;
                if (Main.rand.Next(0, 7 + 1) != 0)
                    Main.dust[dustIndex].noLight = true;
            }
        }
    }
}