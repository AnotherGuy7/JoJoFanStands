using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Mounts
{
    public class BathysphereMount : ModMountData       //ExampleMod'
    {
        public override void SetDefaults()
        {
            mountData.heightBoost = -15;
            mountData.fallDamage = 0.5f;
            mountData.runSpeed = 11f;
            mountData.dashSpeed = 8f;
            mountData.flightTimeMax = 0;
            mountData.fatigueMax = 0;
            mountData.jumpHeight = 5;
            mountData.acceleration = 0.19f;
            mountData.jumpSpeed = 4f;
            mountData.blockExtraJumps = false;
            mountData.totalFrames = 2;
            mountData.constantJump = true;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 20;
            }
            mountData.playerYOffsets = array;
            mountData.xOffset = -10;
            mountData.bodyFrame = 3;
            mountData.yOffset = -32;
            mountData.playerHeadOffset = 22;
            mountData.swimFrameCount = 2;
            mountData.swimFrameDelay = 12;
            mountData.swimFrameStart = 0;
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            mountData.textureWidth = mountData.backTexture.Width;
            mountData.textureHeight = mountData.backTexture.Height;
        }

        public override void UpdateEffects(Player player)
        {
            player.controlUseItem = false;
            player.controlUseTile = false;
            if (!player.wet)
            {
                player.moveSpeed = 0f;
                mountData.acceleration = 0f;
                mountData.runSpeed = 0f;
                mountData.dashSpeed = 0f;
                player.accFlipper = false;
            }
            player.breathCD = 1;
            player.accFlipper = true;
        }
    }
}