using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Mounts
{
    public class BrianEnoMount : ModMountData       //ExampleMod'
    {
        public override void SetDefaults()
        {
            mountData.heightBoost = -12;
            mountData.fallDamage = 0.5f;
            mountData.runSpeed = 11f;
            mountData.dashSpeed = 8f;
            mountData.flightTimeMax = 0;
            mountData.fatigueMax = 0;
            mountData.jumpHeight = 5;
            mountData.acceleration = 0.19f;
            mountData.jumpSpeed = 4f;
            mountData.blockExtraJumps = false;
            mountData.totalFrames = 1;
            mountData.constantJump = true;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 20;
            }
            mountData.playerYOffsets = array;
            mountData.xOffset = -13;
            mountData.bodyFrame = 3;
            mountData.yOffset = -12;
            mountData.playerHeadOffset = 22;
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            mountData.textureWidth = mountData.backTexture.Width;
            mountData.textureHeight = mountData.backTexture.Height / mountData.totalFrames;
        }

        public override void UpdateEffects(Player player)
        {
            if (player.GetModPlayer<FanPlayer>().SpinBoost)
            {
                
            }
        }
    }
}