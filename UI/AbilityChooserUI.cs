using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace JoJoFanStands.UI
{
    internal class AbilityChooserUI : UIState      //ExamplpMod's ExampleUI, CoinPanel. Look for an easier and cleaner way of doig the text thing in the future
    {
        public DragableUIPanel AbilityUI;
        public static bool Visible;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void OnInitialize()
        {
            AbilityUI = new DragableUIPanel();
            AbilityUI.HAlign = 0.5f;
            AbilityUI.VAlign = 0.5f;
            AbilityUI.Width.Set(600f, 0f);
            AbilityUI.Height.Set(400f, 0f);

            UIImageButton pushBack = new UIImageButton(ModContent.GetTexture("JoJoFanStands/UI/PushBack"));
            pushBack.HAlign = 0.1f;
            pushBack.VAlign = 0.2f;
            pushBack.Width.Set(24f, 0f);
            pushBack.Height.Set(24f, 0f);
            pushBack.OnClick += new MouseEvent(PushBackClicked);
            AbilityUI.Append(pushBack);

            UIImageButton forceField = new UIImageButton(ModContent.GetTexture("JoJoFanStands/UI/ForceField"));
            forceField.HAlign = 0.2f;
            forceField.VAlign = 0.2f;
            forceField.Width.Set(24f, 0f);
            forceField.Height.Set(24f, 0f);
            forceField.OnClick += new MouseEvent(ForceFieldClicked);
            AbilityUI.Append(forceField);

            UIImageButton crystal = new UIImageButton(ModContent.GetTexture("JoJoFanStands/UI/Crystal"));
            crystal.HAlign = 0.3f;
            crystal.VAlign = 0.2f;
            crystal.Width.Set(24f, 0f);
            crystal.Height.Set(24f, 0f);
            crystal.OnClick += new MouseEvent(CrystalClicked);
            AbilityUI.Append(crystal);

            UIImageButton gravity = new UIImageButton(ModContent.GetTexture("JoJoFanStands/UI/Gravity"));
            gravity.HAlign = 0.4f;
            gravity.VAlign = 0.2f;
            gravity.Width.Set(24f, 0f);
            gravity.Height.Set(24f, 0f);
            gravity.OnClick += new MouseEvent(GravityClicked);
            AbilityUI.Append(gravity);

            UIImageButton genocide = new UIImageButton(ModContent.GetTexture("JoJoFanStands/UI/Genocide"));
            genocide.HAlign = 0.5f;
            genocide.VAlign = 0.2f;
            genocide.Width.Set(24f, 0f);
            genocide.Height.Set(24f, 0f);
            genocide.OnClick += new MouseEvent(GenocideClicked);
            AbilityUI.Append(genocide);

            UITextBox chooseText = new UITextBox("Choose an ability...");
            chooseText.HAlign = 0.1f;
            AbilityUI.Append(chooseText);

            Append(AbilityUI);
            base.OnInitialize();
        }

        private void PushBackClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Projectiles.PlayerStands.Megalovania.MegalovaniaStand.abilityNumber = 1;
            Visible = false;
        }

        private void ForceFieldClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Projectiles.PlayerStands.Megalovania.MegalovaniaStand.abilityNumber = 2;
            Visible = false;
        }

        private void CrystalClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Projectiles.PlayerStands.Megalovania.MegalovaniaStand.abilityNumber = 3;
            Visible = false;
        }

        private void GravityClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Projectiles.PlayerStands.Megalovania.MegalovaniaStand.abilityNumber = 4;
            Visible = false;
        }

        private void GenocideClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Projectiles.PlayerStands.Megalovania.MegalovaniaStand.abilityNumber = 5;
            Visible = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)       //from ExampleMod's ExampleUI
        {
            base.DrawSelf(spriteBatch);
        }
    }
}