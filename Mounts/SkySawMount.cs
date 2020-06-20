using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Mounts
{
    public class SkySawMount : ModMountData       //ExampleMod'
    {
        public override void SetDefaults()
        {
            mountData.fallDamage = 0.5f;
            mountData.runSpeed = 11f;
            mountData.dashSpeed = 8f;
            mountData.flightTimeMax = 999999999;
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
            mountData.xOffset = 0;
            mountData.bodyFrame = 3;
            mountData.yOffset = -22;
            mountData.playerHeadOffset = -22;
            mountData.standingFrameCount = 0;
            mountData.standingFrameDelay = 12;
            mountData.standingFrameStart = 0;
            mountData.runningFrameCount = 0;
            mountData.runningFrameDelay = 12;
            mountData.runningFrameStart = 0;
            mountData.flyingFrameCount = 2;
            mountData.flyingFrameDelay = 12;
            mountData.flyingFrameStart = 0;
            mountData.inAirFrameCount = 0;
            mountData.inAirFrameDelay = 12;
            mountData.inAirFrameStart = 0;
            mountData.idleFrameCount = 0;
            mountData.idleFrameDelay = 12;
            mountData.idleFrameStart = 0;
            mountData.idleFrameLoop = true;
            mountData.swimFrameCount = mountData.inAirFrameCount;
            mountData.swimFrameDelay = mountData.inAirFrameDelay;
            mountData.swimFrameStart = mountData.inAirFrameStart;
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            mountData.textureWidth = 50;
            mountData.textureHeight = mountData.backTexture.Height;
        }

        public override void UpdateEffects(Player player)
        {
            player.maxFallSpeed = 1f;
        }
    }
}