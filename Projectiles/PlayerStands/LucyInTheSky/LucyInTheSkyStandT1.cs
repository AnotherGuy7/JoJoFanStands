using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ID;
using JoJoFanStands.UI;

namespace JoJoFanStands.Projectiles.PlayerStands.LucyInTheSky
{
    public class LucyInTheSkyStandT1 : StandClass
    {
        public override int punchDamage => 23;
        public override int punchTime => 13;
        public override int altDamage => 96;
        public override int halfStandHeight => 37;
        //public override int standOffset => 0;
        public override StandType standType => StandType.Melee;

        private const int MaxAmountOfMarkers = 3;

        private int amountOfBridges = 0;
        private int lightBridgeShootTimer = 0;
        private Vector2 lightBridgeShootPosition;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (Main.mouseLeft)
            {
                Punch();
            }
            else
            {
                StayBehind();
                Projectile.position.X += 4f * -Projectile.direction;
                if (secondaryAbilityFrames)
                    idleFrames = false;
            }

            if (Main.mouseRight && shootCount <= 0 && player.whoAmI == Main.myPlayer)
            {
                Vector3 lightLevel = Lighting.GetColor((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16).ToVector3();       //1.703 is max light
                if (lightLevel.Length() > 1.3f && amountOfBridges < MaxAmountOfMarkers)
                {
                    secondaryAbilityFrames = true;
                    lightBridgeShootPosition = Main.MouseWorld - new Vector2(8f);
                }
                else
                {
                    if (LightBridgeUI.Visible)
                        LightBridgeUI.HideLightBridgeUI();
                    else
                        LightBridgeUI.ShowLightBridgeUI(3);
                }
                shootCount += 30;
            }

            if (secondaryAbilityFrames)
            {
                lightBridgeShootTimer++;
                if (lightBridgeShootTimer >= 60)
                {
                    secondaryAbilityFrames = false;
                    int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), lightBridgeShootPosition, Vector2.Zero, ModContent.ProjectileType<LightMarker>(), 0, 0f, Projectile.owner, amountOfBridges);
                    player.GetModPlayer<FanPlayer>().lucySelectedMarkerWhoAmI = index;
                    lightBridgeShootTimer = 0;
                    amountOfBridges += 1;
                }
            }
            LimitDistance();
        }

        private Texture2D litSArmSheet;
        private readonly Vector2 litsArmOrigin = new Vector2(42, 36);

        public override bool PreDrawExtras()
        {
            if (secondaryAbilityFrames)
            {
                if (litSArmSheet == null)
                    litSArmSheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/LucyInTheSky/LucyInTheSky_Arms", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                Rectangle sourceRect = new Rectangle(0, Projectile.frame * 72, 84, 72);
                Color lightColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
                Vector2 armTarget = Projectile.Center - lightBridgeShootPosition;
                SpriteEffects effect = SpriteEffects.None;
                if (Projectile.direction == -1)
                    effect = SpriteEffects.FlipHorizontally;

                Main.EntitySpriteDraw(litSArmSheet, Projectile.Center - Main.screenPosition, sourceRect, lightColor, armTarget.ToRotation(), litsArmOrigin, Projectile.scale, effect, 0);
            }
            return true;
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                idleFrames = false;
                PlayAnimation("Attack");
            }
            if (idleFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
            if (secondaryAbilityFrames)
            {
                attackFrames = false;
                idleFrames = false;
                PlayAnimation("Armless");
            }
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/LucyInTheSky/LucyInTheSky_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 15, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 4, newPunchTime, true);
            }
            if (animationName == "Armless")
            {
                AnimateStand(animationName, 4, 15, false);
            }
            if (animationName == "Pose")
            {
                AnimateStand(animationName, 1, 60, true);
            }
        }
    }
}