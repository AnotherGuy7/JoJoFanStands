using JoJoFanStands.UI;
using JoJoFanStands.UI.AbilityWheel.Blur;
using JoJoFanStands.UI.AbilityWheel.HolyDiver;
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
        private UserInterface _holyDiverAbilityWheelUI;
        private UserInterface _soulBarUI;
        private UserInterface _waterGaugeBarUI;

        internal AbilityChooserUI AbilityUI;
        public static LightBridgeUI LightBridgeUI;
        public static BlurBar BlurBarUI;
        public static BlurAbilityWheel BlurAbilityWheelUI;
        public static HolyDiverAbilityWheel HolyDiverAbilityWheelUI;
        public static SoulBar SoulBarUI;
        public static WaterGaugeBar WaterGaugeBarUI;

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

                HolyDiverAbilityWheelUI = new HolyDiverAbilityWheel();
                HolyDiverAbilityWheelUI.Activate();
                _holyDiverAbilityWheelUI = new UserInterface();
                _holyDiverAbilityWheelUI.SetState(HolyDiverAbilityWheelUI);

                SoulBarUI = new SoulBar();
                SoulBarUI.Activate();
                _soulBarUI = new UserInterface();
                _soulBarUI.SetState(SoulBarUI);

                WaterGaugeBarUI = new WaterGaugeBar();
                WaterGaugeBarUI.Activate();
                _waterGaugeBarUI = new UserInterface();
                _waterGaugeBarUI.SetState(WaterGaugeBarUI);
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

            if (HolyDiverAbilityWheel.Visible)
                _holyDiverAbilityWheelUI.Update(gameTime);

            if (SoulBar.Visible)
                _soulBarUI.Update(gameTime);

            if (WaterGaugeBar.Visible)
                _waterGaugeBarUI.Update(gameTime);
        }

        public override void Unload()
        {
            BlurAbilityWheelUI = null;
            HolyDiverAbilityWheelUI = null;
            WaterGaugeBarUI = null;
            for (int i = 0; i < BlurAbilityWheel.blurAbilityWheel.abilityButtons.Length; i++)
                BlurAbilityWheel.blurAbilityWheel.abilityButtons[i] = null;
            for (int i = 0; i < HolyDiverAbilityWheel.holyDiverAbilityWheel.abilityButtons.Length; i++)
                HolyDiverAbilityWheel.holyDiverAbilityWheel.abilityButtons[i] = null;
            BlurAbilityWheel.blurAbilityWheel = null;
            HolyDiverAbilityWheel.holyDiverAbilityWheel = null;
        }

        public override void OnWorldUnload()
        {
            GoldExperienceAbilityWheel.CloseAbilityWheel();
            GoldExperienceRequiemAbilityWheel.CloseAbilityWheel();
            StoneFreeAbilityWheel.CloseAbilityWheel();
            BlurAbilityWheel.CloseAbilityWheel();
            HolyDiverAbilityWheel.CloseAbilityWheel();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            layers.Insert(5, new LegacyGameInterfaceLayer("JoJoFanStands: UI", DrawUI, InterfaceScaleType.UI));
        }

        private bool DrawUI()
        {
            if (AbilityChooserUI.Visible)
                _abilityui.Draw(Main.spriteBatch, new GameTime());

            if (LightBridgeUI.Visible)
                _lightBridgeUI.Draw(Main.spriteBatch, new GameTime());

            if (BlurBar.Visible)
                _blurBarUI.Draw(Main.spriteBatch, new GameTime());

            if (BlurAbilityWheel.Visible)
                _blurAbilityWheelUI.Draw(Main.spriteBatch, new GameTime());

            if (HolyDiverAbilityWheel.Visible)
                _holyDiverAbilityWheelUI.Draw(Main.spriteBatch, new GameTime());

            if (SoulBar.Visible)
                _soulBarUI.Draw(Main.spriteBatch, new GameTime());

            if (WaterGaugeBar.Visible)
                _waterGaugeBarUI.Draw(Main.spriteBatch, new GameTime());

            return true;
        }
    }
}