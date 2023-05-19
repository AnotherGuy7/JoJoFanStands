using JoJoFanStands.UI;
using JoJoFanStands.UI.AbilityWheel.Blur;
using JoJoStands.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace JoJoFanStands
{
    public class JoJoFanStandsSystem : ModSystem
    {
        private UserInterface _abilityui;
        private UserInterface _lightBridgeUI;
        private UserInterface _blurBarUI;
        private UserInterface _blurAbilityWheelUI;

        internal AbilityChooserUI AbilityUI;
        public static LightBridgeUI LightBridgeUI;
        public static BlurBar BlurBarUI;
        public static BlurAbilityWheel BlurAbilityWheelUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                AbilityUI = new AbilityChooserUI();
                AbilityUI.Activate();
                _abilityui = new UserInterface();
                _abilityui.SetState(AbilityUI);

                LightBridgeUI = new LightBridgeUI();
                LightBridgeUI.Activate();
                _lightBridgeUI = new UserInterface();
                _lightBridgeUI.SetState(LightBridgeUI);

                BlurBarUI = new BlurBar();
                BlurBarUI.Activate();
                _blurBarUI = new UserInterface();
                _blurBarUI.SetState(BlurBarUI);

                BlurAbilityWheelUI = new BlurAbilityWheel();
                BlurAbilityWheelUI.Activate();
                _blurAbilityWheelUI = new UserInterface();
                _blurAbilityWheelUI.SetState(BlurAbilityWheelUI);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (AbilityChooserUI.Visible)
                _abilityui.Update(gameTime);

            if (LightBridgeUI.Visible)
                _lightBridgeUI.Update(gameTime);

            if (BlurBar.Visible)
                _blurBarUI.Update(gameTime);

            if (BlurAbilityWheel.Visible)
                _blurAbilityWheelUI.Update(gameTime);
        }

        public override void Unload()
        {
            BlurAbilityWheelUI = null;
            for (int i = 0; i < BlurAbilityWheel.blurAbilityWheel.abilityButtons.Length; i++)
                BlurAbilityWheel.blurAbilityWheel.abilityButtons[i] = null;
            BlurAbilityWheel.blurAbilityWheel = null;
        }

        public override void OnWorldUnload()
        {
            GoldExperienceAbilityWheel.CloseAbilityWheel();
            GoldExperienceRequiemAbilityWheel.CloseAbilityWheel();
            StoneFreeAbilityWheel.CloseAbilityWheel();
            BlurAbilityWheel.CloseAbilityWheel();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)     //from ExampleMod's ExampleUI
        {
            layers.Insert(5, new LegacyGameInterfaceLayer("JoJoFanStands: UI", DrawUI, InterfaceScaleType.UI));     //from Terraria Interface for Dummies, and Insert so it doesn't draw over everything
        }

        private bool DrawUI()       //also from Terraria Interface for Dummies
        {
            if (AbilityChooserUI.Visible)
                _abilityui.Draw(Main.spriteBatch, new GameTime());

            if (LightBridgeUI.Visible)
                _lightBridgeUI.Draw(Main.spriteBatch, new GameTime());

            if (BlurBar.Visible)
                _blurBarUI.Draw(Main.spriteBatch, new GameTime());

            if (BlurAbilityWheel.Visible)
                _blurAbilityWheelUI.Draw(Main.spriteBatch, new GameTime());
            return true;
        }
    }
}
