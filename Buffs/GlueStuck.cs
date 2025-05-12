using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Buffs
{
    public class GlueStuck : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sore Eye");
            // Description.SetDefault("Your eyes are in pain.");
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            for (int i = 0; i < 4; i++)
            {
                int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Pearlsand, Alpha: 100, Scale: 1.5f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 0.5f;
            }
        }
    }
}