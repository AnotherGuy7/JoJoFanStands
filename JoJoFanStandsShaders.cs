using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;


namespace JoJoFanStands
{
    public class JoJoFanStandsShaders
    {
        public const string MonotoneRealityEffect = "MonotoneRealityEffect";
        public const string CircularGreyscale = "CircularGreyscale";
        public const string BackInBlackDistortion = "BackInBlackDistortion";

        public static void LoadShaders()
        {
            //Shader stuff
            Ref<Effect> distortedReality = new Ref<Effect>(ModContent.Request<Effect>("JoJoFanStands/Effects/MonotoneRealityShader", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene[MonotoneRealityEffect] = new Filter(new ScreenShaderData(distortedReality, "MonotoneRealityEffect"), EffectPriority.VeryHigh);
            Filters.Scene[MonotoneRealityEffect].Load();

            Ref<Effect> circularGreyscale = new Ref<Effect>(ModContent.Request<Effect>("JoJoFanStands/Effects/CircularGreyscale", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene[CircularGreyscale] = new Filter(new ScreenShaderData(circularGreyscale, "CircularGreyscale"), EffectPriority.VeryHigh);
            Filters.Scene[CircularGreyscale].Load();

            Ref<Effect> backInBlackDistortion = new Ref<Effect>((Effect)ModContent.Request<Effect>("JoJoFanStands/Effects/BackInBlackDistortion", AssetRequestMode.ImmediateLoad));
            GameShaders.Misc[BackInBlackDistortion] = new MiscShaderData(backInBlackDistortion, "BackInBlackDistortionEffect");
        }
    }
}
