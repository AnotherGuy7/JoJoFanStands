using JoJoFanStands.UI;
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

        internal AbilityChooserUI AbilityUI;
        public static LightBridgeUI LightBridgeUI;

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
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (AbilityChooserUI.Visible)
                _abilityui.Update(gameTime);

            if (LightBridgeUI.Visible)
                _lightBridgeUI.Update(gameTime);
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
            return true;
        }
    }
}
