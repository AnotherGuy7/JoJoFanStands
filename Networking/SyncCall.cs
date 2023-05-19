using Terraria;
using Terraria.ID;

namespace JoJoFanStands.Networking
{
    public class SyncCall
    {
        public static void SyncBlurFastReflexes(int whoAmI, bool blurFastReflexesValue)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModNetHandler.EffectSync.SendBlurFastReflexes(256, whoAmI, blurFastReflexesValue);
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player otherPlayer = Main.player[p];
                    if (otherPlayer.active)
                        otherPlayer.GetModPlayer<FanPlayer>().blurLightningFastReflexes = blurFastReflexesValue;
                }
            }
        }

        public static void SyncBlurInfiniteVelocity(int whoAmI, bool blurInfiniteVelocity)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModNetHandler.EffectSync.SendBlurFastReflexes(256, whoAmI, blurInfiniteVelocity);
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player otherPlayer = Main.player[p];
                    if (otherPlayer.active)
                        otherPlayer.GetModPlayer<FanPlayer>().blurInfiniteVelocity = blurInfiniteVelocity;
                }
            }
        }
    }
}
