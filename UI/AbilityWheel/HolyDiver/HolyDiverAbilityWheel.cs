using JoJoStands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace JoJoFanStands.UI.AbilityWheel.HolyDiver
{
    public class HolyDiverAbilityWheel : AbilityWheel
    {
        public static bool Visible;
        public static HolyDiverAbilityWheel holyDiverAbilityWheel;

        private const int AmountOfAbilities = 5;
        public override int amountOfAbilities => AmountOfAbilities;
        public override string buttonTexturePath => "JoJoFanStands/UI/AbilityWheel/HolyDiver/";
        public override string centerTexturePath => "JoJoFanStands/Items/Stands/HolyDiverT1";

        public override string[] abilityNames => new string[AmountOfAbilities]
        {
            "  Water Replicant",
            "Scorching Water Cannon",
            " Scorching Water Mine",
            " Water Absorption",
            "     Holy Water"
        };

        public override string[] abilityTextureNames => new string[AmountOfAbilities]
        {
            "WaterReplicant",
            "ScorchingWaterCannon",
            "ScorchingWaterMine",
            "WaterAbsorption",
            "HolyWater"
        };

        public override string[] abilityDescriptions => new string[AmountOfAbilities]
        {
            "Summon a water replicant that acts as a defensive sentry, shooting enemies from range and brawling up close.",
            "Fire a scorching hydro pump. Hold M2 to convert it into homing water missiles targeting up to 3 enemies.",
            "Place a water mine that homes in on nearby enemies, dealing extreme damage and applying Burning.",
            "Absorb water from the environment or enemies to refill your liquid gauge.",
            "Use stored water to heal yourself or allies. Damages enemies and applies Burning."
        };

        public override void ExtraInitialize()
        {
            holyDiverAbilityWheel = this;
        }

        public static void OpenAbilityWheel(MyPlayer modPlayer, int amountOfAbilities)
        {
            Visible = true;
            mPlayer = modPlayer;
            mPlayer.chosenAbility = 0;

            HolyDiverAbilityWheel wheel = holyDiverAbilityWheel;
            wheel.abilitiesShown = amountOfAbilities;

            for (int i = 0; i < AmountOfAbilities; i++)
            {
                wheel.abilityButtons[i].SetButtonPosiiton(
                    wheel.wheelCenter.buttonPosition +
                    wheel.IndexToRadianPosition(i, wheel.abilitiesShown, wheel.wheelRotation) * wheel.wheelSpace
                );
                if (i >= amountOfAbilities)
                    wheel.abilityButtons[i].invisible = true;
                else
                    wheel.abilityButtons[i].invisible = false;
            }

            wheel.wheelCenterPosition = new Vector2(Main.screenWidth - (wheel.AbilityWheelSize.X / 2f), Main.screenHeight * 0.5f);
            wheel.abilityNameText.SetText(wheel.abilityNames[0]);
            wheel.abilityNameText.Left.Pixels = wheel.wheelCenterPosition.X + wheel.wheelCenter.buttonPosition.X;
            wheel.abilityNameText.Top.Pixels = wheel.wheelCenterPosition.Y + wheel.wheelCenter.buttonPosition.Y + 60f;
            wheel.abilityNameText.Left.Pixels += -FontAssets.MouseText.Value.MeasureString(wheel.abilityNames[0]).X * 2f * wheel.textScale;
            wheel.abilityNameText.Left.Pixels += 10f;

            Main.player[Main.myPlayer].GetModPlayer<MyPlayer>().hotbarLocked = true;

            if (!Main.player[Main.myPlayer].GetModPlayer<MyPlayer>().abilityWheelTipDisplayed)
            {
                Main.NewText(Language.GetText("Mods.JoJoStands.MiscText.AbilityWheelHint").Value, Color.Gold);
                Main.player[Main.myPlayer].GetModPlayer<MyPlayer>().abilityWheelTipDisplayed = true;
            }
        }

        public static void CloseAbilityWheel()
        {
            Visible = false;
            Main.player[Main.myPlayer].GetModPlayer<MyPlayer>().hotbarLocked = false;
        }
    }
}