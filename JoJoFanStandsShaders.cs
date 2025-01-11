using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
        public const string AuraShader = "AuraShader";
        public const string SoulBarShader = "SoulBarShader";

        public static void LoadShaders()
        {
            //Shader stuff
            Asset<Effect> distortedReality = ModContent.Request<Effect>("JoJoFanStands/Effects/MonotoneRealityShader", AssetRequestMode.ImmediateLoad);
            Filters.Scene[MonotoneRealityEffect] = new Filter(new ScreenShaderData(distortedReality, "MonotoneRealityEffect"), EffectPriority.VeryHigh);
            Filters.Scene[MonotoneRealityEffect].Load();

            Asset<Effect> circularGreyscale = ModContent.Request<Effect>("JoJoFanStands/Effects/CircularGreyscale", AssetRequestMode.ImmediateLoad);
            Filters.Scene[CircularGreyscale] = new Filter(new ScreenShaderData(circularGreyscale, "CircularGreyscale"), EffectPriority.VeryHigh);
            Filters.Scene[CircularGreyscale].Load();

            Asset<Effect> backInBlackDistortion = ModContent.Request<Effect>("JoJoFanStands/Effects/BackInBlackDistortion", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc[BackInBlackDistortion] = new MiscShaderData(backInBlackDistortion, "BackInBlackDistortionEffect");

            Asset<Effect> auraShader = ModContent.Request<Effect>("JoJoFanStands/Effects/AuraShader", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc[AuraShader] = new MiscShaderData(auraShader, "AuraShaderEffect");
            GameShaders.Misc[AuraShader].UseImage1(ModContent.Request<Texture2D>("JoJoFanStands/Extras/Noise", AssetRequestMode.ImmediateLoad));

            Asset<Effect> soulBarShader = ModContent.Request<Effect>("JoJoFanStands/Effects/SoulBarShader", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc[SoulBarShader] = new MiscShaderData(soulBarShader, "SoulBarEffect");
        }
    }
}
