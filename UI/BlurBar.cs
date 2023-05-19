using JoJoFanStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace JoJoStands.UI
{
    public class BlurBar : UIState
    {
        public static bool Visible;
        public static int sizeMode;

        public DragableUIPanel blurBar;
        public UIText blurEnergyText;
        public static Texture2D blurBarTexture;
        public static Texture2D blurEnergyPointTexture;
        public static bool changedInConfig = false;

        private readonly Point EnergyBarStartPosition = new Point(3, 21);
        private readonly Vector2 EnergyBarOrigin = new Vector2(0, 3); 

        public static void ShowBlurBar()
        {
            Visible = true;
        }

        public static void HideBlurBar()
        {
            Visible = false;
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Main.player[Main.myPlayer];
            FanPlayer fanPlayer = player.GetModPlayer<FanPlayer>();
            base.Update(gameTime);

            blurEnergyText.SetText(fanPlayer.amountOfBlurEnergy + "%");
        }

        public override void OnInitialize()
        {
            blurBar = new DragableUIPanel();
            blurBar.Left.Set(Main.screenWidth * 0.95f, 0f);
            blurBar.Top.Set(Main.screenHeight * 0.95f, 0f);
            blurBar.Width.Set(48, 0f);
            blurBar.Height.Set(100, 0f);
            blurBar.BackgroundColor = new Color(0, 0, 0, 0);       //make it invisible so that the image is there itself
            blurBar.BorderColor = new Color(0, 0, 0, 0);

            blurEnergyText = new UIText(0 + "%");
            blurEnergyText.HAlign = 0.5f;
            blurEnergyText.VAlign = 1.4f;
            blurEnergyText.Width.Set(12f * 4f, 0f);
            blurEnergyText.Height.Set(20, 0f);
            blurBar.Append(blurEnergyText);

            Append(blurBar);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)       //from ExampleMod's ExampleUI
        {
            Player player = Main.player[Main.myPlayer];
            FanPlayer fanPlayer = player.GetModPlayer<FanPlayer>();

            spriteBatch.Draw(blurBarTexture, UITools.ReformatRectangle(blurBar.GetClippingRectangle(spriteBatch)), new Rectangle(0, (fanPlayer.amountOfBlurEnergy / 20) * 100, 48, 100), Color.White);
        }
    }
}