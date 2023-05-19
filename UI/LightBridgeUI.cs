using JoJoFanStands.Projectiles.PlayerStands.LucyInTheSky;
using JoJoStands.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace JoJoFanStands.UI
{
    public class LightBridgeUI : UIState
    {
        public static bool Visible;
        public DragableUIPanel lightBridgeUI;

        private static AdjustableButton[] markerButtons;
        private Texture2D buttonOverlayImage;

        public static void ShowLightBridgeUI(int amountOfButtons)
        {
            for (int i = 0; i < amountOfButtons; i++)
            {
                markerButtons[i].invisible = false;
            }
            Visible = true;
        }

        public static void HideLightBridgeUI()
        {
            for (int i = 0; i < 12; i++)
            {
                markerButtons[i].invisible = true;
            }
            Visible = false;
        }

        public override void OnInitialize()
        {
            lightBridgeUI = new DragableUIPanel();
            lightBridgeUI.Width.Pixels = 240;
            lightBridgeUI.Height.Pixels = 240;
            lightBridgeUI.HAlign = 0.9f;
            lightBridgeUI.VAlign = 0.9f;

            int amountOfRows = 3;
            int amountOfColumns = 4;
            Vector2 buttonStartPos = new Vector2(16, 16);
            markerButtons = new AdjustableButton[12];
            for (int i = 0; i < 12; i++)
            {
                float x = i * 16f;
                float y = (i / amountOfColumns) * 16f;
                markerButtons[i] = new AdjustableButton(ModContent.Request<Texture2D>("JoJoFanStands/UI/InnerPanelBackground"), buttonStartPos + new Vector2(x, y), new Vector2(24f), Color.White, false);
                markerButtons[i].OnLeftClick += OnClickLightBridgeButton;
                markerButtons[i].invisible = true;
                lightBridgeUI.Append(markerButtons[i]);
            }

            Append(lightBridgeUI);
        }

        private void OnClickLightBridgeButton(UIMouseEvent evt, UIElement listeningElement)
        {
            Player player = Main.LocalPlayer;
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            for (int i = 0; i < 12; i++)
            {
                if (listeningElement == markerButtons[i])
                {
                    fPlayer.lucySelectedMarkerWhoAmI = i;
                    return;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Main.LocalPlayer;
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (buttonOverlayImage == null)
                buttonOverlayImage = ModContent.Request<Texture2D>("JoJoFanStands/UI/LightMarkerOverlayImage").Value;

            int amountOfLightMarkers = player.ownedProjectileCounts[ModContent.ProjectileType<LightMarker>()];
            for (int i = 0; i < 12; i++)
            {
                if (i < amountOfLightMarkers)
                {
                    markerButtons[i].SetOverlayImage(buttonOverlayImage, 0f);
                    markerButtons[i].drawColor = Color.White;
                }
                else
                    markerButtons[i].drawColor = Color.White * 0.8f;
            }
            base.Update(gameTime);
        }
    }
}