using JoJoFanStands.Items.Armor;
using JoJoFanStands.Items.Stands;
using JoJoFanStands.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.UI;

namespace JoJoFanStands
{
    public class JoJoFanStands : Mod
    {
        public static Mod JoJoStandsMod;
        public static JoJoFanStands Instance;

        public override void Load()
        {
            Instance = ModContent.GetInstance<JoJoFanStands>();
            JoJoStandsMod = ModLoader.GetMod("JoJoStands");

            if (!Main.dedServ)
            {
                //Shader stuff
                Ref<Effect> distortedReality = new Ref<Effect>(ModContent.Request<Effect>("JoJoFanStands/Effects/MonotoneRealityShader", AssetRequestMode.ImmediateLoad).Value);
                Filters.Scene["MonotoneRealityEffect"] = new Filter(new ScreenShaderData(distortedReality, "MonotoneRealityEffect"), EffectPriority.VeryHigh);
                Filters.Scene["MonotoneRealityEffect"].Load();
            }

            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<CoolOutT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<FollowMeT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<MortalReminderT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<SlavesOfFearT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<TheFatesT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<BanksT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<BrianEnoAct1>());
        }

        public override void Unload()
        {
            JoJoStandsMod = null;
            Instance = null;
        }
    }
}