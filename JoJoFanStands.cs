using Terraria.ModLoader;
using JoJoStands;
using Terraria.UI;
using JoJoFanStands.UI;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace JoJoFanStands
{
	public class JoJoFanStands : Mod
	{
        static internal JoJoFanStands Instance;

        static internal Mod JoJoStandsMod;

		private UserInterface _abilityui;

        public static ModHotKey AccessorySpecialKey;

		internal AbilityChooserUI AbilityUI;

        public JoJoFanStands()
		{
            Instance = this;
		}

		public override void Load()
		{
            AccessorySpecialKey = RegisterHotKey("JoJoFanStands: Accessory Special", "L");
            JoJoStandsMod = ModLoader.GetMod("JoJoStands");
            if (!Main.dedServ)
            {
                AbilityUI = new AbilityChooserUI();
                AbilityUI.Activate();
                _abilityui = new UserInterface();
                _abilityui.SetState(AbilityUI);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (AbilityChooserUI.Visible)
            {
                _abilityui.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)     //from ExampleMod's ExampleUI
        {
            layers.Insert(5, new LegacyGameInterfaceLayer("JoJoFanStands: UI", DrawUI, InterfaceScaleType.UI));     //from Terraria Interface for Dummies, and Insert so it doesn't draw over everything
        }

        private bool DrawUI()       //also from Terraria Interface for Dummies
        {
            if (AbilityChooserUI.Visible)
            {
                _abilityui.Draw(Main.spriteBatch, new GameTime());
            }
            return true;
        }
    }
}