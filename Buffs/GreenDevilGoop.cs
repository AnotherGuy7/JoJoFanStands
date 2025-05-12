using JoJoStands;
using JoJoStands.Buffs;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Buffs
{
    public class GreenDevilGoop : JoJoBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Brian Eno Active");
            // Description.SetDefault("Brian Eno is active!");
            Main.buffNoTimeDisplay[Type] = true;
        }

        private float savedVelocityX = -1f;

        public override void OnApply(NPC npc)
        {
            savedVelocityX = Math.Abs(npc.velocity.X) / GetDebuffOwnerModPlayer(npc).standTier;
        }

        public override void UpdateBuffOnNPC(NPC npc)
        {
            Player player = GetDebuffOwner(npc);
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;

            if (Math.Abs(npc.velocity.X) > savedVelocityX)
                npc.velocity.X *= 0.9f;

            npc.lifeRegenExpectedLossPerSecond = 10;
            npc.lifeRegen -= 20;     //losing 10 health
            if (Main.rand.Next(0, 2) == 0)
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.GreenBlood, npc.velocity.X * -0.5f, npc.velocity.Y * -0.5f);
        }
    }
}