using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace JoJoFanStands
{
    public class CustomizableOptions : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        [Tooltip("Determines whether or not you want an NPC called 'Pao' to spawn.")]
        public bool PaoSpawning;

        public override void OnChanged()        //couldn't use Player player = Main.LocalPlayer cause it wasn't set to an instance of an object
        {
            FanPlayer.SpawnPao = PaoSpawning;
        }
    }
}