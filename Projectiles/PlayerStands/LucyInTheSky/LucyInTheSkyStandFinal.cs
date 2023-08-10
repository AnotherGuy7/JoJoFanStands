using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.LucyInTheSky
{
    public class LucyInTheSkyStandFinal : StandClass
    {
        public override int PunchDamage => 101;
        public override int PunchTime => 9;
        public override int HalfStandHeight => 37;
        public override int TierNumber => 4;
        public override int AltDamage => 98;
        //public override Vector2 StandOffset => Vector2.Zero;
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;
        public override bool UseProjectileAlpha => true;

        private const int MaxAmountOfMarkers = 12;

        private int amountOfBridges = 0;
        private int lightBridgeShootTimer = 0;
        private Vector2 lightBridgeShootPosition;
        private bool showAllBridgeSources = false;
        private int lightMarkerFrame = 0;
        private int lightMarkerFrameCounter = 0;
        private int hiddenTeleportCooldownTimer = 0;
        private int intoTheLightTimer = 0;
        private int secondSpecialKeyPressTimer = 0;
        private int bulletAbilityShootRequirement = 45;

        public override void ExtraSpawnEffects()
        {
            Projectile.alpha = 255;
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
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;
            if (hiddenTeleportCooldownTimer > 0)
                hiddenTeleportCooldownTimer--;
            if (intoTheLightTimer > 0)
            {
                intoTheLightTimer--;
                if (intoTheLightTimer <= 0)
                {
                    fPlayer.litsIntoTheLightAbilityActive = false;
                    player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(15));
                }
            }
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;
            if (fPlayer.hidingInLucyMarker)
                Projectile.alpha = 0;
            else
                Projectile.alpha = 255;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft && !fPlayer.hidingInLucyMarker)
                        Punch(afterImages: false);
                    else
                    {
                        StayBehind();
                        Projectile.position.X += 4f * -Projectile.direction;
                    }

                    if (Main.mouseRight && !fPlayer.hidingInLucyMarker && shootCount <= 0 && !playerHasAbilityCooldown)
                    {
                        bool attemptingTeleport = false;
                        for (int p = 0; p < Main.maxProjectiles; p++)
                        {
                            Projectile marker = Main.projectile[p];
                            if (marker.active && marker.type == ModContent.ProjectileType<LightMarker>() && marker.owner == player.whoAmI && marker.Hitbox.Contains(Main.MouseWorld.ToPoint()))
                            {
                                shootCount = 60;
                                attemptingTeleport = true;
                                break;
                            }
                        }
                        if (!attemptingTeleport)
                        {
                            Vector3 lightLevel = Lighting.GetColor((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16).ToVector3();       //1.703 is max light
                            if (lightLevel.Length() > 1.3f && Main.tile[(int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16].TileType == TileID.Torches)
                            {
                                secondaryAbility = true;
                                lightBridgeShootPosition = Main.MouseWorld - new Vector2(8f);
                            }
                            shootCount += 30;
                        }
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

                if (SpecialKeyPressed())
                {
                    intoTheLightTimer = 10 * 60;
                    fPlayer.litsIntoTheLightAbilityActive = true;
                }

                if (SecondSpecialKeyCurrent())
                {
                    secondSpecialKeyPressTimer++;
                    if (secondSpecialKeyPressTimer % bulletAbilityShootRequirement == 0)
                    {
                        for (int p = 0; p < Main.maxProjectiles; p++)
                        {
                            Projectile marker = Main.projectile[p];
                            if (marker.active && marker.type == ModContent.ProjectileType<LightMarker>() && marker.owner == player.whoAmI && marker.whoAmI == fPlayer.lucySelectedMarkerWhoAmI)
                            {
                                int targetWhoAmI = -1;
                                for (int n = 0; n < Main.maxNPCs; n++)
                                {
                                    NPC npc = Main.npc[n];
                                    if (npc.active && npc.lifeMax > 5 && !npc.townNPC && !npc.hide && !npc.friendly && Vector2.Distance(npc.Center, marker.Center) <= 42 * 16)
                                    {
                                        targetWhoAmI = npc.whoAmI;
                                        break;
                                    }
                                }

                                if (targetWhoAmI != -1)
                                {
                                    int bulletIndex = GetPlayerAmmo(player);
                                    Item bulletItem = player.inventory[bulletIndex];
                                    Vector2 shootVel = Main.npc[targetWhoAmI].Center + new Vector2(Main.rand.Next(-16, 16 + 1), Main.rand.Next(-16, 16 + 1)) - marker.Center;
                                    if (shootVel == Vector2.Zero)
                                        shootVel = new Vector2(0f, 1f);

                                    shootVel.Normalize();
                                    shootVel *= 12f;
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), marker.Center, shootVel, bulletItem.shoot, (int)((AltDamage + bulletItem.damage) * mPlayer.standDamageBoosts), bulletItem.knockBack, Projectile.owner, Projectile.whoAmI);
                                    if (bulletItem.type != ItemID.EndlessMusketPouch)
                                        player.ConsumeItem(bulletItem.type);
                                    if (bulletAbilityShootRequirement > 6)
                                        bulletAbilityShootRequirement -= 3;
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (secondSpecialKeyPressTimer != 0)
                    {
                        if (secondSpecialKeyPressTimer < 45)
                            showAllBridgeSources = !showAllBridgeSources;

                        secondSpecialKeyPressTimer = 0;
                        bulletAbilityShootRequirement = 45;
                    }
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
                            {
                                fPlayer.lucySelectedMarkerWhoAmI = marker.whoAmI;
                                if (fPlayer.hidingInLucyMarker && hiddenTeleportCooldownTimer <= 0)
                                {
                                    hiddenTeleportCooldownTimer = 30;
                                    SoundStyle coinSound = SoundID.CoinPickup;
                                    coinSound.Pitch = 2f;
                                    coinSound.PitchVariance = 0.2f;
                                    SoundEngine.PlaySound(coinSound, Projectile.Center);
                                    player.position = marker.position;
                                }
                            }
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

        private int GetPlayerAmmo(Player player)
        {
            int ammoType = -1;
            for (int i = 54; i < 58; i++)
            {
                Item Item = player.inventory[i];

                if (Item.ammo == AmmoID.Bullet && Item.stack > 0)
                {
                    ammoType = i;
                    break;
                }
            }
            if (ammoType == -1)
            {
                for (int i = 0; i < 54; i++)
                {
                    Item Item = player.inventory[i];
                    if (Item.ammo == AmmoID.Bullet && Item.stack > 0)
                    {
                        ammoType = i;
                        break;
                    }
                }
            }
            return ammoType;
        }

        public override void StandKillEffects()
        {
            Player player = Main.player[Projectile.owner];
            if (player.GetModPlayer<FanPlayer>().litsIntoTheLightAbilityActive)
            {
                player.GetModPlayer<FanPlayer>().litsIntoTheLightAbilityActive = false;
                player.AddBuff(ModContent.BuffType<AbilityCooldown>(), player.GetModPlayer<MyPlayer>().AbilityCooldownTime(15));
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