using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Mounts
{
    public class BrianEnoMount : ModMount       //ExampleMod'
    {
        public override void SetStaticDefaults()
        {
            MountData.heightBoost = -12;
            MountData.fallDamage = 0.5f;
            MountData.runSpeed = 11f;
            MountData.dashSpeed = 8f;
            MountData.flightTimeMax = 0;
            MountData.fatigueMax = 0;
            MountData.jumpHeight = 5;
            MountData.acceleration = 0.19f;
            MountData.jumpSpeed = 4f;
            MountData.blockExtraJumps = false;
            MountData.totalFrames = 1;
            MountData.constantJump = true;
            int[] array = new int[MountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 20;
            }
            MountData.playerYOffsets = array;
            MountData.xOffset = -13;
            MountData.bodyFrame = 3;
            MountData.yOffset = -12;
            MountData.playerHeadOffset = 22;
            if (Main.netMode == NetmodeID.Server)
                return;

            MountData.textureWidth = 38;
            MountData.textureHeight = 54;
        }
    }
}