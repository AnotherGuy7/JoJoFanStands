using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Mounts
{
    public class BathysphereMount : ModMount       //ExampleMod'
    {
        public override void SetStaticDefaults()
        {
            MountData.heightBoost = 0;      //the height the mount gains
            MountData.fallDamage = 0.5f;
            MountData.runSpeed = 7f;
            MountData.dashSpeed = 4f;
            MountData.flightTimeMax = 0;
            MountData.fatigueMax = 0;
            MountData.jumpHeight = 5;
            MountData.acceleration = 0.08f;
            MountData.jumpSpeed = 5f;
            MountData.blockExtraJumps = false;
            MountData.totalFrames = 2;
            MountData.constantJump = true;
            int[] array = new int[MountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 0;
            }
            MountData.playerYOffsets = array;
            MountData.xOffset = -10;
            MountData.bodyFrame = 3;
            MountData.yOffset = 0;        //mount Y draw offset
            MountData.playerHeadOffset = 0;     //player head in the map offset
            MountData.swimFrameCount = 2;
            MountData.swimFrameDelay = 12;
            MountData.swimFrameStart = 0;
            if (Main.netMode == NetmodeID.Server)
                return;

            MountData.textureWidth = 46;
            MountData.textureHeight = 50;
        }

        public override void UpdateEffects(Player player)
        {
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.breathCD = 1;
            player.accFlipper = player.wet;
            if (!player.wet)
                player.moveSpeed = 0f;
        }
    }
}