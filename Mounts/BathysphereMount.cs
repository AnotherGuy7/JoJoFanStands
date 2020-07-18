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
            mountData.heightBoost = 0;      //the height the mount gains
            mountData.fallDamage = 0.5f;
            mountData.runSpeed = 7f;
            mountData.dashSpeed = 4f;
            mountData.flightTimeMax = 0;
            mountData.fatigueMax = 0;
            mountData.jumpHeight = 5;
            mountData.acceleration = 0.08f;
            mountData.jumpSpeed = 5f;
            mountData.blockExtraJumps = false;
            mountData.totalFrames = 2;
            mountData.constantJump = true;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 0;
            }
            mountData.playerYOffsets = array;
            mountData.xOffset = -10;
            mountData.bodyFrame = 3;
            mountData.yOffset = 0;        //mount Y draw offset
            mountData.playerHeadOffset = 0;     //player head in the map offset
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
            }
            player.breathCD = 1;
            player.accFlipper = player.wet;
        }
    }
}