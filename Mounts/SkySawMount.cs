using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Mounts
{
    public class SkySawMount : ModMount       //ExampleMod'
    {
        public override void SetStaticDefaults()
        {
            MountData.fallDamage = 0.5f;
            MountData.runSpeed = 11f;
            MountData.dashSpeed = 8f;
            MountData.flightTimeMax = 999999999;
            MountData.fatigueMax = 0;
            MountData.jumpHeight = 5;
            MountData.acceleration = 0.19f;
            MountData.jumpSpeed = 4f;
            MountData.blockExtraJumps = false;
            MountData.totalFrames = 2;
            MountData.constantJump = true;
            int[] array = new int[MountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 0;
            }
            MountData.playerYOffsets = array;
            MountData.xOffset = 0;
            MountData.bodyFrame = 3;
            MountData.yOffset = -22;
            MountData.playerHeadOffset = 2;
            MountData.standingFrameCount = 0;
            MountData.standingFrameDelay = 15;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 0;
            MountData.runningFrameDelay = 12;
            MountData.runningFrameStart = 0;
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 12;
            MountData.flyingFrameStart = 0;
            MountData.inAirFrameCount = 0;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 0;
            MountData.idleFrameCount = 2;
            MountData.idleFrameDelay = 10;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            if (Main.netMode == NetmodeID.Server)
                return;

            MountData.textureWidth = 50;
            MountData.textureHeight = 90;
        }

        public override void UpdateEffects(Player player)
        {
            player.maxFallSpeed = 1f;
        }

        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            mountedPlayer.mount._frameCounter++;
            if (mountedPlayer.mount._frameCounter >= 15)
            {
                mountedPlayer.mount._frame++;
                mountedPlayer.mount._frameCounter = 0;
                if (mountedPlayer.mount._frame >= 2)
                    mountedPlayer.mount._frame = 0;
            }

            return false;
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            frame = new Rectangle(0, drawPlayer.mount._frame * 90, 50, 90);
            drawOrigin = new Vector2(30, 45);
            return true;
        }
    }
}