using JoJoFanStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Shaders;
using Terraria.UI;

namespace JoJoStands.UI
{
    public class SoulBar : UIState
    {
        public static bool Visible;
        public static int sizeMode;

        public DragableUIPanel soulBar;
        public UIText soulPointsText;
        public static Texture2D soulBarTexture;
        public static Texture2D soulBarBarTexture;
        public static bool changedInConfig = false;

        private readonly Point EnergyBarStartPosition = new Point(3, 21);
        private readonly Vector2 EnergyBarOrigin = new Vector2(0, 3); 

        public static void ShowSoulBar()
        {
            Visible = true;
        }

        public static void HideSoulBar()
        {
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Main.player[Main.myPlayer];
            FanPlayer fanPlayer = player.GetModPlayer<FanPlayer>();
            base.Update(gameTime);

            soulPointsText.SetText(fanPlayer.metempsychosisPoints + " / 100");
        }

        public override void OnInitialize()
        {
            soulBar = new DragableUIPanel();
            soulBar.Left.Set(Main.screenWidth * 0.95f, 0f);
            soulBar.Top.Set(Main.screenHeight * 0.95f, 0f);
            soulBar.Width.Set(42, 0f);
            soulBar.Height.Set(72, 0f);
            soulBar.BackgroundColor = new Color(0, 0, 0, 0);       //make it invisible so that the image is there itself
            soulBar.BorderColor = new Color(0, 0, 0, 0);

            soulPointsText = new UIText(0 + " / 100");
            soulPointsText.HAlign = 0.5f;
            soulPointsText.VAlign = 1.4f;
            soulPointsText.Width.Set(12f * 4f, 0f);
            soulPointsText.Height.Set(20, 0f);
            soulBar.Append(soulPointsText);

            Append(soulBar);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)       //from ExampleMod's ExampleUI
        {
            Player player = Main.player[Main.myPlayer];
            FanPlayer fanPlayer = player.GetModPlayer<FanPlayer>();

            spriteBatch.Draw(soulBarTexture, UITools.ReformatRectangle(soulBar.GetClippingRectangle(spriteBatch)), new Rectangle(0, 0, 42, 72), Color.White);

            Rectangle mainUIDestinationRect = UITools.ReformatRectangle(soulBar.GetClippingRectangle(spriteBatch));
            MiscShaderData soulBarShader = JoJoStandsShaders.GetShaderInstance(JoJoFanStandsShaders.SoulBarShader);
            float gradientValue = ((60f / 72f) * (fanPlayer.metempsychosisPoints / 100)) + (12f / 72f);
            soulBarShader.UseOpacity(gradientValue);
            UITools.DrawUIWithShader(spriteBatch, soulBarShader, soulBarBarTexture, mainUIDestinationRect);
        }
    }
}