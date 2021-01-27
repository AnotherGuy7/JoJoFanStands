using Terraria.ModLoader;
using JoJoStands;
using Terraria.UI;
using JoJoFanStands.UI;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using JoJoFanStands.Items.Stands;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace JoJoFanStands
{
	public class JoJoFanStands : Mod
	{
        static internal JoJoFanStands Instance;

        static internal Mod JoJoStandsMod;

		private UserInterface _abilityui;

		internal AbilityChooserUI AbilityUI;

        public JoJoFanStands()
		{
            Instance = this;
		}

		public override void Load()
		{
            JoJoStandsMod = ModLoader.GetMod("JoJoStands");

            if (!Main.dedServ)
            {
                AbilityUI = new AbilityChooserUI();
                AbilityUI.Activate();
                _abilityui = new UserInterface();
                _abilityui.SetState(AbilityUI);

                //Shader stuff
                Ref<Effect> distortedReality = new Ref<Effect>(GetEffect("Effects/MonotoneRealityShader"));
                Filters.Scene["MonotoneRealityEffect"] = new Filter(new ScreenShaderData(distortedReality, "MonotoneRealityEffect"), EffectPriority.VeryHigh);
                Filters.Scene["MonotoneRealityEffect"].Load();
            }
            MyPlayer.standTier1List.Add(ModContent.ItemType<CoolOutT1>());
            MyPlayer.standTier1List.Add(ModContent.ItemType<FollowMeT1>());
            MyPlayer.standTier1List.Add(ModContent.ItemType<MortalReminderT1>());
            MyPlayer.standTier1List.Add(ModContent.ItemType<SlavesOfFearT1>());
            MyPlayer.standTier1List.Add(ModContent.ItemType<TheFatesT1>());
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