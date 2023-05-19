using System.IO;

namespace JoJoFanStands.Networking
{
	public class ModNetHandler
	{
		public const byte Effect = 0;

		public static readonly EffectPacketHandler EffectSync = new EffectPacketHandler(Effect);

		public static void HandlePacket(BinaryReader reader, int fromWho)
		{
			byte messageType = reader.ReadByte();
			switch (messageType)
			{
				case Effect:
					EffectSync.HandlePacket(reader, fromWho);
					break;
			}
		}
	}
}