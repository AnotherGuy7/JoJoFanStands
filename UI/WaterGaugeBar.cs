using JoJoFanStands.Projectiles.PlayerStands.HolyDiver;
using JoJoStands.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace JoJoFanStands.UI
{
    public class WaterGaugeBar : UIState
    {
        public static bool Visible;
        public static Texture2D WaterGaugeBarTexture;
        public static Texture2D WaterGaugeFillTexture;

        public static void ShowWaterGaugeBar() => Visible = true;
        public static void HideWaterGaugeBar() => Visible = false;

        public DragableUIPanel waterBar;
        public UIText waterEnergyText;

        private const int BarWidth = 32;
        private const int BarHeight = 64;
        private const int DisplayWidth = 64;
        private const int DisplayHeight = 128;

        public override void OnInitialize()
        {
            waterBar = new DragableUIPanel();
            waterBar.Left.Set(Main.screenWidth * 0.90f, 0f);
            waterBar.Top.Set(Main.screenHeight * 0.88f, 0f);
            waterBar.Width.Set(DisplayWidth, 0f);
            waterBar.Height.Set(DisplayHeight, 0f);
            waterBar.BackgroundColor = new Color(0, 0, 0, 0);
            waterBar.BorderColor = new Color(0, 0, 0, 0);

            waterEnergyText = new UIText("100%");
            waterEnergyText.HAlign = 0.5f;
            waterEnergyText.VAlign = 1.4f;
            waterEnergyText.Width.Set(12f * 4f, 0f);
            waterEnergyText.Height.Set(20, 0f);
            waterBar.Append(waterEnergyText);

            Append(waterBar);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Player player = Main.player[Main.myPlayer];
            WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();

            int pct = WaterGaugePlayer.MaxWater > 0
                ? (int)((float)wgp.CurrentWater / WaterGaugePlayer.MaxWater * 100f)
                : 0;

            waterEnergyText.SetText(pct + "%");
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Player player = Main.player[Main.myPlayer];
            WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();
            Rectangle dest = UITools.ReformatRectangle(waterBar.GetClippingRectangle(spriteBatch));

            float fillRatio = WaterGaugePlayer.MaxWater > 0
                ? (float)wgp.CurrentWater / WaterGaugePlayer.MaxWater
                : 0f;
            int fillHeight = (int)(dest.Height * fillRatio);
            int emptyHeight = dest.Height - fillHeight;

            int srcEmptyHeight = (int)(emptyHeight * ((float)BarHeight / dest.Height));
            int srcFillHeight = BarHeight - srcEmptyHeight;

            if (WaterGaugeFillTexture != null && fillHeight > 0)
            {
                Rectangle fillSrc = new Rectangle(0, srcEmptyHeight, BarWidth, srcFillHeight);
                Rectangle fillDest = new Rectangle(dest.X, dest.Y + emptyHeight, dest.Width, fillHeight);
                spriteBatch.Draw(WaterGaugeFillTexture, fillDest, fillSrc, Color.White);
            }

            if (WaterGaugeBarTexture != null)
                spriteBatch.Draw(WaterGaugeBarTexture, dest, new Rectangle(0, 0, BarWidth, BarHeight), Color.White);
        }
    }
}