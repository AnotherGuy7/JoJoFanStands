using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Dusts
{
    public class ChargedShotTrail : ModDust
    {
        private const int FrameWidth = 24;
        private const int FrameHeight = 14;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, Main.rand.Next(0, 3 + 1) * FrameHeight, FrameWidth, FrameHeight);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.98f;
            dust.alpha += 4;
            if (dust.alpha >= 255)
                dust.active = false;

            dust.position += dust.velocity;
            return false;
        }
    }
}