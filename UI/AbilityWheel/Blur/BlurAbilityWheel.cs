using JoJoStands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;

namespace JoJoFanStands.UI.AbilityWheel.Blur
{
    public class BlurAbilityWheel : AbilityWheel
    {
        public static bool Visible;
        public static BlurAbilityWheel blurAbilityWheel;

        private const int AmountOfAbilities = 3;
        public override int amountOfAbilities => AmountOfAbilities;
        public override string buttonTexturePath => "JoJoFanStands/UI/AbilityWheel/Blur/";
        public override string centerTexturePath => "JoJoFanStands/Items/Stands/BlurT1";
        public override string[] abilityNames => new string[AmountOfAbilities]
        {
            "      Knife Throw",
            "        Vibration",
            "Infinite Velocity",
        };

        public override string[] abilityTextureNames => new string[AmountOfAbilities]
{
            "Knife",
            "Vibration",
            "InfiniteVelocity",
        };


        public override string[] abilityDescriptions => new string[AmountOfAbilities]
        {
            "Allows Blur to throw a dual stream of knives.",
            "[Special]\nAllows Blur to vibrate itself, moving out of the way of any dangers approaching it.",
            "[Special]\nAllows the Blur to move so fast the world would seem halted in comparison.",
        };

        public override void ExtraInitialize()
        {
            blurAbilityWheel = this;
        }

        public static void OpenAbilityWheel(MyPlayer modPlayer, int amountOfAbilities)
        {
            Visible = true;
            mPlayer = modPlayer;
            mPlayer.chosenAbility = 0;

            BlurAbilityWheel wheel = blurAbilityWheel;
            wheel.abilitiesShown = amountOfAbilities;
            for (int i = 0; i < amountOfAbilities; i++)
            {
                wheel.abilityButtons[i].SetButtonPosiiton(wheel.wheelCenter.buttonPosition + wheel.IndexToRadianPosition(i, wheel.abilitiesShown, wheel.wheelRotation) * wheel.wheelSpace);
                if (i > wheel.abilitiesShown - 1)
                    wheel.abilityButtons[i].invisible = true;
            }
            wheel.wheelCenterPosition = new Vector2(Main.screenWidth - (wheel.AbilityWheelSize.X / 2f), Main.screenHeight * JoJoStands.UI.AbilityWheel.VerticalAlignmentPercentage);
            wheel.abilityNameText.SetText(wheel.abilityNames[0]);
            wheel.abilityNameText.Left.Pixels = wheel.wheelCenterPosition.X + wheel.wheelCenter.buttonPosition.X;
            wheel.abilityNameText.Top.Pixels = wheel.wheelCenterPosition.Y + wheel.wheelCenter.buttonPosition.Y + 60f;
            wheel.abilityNameText.Left.Pixels += -FontAssets.MouseText.Value.MeasureString(wheel.abilityNames[0]).X * 2f * wheel.textScale;
            wheel.abilityNameText.Left.Pixels += 10f;
            Main.player[Main.myPlayer].GetModPlayer<MyPlayer>().hotbarLocked = true;
            if (!Main.player[Main.myPlayer].GetModPlayer<MyPlayer>().abilityWheelTipDisplayed)
            {
                Main.NewText("*To use the Ability Wheel, use the numbers 1-x OR hover over the ability wheel with your cursor and scroll up or down!*", Color.Gold);
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