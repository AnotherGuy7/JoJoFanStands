using JoJoFanStands.Buffs;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.BombTellyDir;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.GlueManDir;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.GreenDevilDir;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.PowerMusclerDir;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.YellowDevilDir;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity
{
    public class VirtualInsanityStandFinal : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 96;
        public override int AltDamage => 84;
        public override int PunchTime => 9;
        public override int TierNumber => 4;
        public override bool CanUseAfterImagePunches => false;
        public override Vector2 StandOffset => new Vector2(-24, 0);
        public override int FistID => FanStandFists.VirtualInsanityFists;
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        public new enum AnimationState
        {
            Idle,
            Attack,
            SecondaryAbility,
            Special,
            Pose
        }

        private int mouseRightHoldTimer = 0;
        private bool powerInstallBuff = false;
        private int attackChangeEffectTimer = 0;
        private bool throwingProjectile;
        private int projectileThrowTimer;
        private int oldSpinFrame = 0;
        private int throwTimer = 0;
        private int chargeTimer = 0;

        private byte attackType = Attack_Barrage;

        private const byte Attack_Barrage = 0;
        private const byte Attack_Sword = 1;
        private const byte Attack_Cannon = 2;
        private readonly string[] AttackStyleNames = new string[3] { "Barrage", "Sword", "Cannon" };
        private readonly int[] AttackStyleIdleFrameAmounts = new int[3] { 5, 5, 4 };
        private readonly int[] AttackStyleAttackFrameAmounts = new int[3] { 17, 18, 4 };
        private readonly int[] ThrowProjectiles = new int[5] { ModContent.ProjectileType<YellowDevil>(), ModContent.ProjectileType<GreenDevil>(), ModContent.ProjectileType<BombTelly>(), ModContent.ProjectileType<GlueMan>(), ModContent.ProjectileType<PowerMuscler>() };
        private readonly Vector2[] ThrowProjectilesOffset = new Vector2[5] { new Vector2(16, -63), new Vector2(16, -53), new Vector2(36, -66), new Vector2(12, -46), new Vector2(16, -58) };
        public static Texture2D[] AttackStyleTextures;
        public static Texture2D[] PortalTextures;
        public static Texture2D[] ArmCannonSpritesheets;
        public static Texture2D[] CannonHeadSpritesheets;
        public static Texture2D[] CannonHeadFlashSpritesheets;
        private readonly Vector2 ArmPlacementOffset = new Vector2(40 - 29, 18 + 39);
        private int portalFrame;
        private int portalFrameCounter;
        private int portalAnimationIndex;
        private bool portalSpawned = false;
        private bool throwAnimationOverride = false;
        private int throwProjectileSpawnTimer = 0;
        private bool powerInstallAnimation = false;
        private Projectile projectileToThrow;
        private readonly AnimationData[] portalAnimations = new AnimationData[3] {
            new AnimationData(10, 5),
            new AnimationData(9, 5),
            new AnimationData(9, 5)
        };

        public static readonly SoundStyle ShootSound = new SoundStyle("JoJoFanStands/Sounds/SoundEffects/VirtualInsanity/Gun")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };

        public struct AnimationData
        {
            public int maxFrames;
            public int frameDuration;

            public AnimationData(int frames, int framesPerFrame)
            {
                maxFrames = frames;
                frameDuration = framesPerFrame;
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

            if (attackChangeEffectTimer > 0)
                attackChangeEffectTimer--;
            if (projectileThrowTimer > 0)
                projectileThrowTimer--;
            if (throwProjectileSpawnTimer > 0)
                throwProjectileSpawnTimer--;
            if (powerInstallBuff && attackType == Attack_Barrage)
                newPunchTime /= 2;

            if (!powerInstallAnimation && !playerHasAbilityCooldown && SecondSpecialKeyPressed(false))
            {
                if (!player.HasBuff(ModContent.BuffType<PowerInstall>()))
                {
                    player.AddBuff(ModContent.BuffType<PowerInstall>(), 2 * 60 * 60);
                    powerInstallAnimation = true;
                }
                else
                    player.ClearBuff(ModContent.BuffType<PowerInstall>());
            }

            powerInstallBuff = player.HasBuff(ModContent.BuffType<PowerInstall>());
            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft && !throwingProjectile)
                    {
                        if (attackType == Attack_Barrage)
                        {
                            Punch(ModContent.ProjectileType<FanStandFists>(), new Vector2(mouseX, mouseY), afterImages: false);
                            if (attacking)
                                currentAnimationState = AnimationState.Attack;
                        }
                        else if (attackType == Attack_Sword)
                        {
                            Vector2 targetPosition = Main.MouseWorld;
                            if (JoJoStands.JoJoStands.StandAimAssist)
                            {
                                float lowestDistance = 4f * 16f;
                                for (int n = 0; n < Main.maxNPCs; n++)
                                {
                                    NPC npc = Main.npc[n];
                                    if (npc.active && npc.CanBeChasedBy(this, false))
                                    {
                                        float distance = Vector2.Distance(npc.Center, Main.MouseWorld);
                                        if (distance < lowestDistance && Collision.CanHitLine(Projectile.Center, Projectile.width, Projectile.height, npc.position, npc.width, npc.height) && npc.lifeMax > 5 && !npc.immortal && !npc.hide && !npc.townNPC && !npc.friendly)
                                        {
                                            targetPosition = npc.Center;
                                            lowestDistance = distance;
                                        }
                                    }
                                }
                            }

                            if (!mPlayer.canStandBasicAttack)
                            {
                                currentAnimationState = AnimationState.Idle;
                                return;
                            }

                            attacking = true;
                            currentAnimationState = AnimationState.Attack;
                            float rotaY = targetPosition.Y - Projectile.Center.Y;
                            Projectile.rotation = MathHelper.ToRadians((rotaY * Projectile.spriteDirection) / 6f);
                            Vector2 velocityAddition = targetPosition - Projectile.Center;
                            velocityAddition.Normalize();
                            velocityAddition *= 5f + mPlayer.standTier;

                            Projectile.spriteDirection = Projectile.direction = targetPosition.X > Projectile.Center.X ? 1 : -1;
                            float targetDistance = Vector2.Distance(targetPosition, Projectile.Center);
                            if (targetDistance > 16f)
                                Projectile.velocity = player.velocity + velocityAddition;
                            else
                                Projectile.velocity = Vector2.Zero;

                            PlayPunchSound();
                            if (!powerInstallBuff)
                            {
                                if (shootCount <= 0 && (Projectile.frame == 2 || Projectile.frame == 8 || Projectile.frame == 14))
                                {
                                    int rectWidth = 128;
                                    int rectHeight = 96;
                                    int rectXPosition = Projectile.direction == 1 ? (int)Projectile.position.X : (int)Projectile.position.X - rectWidth;
                                    Rectangle attackHitbox = new Rectangle(rectXPosition, (int)Projectile.position.Y - (rectHeight / 2), rectWidth, rectHeight);
                                    shootCount += newPunchTime / 2;
                                    for (int n = 0; n < Main.maxNPCs; n++)
                                    {
                                        NPC npc = Main.npc[n];
                                        if (npc.CanBeChasedBy(this) && npc.Hitbox.Intersects(attackHitbox))
                                        {
                                            NPC.HitInfo hitInfo = new NPC.HitInfo()
                                            {
                                                Damage = (int)(newPunchDamage * 1.5f),
                                                Knockback = PunchKnockback * 1.5f,
                                                HitDirection = npc.direction
                                            };
                                            npc.StrikeNPC(hitInfo);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Projectile.frame == 16)
                                {
                                    int rectWidth = 382;
                                    int rectHeight = 357;
                                    int rectXPosition = Projectile.direction == 1 ? (int)Projectile.position.X : (int)Projectile.position.X - rectWidth;
                                    Rectangle attackHitbox = new Rectangle(rectXPosition, (int)Projectile.position.Y - (rectHeight / 2), rectWidth, rectHeight);
                                    for (int n = 0; n < Main.maxNPCs; n++)
                                    {
                                        NPC npc = Main.npc[n];
                                        if (npc.CanBeChasedBy(this) && npc.Hitbox.Intersects(attackHitbox))
                                        {
                                            int damage = newPunchDamage * 4;
                                            NPC.HitInfo hitInfo = new NPC.HitInfo()
                                            {
                                                Damage = damage,
                                                Knockback = PunchKnockback * 4f,
                                                HitDirection = npc.direction
                                            };
                                            npc.StrikeNPC(hitInfo);
                                        }
                                    }
                                }
                            }

                            LimitDistance();
                            Projectile.netUpdate = true;
                        }
                        else if (attackType == Attack_Cannon)
                        {
                            if (!mPlayer.canStandBasicAttack)
                            {
                                currentAnimationState = AnimationState.Idle;
                                return;
                            }

                            attacking = true;
                            currentAnimationState = AnimationState.Attack;
                            Projectile.netUpdate = true;
                            player.direction = Projectile.spriteDirection = Projectile.direction = Main.MouseWorld.X > player.Center.X ? 1 : -1;
                            GoInFront();
                            if (shootCount <= 0)
                            {
                                shootCount += newPunchTime * 2;
                                Vector2 armOffset = StandOffset - new Vector2(0f, HalfStandHeight * 2) + ArmPlacementOffset + new Vector2(0f, 4f);
                                armOffset.X *= Projectile.spriteDirection;
                                Vector2 circularOffset = Main.MouseWorld - (Projectile.Center + armOffset);
                                circularOffset.Normalize();
                                armOffset += circularOffset * 12f;

                                Vector2 shootPosition = Projectile.Center + armOffset;
                                Vector2 shootVel = Main.MouseWorld - shootPosition;
                                shootVel.Normalize();
                                shootVel *= ProjectileSpeed;
                                int projectileToShoot = powerInstallBuff ? ModContent.ProjectileType<ChargedShot1>() : ModContent.ProjectileType<GunPellet>();
                                int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition, shootVel, projectileToShoot, newPunchDamage / 3, 1f, Projectile.owner);
                                Main.projectile[projIndex].netUpdate = true;
                                Projectile.netUpdate = true;
                                SoundEngine.PlaySound(ShootSound, Projectile.Center);
                            }
                        }
                    }
                    else
                    {
                        attacking = false;
                        currentAnimationState = AnimationState.Idle;
                        if (attackType == Attack_Barrage)
                        {
                            if (!secondaryAbility)
                                StayBehind();
                            secondaryAbility = false;
                        }
                        else if (attackType == Attack_Sword)
                        {
                            if (!secondaryAbility)
                                StayBehind();
                            secondaryAbility = false;
                        }
                        else if (attackType == Attack_Cannon)
                        {
                            if (!attacking)
                                StayBehind();
                            else
                                GoInFront();
                        }
                    }
                    if (Main.mouseRight)
                    {
                        secondaryAbility = true;
                        currentAnimationState = AnimationState.SecondaryAbility;
                        if (attackType == Attack_Barrage)       //throw
                        {
                            if (!throwingProjectile)
                            {
                                throwingProjectile = true;
                                portalSpawned = false;
                                throwAnimationOverride = false;
                                portalAnimationIndex = 0;
                            }
                        }
                        else if (attackType == Attack_Sword)        //mega slash
                        {
                            Vector2 targetPosition = Main.MouseWorld;
                            if (JoJoStands.JoJoStands.StandAimAssist)
                            {
                                float lowestDistance = 4f * 16f;
                                for (int n = 0; n < Main.maxNPCs; n++)
                                {
                                    NPC npc = Main.npc[n];
                                    if (npc.active && npc.CanBeChasedBy(this, false))
                                    {
                                        float distance = Vector2.Distance(npc.Center, Main.MouseWorld);
                                        if (distance < lowestDistance && Collision.CanHitLine(Projectile.Center, Projectile.width, Projectile.height, npc.position, npc.width, npc.height) && npc.lifeMax > 5 && !npc.immortal && !npc.hide && !npc.townNPC && !npc.friendly)
                                        {
                                            targetPosition = npc.Center;
                                            lowestDistance = distance;
                                        }
                                    }
                                }
                            }

                            Vector2 velocityAddition = targetPosition - Projectile.Center;
                            velocityAddition.Normalize();
                            velocityAddition *= 5f + mPlayer.standTier;

                            Projectile.spriteDirection = Projectile.direction = targetPosition.X > Projectile.Center.X ? 1 : -1;
                            float targetDistance = Vector2.Distance(targetPosition, Projectile.Center);
                            if (targetDistance > 16f)
                                Projectile.velocity = player.velocity + velocityAddition;
                            else
                                Projectile.velocity = Vector2.Zero;

                            if (shootCount <= 0 && Projectile.frame != oldSpinFrame)
                            {
                                shootCount += newPunchTime * 3 / 4;
                                oldSpinFrame = Projectile.frame;
                                int rectWidth = 80;
                                int rectHeight = 80;
                                float angle = (float)Math.Cos((Projectile.frame / 6) * 2 * MathHelper.Pi);
                                Vector2 rectPosition = Projectile.Center + (angle.ToRotationVector2() * (rectWidth / 2));
                                Rectangle attackHitbox = new Rectangle((int)(rectPosition.X) - (rectWidth / 2), (int)rectPosition.Y - (rectHeight / 2), rectWidth, rectHeight);
                                for (int n = 0; n < Main.maxNPCs; n++)
                                {
                                    NPC npc = Main.npc[n];
                                    if (npc.CanBeChasedBy(this) && npc.Hitbox.Intersects(attackHitbox))
                                    {
                                        int damage = newPunchDamage * 5 / 4;
                                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                                        {
                                            Damage = damage,
                                            Knockback = PunchKnockback * 4f,
                                            HitDirection = npc.direction
                                        };
                                        npc.StrikeNPC(hitInfo);
                                    }
                                }
                            }
                        }
                        else if (attackType == Attack_Cannon)       //Charged shots 1 (2s) & 2 (5s)
                        {
                            mouseRightHoldTimer++;
                            Vector2 dustSpawnPosition = Projectile.position - new Vector2(0f, HalfStandHeight);
                            if (Projectile.direction == 1)
                                dustSpawnPosition += StandOffset;
                            else
                                dustSpawnPosition -= StandOffset;

                            if (Main.rand.Next(0, 1 + 1) == 0)
                                Main.dust[Dust.NewDust(dustSpawnPosition, Projectile.width, HalfStandHeight * 2, DustID.CoralTorch)].noGravity = true;
                            if (mouseRightHoldTimer >= 3 * 60)
                            {
                                if (Main.rand.Next(0, 1 + 1) == 0)
                                    Main.dust[Dust.NewDust(dustSpawnPosition, Projectile.width, HalfStandHeight * 2, DustID.Firework_Green)].noGravity = true;
                                if (powerInstallBuff && Main.rand.Next(0, 1 + 1) == 0)
                                    Main.dust[Dust.NewDust(dustSpawnPosition, Projectile.width, HalfStandHeight * 2, DustID.Electric)].noGravity = true;
                            }
                        }
                    }
                }

                if (Main.myPlayer == Projectile.owner && !Main.mouseRight && mouseRightHoldTimer > 0)
                {
                    if (mouseRightHoldTimer >= 1 * 60)
                    {
                        int multiplier = 1;
                        int chargedProjectile = ModContent.ProjectileType<ChargedShot1>();
                        if (mouseRightHoldTimer >= 3 * 60)
                        {
                            multiplier = 2;
                            chargedProjectile = ModContent.ProjectileType<ChargedShot2>();
                        }

                        if (powerInstallBuff)
                        {
                            chargedProjectile = ModContent.ProjectileType<ChargedShot2>();
                            if (mouseRightHoldTimer >= 3 * 60)
                                chargedProjectile = ModContent.ProjectileType<ChargedShot3>();
                        }

                        shootCount += newShootTime * 3;
                        Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                        if (shootVel == Vector2.Zero)
                            shootVel = new Vector2(0f, 1f);

                        shootVel.Normalize();
                        shootVel *= ProjectileSpeed;
                        int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, chargedProjectile, newPunchDamage * 2 * multiplier, 3f, Projectile.owner);
                        Main.projectile[projIndex].netUpdate = true;
                        Projectile.netUpdate = true;
                    }
                    mouseRightHoldTimer = 0;
                }

                if (throwingProjectile)
                {
                    secondaryAbility = true;
                    currentAnimationState = AnimationState.SecondaryAbility;
                    Projectile.position = player.Center - new Vector2(0f, (HalfStandHeight * 2) + 8f);
                    if (Projectile.spriteDirection == -1)
                        Projectile.position.X -= 32;

                    if (!portalSpawned)
                    {
                        portalFrameCounter++;
                        if (portalFrameCounter >= portalAnimations[portalAnimationIndex].frameDuration)
                        {
                            portalFrame += 1;
                            portalFrameCounter = 0;
                            OnPortalFrameChange();
                            if (portalFrame >= portalAnimations[portalAnimationIndex].maxFrames)
                            {
                                portalFrame = 0;
                                portalFrameCounter = 0;
                                portalAnimationIndex++;
                                if (portalAnimationIndex >= 3)
                                {
                                    portalAnimationIndex = 2;
                                    portalSpawned = true;
                                }
                            }
                        }
                    }

                    if (throwTimer > 0)
                        throwTimer--;

                    if ((Projectile.frame >= 2 && (portalAnimationIndex == 0 || (portalAnimationIndex == 1 && portalFrame < 4))) || (throwAnimationOverride && throwTimer > 0))
                    {
                        Projectile.frame = 1;
                        Projectile.frameCounter = 0;
                    }
                }


                if (SpecialKeyPressed(false))
                {
                    attackType++;
                    if (attackType > Attack_Cannon)
                        attackType = Attack_Barrage;

                    fPlayer.virtualInsanityRangeBoost = attackType == Attack_Sword;
                    Main.NewText("Attack Style: " + AttackStyleNames[attackType]);
                    attackChangeEffectTimer = 90;
                }

                if (mPlayer.posing)
                    currentAnimationState = AnimationState.Pose;

                if (powerInstallAnimation)
                    currentAnimationState = AnimationState.PowerInstall;

                LimitDistance();
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                if (attackType != Attack_Barrage)
                {
                    attackType = Attack_Barrage;
                    Main.NewText("Attack Style: " + AttackStyleNames[attackType]);
                    attackChangeEffectTimer = 90;
                }

                BasicPunchAI();
                if (attacking)
                    currentAnimationState = AnimationState.Attack;
                else
                    currentAnimationState = AnimationState.Idle;
            }
        }

        public override bool PreDrawExtras()
        {
            Color bodyColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
            if (attackChangeEffectTimer > 0)
            {
                Vector2 drawOffset = StandOffset - new Vector2(0f, HalfStandHeight * 2);
                drawOffset.X *= Projectile.spriteDirection;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                Main.EntitySpriteDraw(AttackStyleTextures[attackType], drawPosition, null, bodyColor * Math.Clamp(attackChangeEffectTimer / 60f, 0f, 1f), 0f, new Vector2(43), 1f, SpriteEffects.None);
            }
            if (attackType == Attack_Cannon)
            {
                Vector2 drawOffset = StandOffset - new Vector2(0f, HalfStandHeight * 2) + ArmPlacementOffset;
                drawOffset.X *= Projectile.spriteDirection;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                float rotation = (Main.MouseWorld - (Projectile.Center - new Vector2(29, 39) + drawOffset + ArmPlacementOffset)).ToRotation();

                int textureType = 0;
                Rectangle animRect = new Rectangle(0, 0, 32, 32);
                if (currentAnimationState == AnimationState.Attack)
                {
                    textureType = 1;
                    animRect = new Rectangle(0, 32 * Projectile.frame, 32, 32);
                }

                Vector2 armOrigin = new Vector2(4, 20);
                if (Projectile.spriteDirection == -1)
                    armOrigin.Y -= 8;

                Main.EntitySpriteDraw(ArmCannonSpritesheets[textureType], drawPosition, animRect, bodyColor, rotation, armOrigin, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            }

            return true;
        }

        public override void PostDrawExtras()
        {
            Color bodyColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
            if (throwingProjectile && !portalSpawned)
            {
                Vector2 drawOffset = StandOffset - new Vector2(0f, HalfStandHeight * 2);
                drawOffset.X *= Projectile.spriteDirection;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                Main.EntitySpriteDraw(PortalTextures[portalAnimationIndex], drawPosition, new Rectangle(0, 100 * portalFrame, 100, 100), Color.White, 0f, new Vector2(50), 1f, SpriteEffects.None);
            }
            if (attackType == Attack_Cannon)
            {
                effects = SpriteEffects.None;
                if (Projectile.spriteDirection == -1)
                    effects = SpriteEffects.FlipHorizontally;

                if (standTexture != null && Main.netMode != NetmodeID.Server)
                {
                    int frameHeight = standTexture.Height / amountOfFrames;
                    Vector2 drawOffset = StandOffset;
                    drawOffset.X *= Projectile.spriteDirection;
                    Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                    Rectangle animRect = new Rectangle(0, frameHeight * Projectile.frame, standTexture.Width, frameHeight);
                    Vector2 standOrigin = new Vector2(standTexture.Width / 2f, frameHeight / 2f);
                    int textureIndex = Math.Clamp((int)((Main.MouseScreen.Y / Main.screenHeight) * 3), 0, 2);
                    if (currentAnimationState != AnimationState.Attack)
                        Main.EntitySpriteDraw(CannonHeadSpritesheets[textureIndex], drawPosition, animRect, bodyColor, Projectile.rotation, standOrigin, Projectile.scale, effects, 0);
                    else if (currentAnimationState == AnimationState.Attack)
                        Main.EntitySpriteDraw(CannonHeadFlashSpritesheets[textureIndex], drawPosition, animRect, bodyColor, Projectile.rotation, standOrigin, Projectile.scale, effects, 0);
                }
            }
        }

        private void OnPortalFrameChange()
        {
            if (portalAnimationIndex == 1 && portalFrame == 4)
            {
                int randomIndex = Main.rand.Next(0, ThrowProjectiles.Length);
                randomIndex = 3;
                Vector2 projectileCenter = Projectile.Center + StandOffset - new Vector2(0f, HalfStandHeight / 2f) + ThrowProjectilesOffset[randomIndex];
                Vector2 shootVelocity = Main.MouseWorld - projectileCenter;
                shootVelocity.Normalize();
                shootVelocity *= 16f;
                int damage = 0;
                float knockback = 0f;
                if (randomIndex == 0)
                    shootVelocity = Vector2.Zero;
                else if (randomIndex == 1)
                {
                    damage = 48 * TierNumber;
                    knockback = 2f * TierNumber;
                    shootVelocity = Vector2.Zero;
                }
                else if (randomIndex == 3)       //Glue man
                {
                    throwTimer = 60;
                    throwAnimationOverride = true;
                }
                else if (randomIndex == 4)
                {
                    damage = 60 * TierNumber;
                    knockback = 8f * TierNumber;
                }
                projectileToThrow = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(), projectileCenter, shootVelocity, ThrowProjectiles[randomIndex], damage, knockback, Projectile.owner, newPunchDamage * 3 / 4)];
                projectileToThrow.spriteDirection = projectileToThrow.direction = -Projectile.spriteDirection;
                projectileToThrow.alpha = 255;
                Main.player[Projectile.owner].AddBuff(ModContent.BuffType<AbilityCooldown>() ,Main.player[Projectile.owner].GetModPlayer<MyPlayer>().AbilityCooldownTime(15 / TierNumber));
            }
        }

        public override void AnimationCompleted(string animationName)
        {
            if (animationName == "Throw")
                throwingProjectile = false;
            else if (animationName == "PowerInstall")
                powerInstallAnimation = false;
        }

        public override void StandKillEffects()
        {
            Main.player[Projectile.owner].GetModPlayer<FanPlayer>().virtualInsanityRangeBoost = false;
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(attackType);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            attackType = reader.ReadByte();
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
            {
                if (attackType == Attack_Barrage)
                    PlayAnimation("Throw");
                else if (attackType == Attack_Sword)
                    PlayAnimation("Spin");
                else if (attackType == Attack_Cannon)
                    PlayAnimation("Idle");
            }
            else if (currentAnimationState == AnimationState.Special)
                PlayAnimation("PowerInstall");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/" + AttackStyleNames[attackType] + "_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, AttackStyleIdleFrameAmounts[attackType], 14, true);
            else if (animationName == "Attack")
            {
                int frameTime = newPunchTime * 3 / 4;
                if (attackType == Attack_Cannon)
                    frameTime = newPunchTime / 2;
                AnimateStand(animationName, AttackStyleAttackFrameAmounts[attackType], frameTime, true);
            }
            else if (animationName == "Secondary")
                AnimateStand(animationName, 1, 15, true);
            else if (animationName == "Throw")
                AnimateStand(animationName, 6, 8, false);
            else if (animationName == "Spin")
                AnimateStand(animationName, 7, 2, true);
            else if (animationName == "PowerInstall")
                AnimateStand(animationName, 17, 8, false);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 300, false);
        }
    }
}