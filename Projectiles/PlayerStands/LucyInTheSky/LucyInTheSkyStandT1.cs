using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.LucyInTheSky
{
    public class LucyInTheSkyStandT1 : StandClass
    {
        public override int PunchDamage => 19;
        public override int PunchTime => 12;
        public override int HalfStandHeight => 37;
        public override int TierNumber => 1;
        //public override Vector2 StandOffset => Vector2.Zero;
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;
        public override string PoseSoundName => "LucyInTheSky";

        private const int MaxAmountOfMarkers = 3;

        private int amountOfBridges = 0;
        private int lightBridgeShootTimer = 0;
        private Vector2 lightBridgeShootPosition;
        private bool showAllBridgeSources = false;
        private int lightMarkerFrame = 0;
        private int lightMarkerFrameCounter = 0;

        public override void ExtraSpawnEffects()
        {
            for (int p = 0; p < Main.maxProjectiles; p++)
            {
                Projectile marker = Main.projectile[p];
                if (marker.active && marker.type == ModContent.ProjectileType<LightMarker>() && marker.owner == Main.player[Projectile.owner].whoAmI)
                {
                    marker.ai[0] = amountOfBridges;
                    amountOfBridges++;
                    if (amountOfBridges > MaxAmountOfMarkers)
                    {
                        amountOfBridges--;
                        marker.Kill();
                    }
                }
            }
        }

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft)
                        Punch(afterImages: false);
                    else
                    {
                        StayBehind();
                        Projectile.position.X += 4f * -Projectile.direction;
                    }

                    if (Main.mouseRight && shootCount <= 0)
                    {
                        Vector3 lightLevel = Lighting.GetColor((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16).ToVector3();       //1.703 is max light
                        if (lightLevel.Length() > 1.3f && Main.tile[(int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16].TileType == TileID.Torches)
                        {
                            secondaryAbility = true;
                            lightBridgeShootPosition = Main.MouseWorld - new Vector2(8f);
                            Projectile.frame = 0;
                        }
                        shootCount += 30;
                    }

                    if (secondaryAbility)
                    {
                        currentAnimationState = AnimationState.SecondaryAbility;
                        if (lightBridgeShootPosition.X > Projectile.Center.X)
                            Projectile.spriteDirection = 1;
                        else
                            Projectile.spriteDirection = -1;

                        lightBridgeShootTimer++;
                        if (lightBridgeShootTimer >= 10)
                        {
                            if (amountOfBridges >= MaxAmountOfMarkers)
                            {
                                amountOfBridges -= 1;
                                for (int p = 0; p < Main.maxProjectiles; p++)
                                {
                                    Projectile marker = Main.projectile[p];
                                    if (marker.active && marker.type == ModContent.ProjectileType<LightMarker>() && marker.owner == player.whoAmI)
                                    {
                                        if (marker.ai[0] == 0)
                                            marker.Kill();
                                        else
                                            marker.ai[0] -= 1;
                                    }
                                }
                            }

                            secondaryAbility = false;
                            Vector2 velocity = Main.MouseWorld - Projectile.Center;
                            velocity.Normalize();
                            velocity *= 12f;
                            int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<LucyInTheSkyBeam>(), 0, 0f, Projectile.owner, amountOfBridges);
                            (Main.projectile[index].ModProjectile as LucyInTheSkyBeam).targetPosition = Main.MouseWorld;
                            lightBridgeShootTimer = 0;
                            amountOfBridges += 1;
                        }
                    }
                }

                if (SecondSpecialKeyPressed(false))
                {
                    /*if (LightBridgeUI.Visible)
                        LightBridgeUI.HideLightBridgeUI();
                    else
                        LightBridgeUI.ShowLightBridgeUI(3);*/
                    showAllBridgeSources = !showAllBridgeSources;
                }

                if (showAllBridgeSources)
                {
                    lightMarkerFrameCounter++;
                    if (lightMarkerFrameCounter >= 8)
                    {
                        lightMarkerFrame++;
                        lightMarkerFrameCounter = 0;
                        if (lightMarkerFrame >= 9)
                            lightMarkerFrame = 0;
                    }

                    for (int p = 0; p < Main.maxProjectiles; p++)
                    {
                        Projectile marker = Main.projectile[p];
                        if (marker.active && marker.type == ModContent.ProjectileType<LightMarker>() && marker.owner == player.whoAmI)
                        {
                            Vector2 markerDirection = marker.position - player.Center;
                            markerDirection.Normalize();
                            Vector2 uiPosition = Main.player[Projectile.owner].Center + (markerDirection * 64f);

                            if (Main.mouseLeft && Vector2.Distance(Main.MouseWorld, uiPosition) <= 16f)
                                fPlayer.lucySelectedMarkerWhoAmI = marker.whoAmI;
                        }
                    }
                }
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
                BasicPunchAI();
            LimitDistance();
            if (mPlayer.posing)
                currentAnimationState = AnimationState.Pose;
        }

        private Texture2D leftArmSheet;
        private Texture2D rightArmSheet;
        private Texture2D lightMarkerSheet;
        private readonly Vector2 lightMarkerOrigin = new Vector2(8);

        public override bool PreDrawExtras()
        {
            Player player = Main.player[Projectile.owner];
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (secondaryAbility)
            {
                if (Projectile.spriteDirection == 1)
                {
                    if (rightArmSheet == null)
                        rightArmSheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/LucyInTheSky/LucyInTheSky_RightArm", AssetRequestMode.ImmediateLoad).Value;

                    Rectangle sourceRect = new Rectangle(0, Projectile.frame * 18, 34, 18);
                    Color lightColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
                    Vector2 drawOffset = StandOffset;
                    drawOffset.X *= Projectile.spriteDirection;
                    Vector2 drawPosition = Projectile.Center - drawOffset;
                    Vector2 armTarget = lightBridgeShootPosition - Main.player[Projectile.owner].Center;
                    armTarget.Normalize();
                    SpriteEffects effect = SpriteEffects.None;
                    if (Projectile.spriteDirection == -1)
                    {
                        effect = SpriteEffects.FlipVertically;
                        drawPosition -= new Vector2(24f, 0);
                    }
                    Vector2 rightArmPosition = drawPosition - Main.screenPosition + new Vector2(22f * Projectile.direction, -6f);

                    Main.EntitySpriteDraw(rightArmSheet, rightArmPosition, sourceRect, lightColor, armTarget.ToRotation(), new Vector2(8, 10), Projectile.scale, effect, 0);
                }
                else
                {
                    if (leftArmSheet == null)
                        leftArmSheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/LucyInTheSky/LucyInTheSky_LeftArm", AssetRequestMode.ImmediateLoad).Value;

                    Rectangle sourceRect = new Rectangle(0, Projectile.frame * 18, 34, 18);
                    Color lightColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
                    Vector2 drawOffset = StandOffset;
                    drawOffset.X *= Projectile.spriteDirection;
                    Vector2 drawPosition = Projectile.Center - drawOffset;
                    Vector2 armTarget = lightBridgeShootPosition - Main.player[Projectile.owner].Center;
                    armTarget.Normalize();
                    SpriteEffects effect = SpriteEffects.None;
                    if (Projectile.spriteDirection == -1)
                    {
                        effect = SpriteEffects.FlipVertically;
                        drawPosition -= new Vector2(24f, 0);
                    }
                    Vector2 rightArmPosition = drawPosition - Main.screenPosition + new Vector2(22f * Projectile.direction, -6f);
                    Vector2 leftArmPosition = drawPosition - Main.screenPosition + new Vector2(4f, -6f);
                    Vector2 leftArmRotation = (rightArmPosition + (armTarget * new Vector2(22, 14))) - (leftArmPosition + new Vector2(22, 8));

                    Main.EntitySpriteDraw(leftArmSheet, leftArmPosition, sourceRect, lightColor, leftArmRotation.ToRotation() - MathHelper.Pi / 4f, new Vector2(6), Projectile.scale, effect, 0);
                }
            }
            if (showAllBridgeSources)
            {
                if (lightMarkerSheet == null)
                    lightMarkerSheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/LucyInTheSky/LightMarker", AssetRequestMode.ImmediateLoad).Value;

                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile marker = Main.projectile[p];
                    if (marker.active && marker.type == ModContent.ProjectileType<LightMarker>() && marker.owner == player.whoAmI)
                    {
                        float scale = 1f;
                        Rectangle animRect = new Rectangle(0, lightMarkerFrame * 16, 16, 16);
                        Vector2 markerDirection = marker.position - player.Center;
                        markerDirection.Normalize();
                        Vector2 drawPosition = Main.player[Projectile.owner].Center + (markerDirection * 64f);
                        Color drawColor = Color.White;
                        if (Vector2.Distance(Main.MouseWorld, drawPosition) <= 16f)
                        {
                            scale = 1.2f;
                            drawColor = Color.Orange;
                        }
                        if (marker.whoAmI == fPlayer.lucySelectedMarkerWhoAmI)
                            drawColor = Color.Yellow;

                        Main.EntitySpriteDraw(lightMarkerSheet, drawPosition - Main.screenPosition, animRect, drawColor, 0f, lightMarkerOrigin, scale, SpriteEffects.None, 0);
                    }
                }
            }
            return true;
        }

        public override void PostDraw(Color drawColor)
        {
            base.PostDraw(drawColor);

            if (secondaryAbility)
            {
                if (Projectile.spriteDirection == 1)
                {
                    if (leftArmSheet == null)
                        leftArmSheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/LucyInTheSky/LucyInTheSky_LeftArm", AssetRequestMode.ImmediateLoad).Value;

                    Rectangle sourceRect = new Rectangle(0, Projectile.frame * 18, 34, 18);
                    Color lightColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
                    Vector2 drawOffset = StandOffset;
                    drawOffset.X *= Projectile.spriteDirection;
                    Vector2 drawPosition = Projectile.Center - drawOffset;
                    Vector2 armTarget = lightBridgeShootPosition - Main.player[Projectile.owner].Center;
                    armTarget.Normalize();
                    SpriteEffects effect = SpriteEffects.None;
                    if (Projectile.spriteDirection == -1)
                    {
                        effect = SpriteEffects.FlipVertically;
                        drawPosition -= new Vector2(24f, 0);
                    }
                    Vector2 rightArmPosition = drawPosition - Main.screenPosition + new Vector2(22f * Projectile.direction, -6f);
                    Vector2 leftArmPosition = drawPosition - Main.screenPosition + new Vector2(4f, -6f);
                    Vector2 leftArmRotation = (rightArmPosition + (armTarget * new Vector2(22, 14))) - (leftArmPosition + new Vector2(22, 8));

                    Main.EntitySpriteDraw(leftArmSheet, leftArmPosition, sourceRect, lightColor, leftArmRotation.ToRotation(), new Vector2(6), Projectile.scale, effect, 0);
                }
                else
                {
                    if (rightArmSheet == null)
                        rightArmSheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/LucyInTheSky/LucyInTheSky_RightArm", AssetRequestMode.ImmediateLoad).Value;

                    Rectangle sourceRect = new Rectangle(0, Projectile.frame * 18, 34, 18);
                    Color lightColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
                    Vector2 drawOffset = StandOffset;
                    drawOffset.X *= Projectile.spriteDirection;
                    Vector2 drawPosition = Projectile.Center - drawOffset;
                    Vector2 armTarget = lightBridgeShootPosition - Main.player[Projectile.owner].Center;
                    armTarget.Normalize();
                    SpriteEffects effect = SpriteEffects.None;
                    if (Projectile.spriteDirection == -1)
                    {
                        effect = SpriteEffects.FlipVertically;
                        drawPosition -= new Vector2(24f, 0);
                    }
                    Vector2 rightArmPosition = drawPosition - Main.screenPosition + new Vector2(22f * Projectile.direction, -6f);

                    Main.EntitySpriteDraw(rightArmSheet, rightArmPosition, sourceRect, lightColor, armTarget.ToRotation(), new Vector2(8, 10), Projectile.scale, effect, 0);
                }
            }
        }
        public override void SelectAnimation()
        {
            if (oldAnimationState != currentAnimationState)
            {
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
                oldAnimationState = currentAnimationState;
                Projectile.netUpdate = true;
            }

            if (currentAnimationState == AnimationState.Idle)
                PlayAnimation("Idle");
            else if (currentAnimationState == AnimationState.Attack)
                PlayAnimation("Attack");
            else if (currentAnimationState == AnimationState.SecondaryAbility)
                PlayAnimation("Armless");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/LucyInTheSky/LucyInTheSky_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
            else if (animationName == "Armless")
                AnimateStand(animationName, 4, 2, false);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 60, true);
        }
    }
}