using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Networking
{
    public class EffectPacketHandler : PacketHandler
    {
        public const byte BlurFastReflexesSync = 0;
        public const byte BlurInfiniteVelocitySync = 1;

        public EffectPacketHandler(byte handlerType) : base(handlerType)
        { }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            byte messageType = reader.ReadByte();
            switch (messageType)
            {
                case BlurFastReflexesSync:
                    ReceiveBlurFastReflexes(reader, fromWho);
                    break;
                case BlurInfiniteVelocitySync:
                    ReceiveBlurInfiniteVelocity(reader, fromWho);
                    break;
            }
        }

        public void SendBlurFastReflexes(int toWho, int fromWho, bool blurFastReflexesValue)
        {
            ModPacket packet = CreatePacket(BlurFastReflexesSync);
            packet.Write(blurFastReflexesValue);
            packet.Send(toWho, fromWho);
        }

        public void ReceiveBlurFastReflexes(BinaryReader reader, int fromWho)
        {
            bool blurFastReflexesValue = reader.ReadBoolean();
            if (Main.netMode != NetmodeID.Server)
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player otherPlayer = Main.player[p];
                    if (otherPlayer.active)
                        otherPlayer.GetModPlayer<FanPlayer>().blurLightningFastReflexes = blurFastReflexesValue;
                }
            }
            else
            {
                SendBlurFastReflexes(-1, fromWho, blurFastReflexesValue);
            }
        }

        public void SendBlurInfiniteVelocity(int toWho, int fromWho, bool blurInfiniteVelocityValue)
        {
            ModPacket packet = CreatePacket(BlurInfiniteVelocitySync);
            packet.Write(blurInfiniteVelocityValue);
            packet.Send(toWho, fromWho);
        }

        public void ReceiveBlurInfiniteVelocity(BinaryReader reader, int fromWho)
        {
            bool blurInfiniteVelocityValue = reader.ReadBoolean();
            if (Main.netMode != NetmodeID.Server)
            {
                for (int p = 0; p < Main.maxPlayers; p++)
                {
                    Player otherPlayer = Main.player[p];
                    if (otherPlayer.active)
                        otherPlayer.GetModPlayer<FanPlayer>().blurInfiniteVelocity = blurInfiniteVelocityValue;
                }
            }
            else
            {
                SendBlurInfiniteVelocity(-1, fromWho, blurInfiniteVelocityValue);
            }
        }
    }
}