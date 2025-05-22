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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity
{
    public class VirtualInsanityStandT3 : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 62;
        public override int PunchTime => 10;
        public override int TierNumber => 3;
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

        private const float PowerInstallDuration = 60f * 60f;

        private int mouseRightHoldTimer = 0;
        private bool powerInstallBuff = false;
        private int attackChangeEffectTimer = 0;
        private bool throwingProjectile;
        private int projectileThrowTimer;
        private int oldSlashFrame = 0;
        private int throwTimer = 0;

        private byte attackType = Attack_Barrage;

        private const byte Attack_Barrage = 0;
        private const byte Attack_Sword = 1;
        private const byte Attack_Cannon = 2;
        private readonly string[] AttackStyleNames = new string[3] { "Barrage", "Sword", "Cannon" };
        private readonly int[] AttackStyleIdleFrameAmounts = new int[3] { 5, 5, 4 };
        private readonly int[] AttackStyleAttackFrameAmounts = new int[3] { 17, 18, 4 };
        private readonly int[] ThrowProjectiles = new int[5] { ModContent.ProjectileType<YellowDevil>(), ModContent.ProjectileType<GreenDevil>(), ModContent.ProjectileType<BombTelly>(), ModContent.ProjectileType<GlueMan>(), ModContent.ProjectileType<PowerMuscler>() };
        private readonly Vector2[] ThrowProjectilesOffset = new Vector2[5] { new Vector2(16, -63), new Vector2(16, -53), new Vector2(36, -66), new Vector2(12, -46), new Vector2(16, -58) };
        private readonly Vector2 ArmPlacementOffset = new Vector2(40 - 29, 18 + 39);
        private int portalFrame;
        private int portalFrameCounter;
        private int portalAnimationIndex;
        private bool portalSpawned = false;
        private bool throwAnimationOverride = false;
        private int throwProjectileSpawnTimer = 0;
        private bool powerInstallAnimation = false;
        private int otherClientMouseRightHoldStage = 0;
        private Projectile projectileToThrow;
        private int powerInstallAuraTimer = 0;
        private bool performingBigSlash = false;
        private int bigSlashDirection = 0;
        private int bigSlashFrame = 0;
        private int lightningFrameOffset = 0;
        private int lightningFrameCounter = 0;
        private int lightningShowTimer = 0;
        private int lightningShowTime = 0;
        private List<VirtualInsanityStandFinal.LightningData> lightningDatas = new List<VirtualInsanityStandFinal.LightningData>();

        public static readonly SoundStyle PowerInstallThemeExit = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/VirtualInsanity/PowerInstallExit_3")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };
        public static SoundEffect PowerInstallTheme;
        public static SoundEffectInstance PowerInstallThemeInstance;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (attackType == Attack_Sword)
                mPlayer.standRangeBoosts += 4f * 16f;

            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (attackChangeEffectTimer > 0)
                attackChangeEffectTimer--;
            if (projectileThrowTimer > 0)
                projectileThrowTimer--;
            if (throwProjectileSpawnTimer > 0)
                throwProjectileSpawnTimer--;
            if (lightningShowTimer > 0)
                lightningShowTimer--;
            if (powerInstallBuff && attackType == Attack_Barrage)
                newPunchTime /= 2;
            if (powerInstallBuff && powerInstallAuraTimer < 56)
            {
                powerInstallAuraTimer++;
                if (powerInstallAuraTimer >= 56)
                    powerInstallAuraTimer = 0;
            }
            if (lightningFrameCounter > 0)
            {
                lightningFrameCounter--;
                if (lightningFrameCounter <= 0)
                {
                    lightningFrameCounter = 6;
                    lightningFrameOffset++;
                    if (lightningFrameOffset >= 2)
                        lightningFrameOffset = 0;
                }
            }

            bool canPerformAction = !throwingProjectile && !performingBigSlash && !powerInstallAnimation;
            if (canPerformAction && !playerHasAbilityCooldown && SecondSpecialKeyPressed(false))
            {
                if (!player.HasBuff(ModContent.BuffType<PowerInstall>()))
                {
                    player.AddBuff(ModContent.BuffType<PowerInstall>(), (int)PowerInstallDuration);
                    powerInstallAnimation = true;
                    attackChangeEffectTimer = 60;
                    int amountOfLightning = Main.rand.Next(4, 8 + 1);
                    for (int i = 0; i < amountOfLightning; i++)
                    {
                        AddLightning(Projectile.Center - new Vector2(Main.rand.Next(-120, 120 + 1), (Main.screenHeight * 3 / 4)), Projectile.Center + StandOffset);
                        if (JoJoFanStands.SoundsLoaded && i % 2 == 0)
                            SoundEngine.PlaySound(VirtualInsanityStandFinal.ElectricitySounds[Main.rand.Next(0, VirtualInsanityStandFinal.ElectricitySounds.Length)], Projectile.Center);
                    }

                    if (JoJoFanStands.SoundsLoaded)
                        SoundEngine.PlaySound(VirtualInsanityStandFinal.PowerInstall, Projectile.Center);
                }
                else
                {
                    if (player.HasBuff(ModContent.BuffType<PowerInstall>()))
                    {
                        float powerInstallCompletion = (PowerInstallDuration - player.buffTime[player.FindBuffIndex(ModContent.BuffType<PowerInstall>())]) / PowerInstallDuration;
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), (int)(5 * 60 * 60 * powerInstallCompletion));
                        player.ClearBuff(ModContent.BuffType<PowerInstall>());
                    }
                }
            }

            if (JoJoFanStands.SoundsLoaded && Projectile.owner == Main.myPlayer)
            {
                if (PowerInstallThemeInstance == null)
                    PowerInstallThemeInstance = PowerInstallTheme.CreateInstance();
                else
                {
                    if (powerInstallBuff)
                    {
                        if (PowerInstallThemeInstance.State != SoundState.Playing)
                            PowerInstallThemeInstance.Play();
                    }
                    else
                    {
                        if (PowerInstallThemeInstance.State != SoundState.Stopped)
                            PowerInstallThemeInstance.Stop();
                    }
                }
            }

            bool oldPowerInstallValue = powerInstallBuff;
            powerInstallBuff = player.HasBuff(ModContent.BuffType<PowerInstall>());
            if (JoJoFanStands.SoundsLoaded && oldPowerInstallValue && !powerInstallBuff)
                SoundEngine.PlaySound(PowerInstallThemeExit, Projectile.Center);

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft && canPerformAction)
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
                                            NetMessage.SendStrikeNPC(npc, hitInfo);
                                        }
                                    }
                                    SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(-0.8f), Projectile.Center);
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
                                            NetMessage.SendStrikeNPC(npc, hitInfo);
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
                                if (JoJoFanStands.SoundsLoaded)
                                    SoundEngine.PlaySound(VirtualInsanityStandFinal.ShootSound, Projectile.Center);
                                else
                                    SoundEngine.PlaySound(SoundID.Item36.WithPitchOffset(-0.8f), Projectile.Center);
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
                    if (Main.mouseRight && canPerformAction)
                    {
                        Projectile.netUpdate = true;
                        if (attackType == Attack_Barrage)       //throw
                        {
                            if (!playerHasAbilityCooldown)
                            {
                                secondaryAbility = true;
                                currentAnimationState = AnimationState.SecondaryAbility;
                                throwingProjectile = true;
                                portalSpawned = false;
                                throwAnimationOverride = false;
                                portalAnimationIndex = 0;
                                portalFrameCounter = 0;
                                portalFrame = 0;
                            }
                        }
                        else if (attackType == Attack_Sword)        //mega slash
                        {
                            secondaryAbility = true;
                            currentAnimationState = AnimationState.SecondaryAbility;
                            if (!powerInstallBuff)
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
                                Projectile.netUpdate = true;

                                if (shootCount <= 0 && Projectile.frame != oldSlashFrame)
                                {
                                    shootCount += newPunchTime * 3 / 4;
                                    oldSlashFrame = Projectile.frame;
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
                                            NetMessage.SendStrikeNPC(npc, hitInfo);
                                        }
                                    }
                                    SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(0.6f), Projectile.Center);
                                }
                            }
                            else
                            {
                                performingBigSlash = true;
                                bigSlashDirection = player.Center.X < Main.MouseWorld.X ? 1 : -1;
                                if (Projectile.owner == Main.myPlayer)
                                    standTexture = VirtualInsanityStandFinal.BigSlashFrames[0];
                            }
                        }
                        else if (attackType == Attack_Cannon)       //Charged shots 1 (2s) & 2 (5s)
                        {
                            secondaryAbility = true;
                            currentAnimationState = AnimationState.SecondaryAbility;
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
                            if (mouseRightHoldTimer % 60 == 0)
                                Projectile.netUpdate = true;
                        }
                    }
                }

                if (Main.myPlayer == Projectile.owner && !Main.mouseRight && mouseRightHoldTimer > 0)
                {
                    if (mouseRightHoldTimer >= 1 * 60)
                    {
                        int multiplier = 1;
                        int chargedProjectile = ModContent.ProjectileType<ChargedShot1>();
                        if (powerInstallBuff)
                        {
                            if (mouseRightHoldTimer >= 3 * 60)
                            {
                                chargedProjectile = ModContent.ProjectileType<ChargedShot3>();
                                if (JoJoFanStands.SoundsLoaded)
                                    SoundEngine.PlaySound(VirtualInsanityStandFinal.ChargeShot3, Projectile.Center);
                                else
                                    SoundEngine.PlaySound(SoundID.Item36.WithPitchOffset(-1f), Projectile.Center);
                            }
                            else
                            {
                                chargedProjectile = ModContent.ProjectileType<ChargedShot2>();
                                if (JoJoFanStands.SoundsLoaded)
                                    SoundEngine.PlaySound(VirtualInsanityStandFinal.ChargeShot2, Projectile.Center);
                                else
                                    SoundEngine.PlaySound(SoundID.Item36.WithPitchOffset(-0.9f), Projectile.Center);
                            }
                        }
                        else
                        {
                            if (mouseRightHoldTimer >= 3 * 60)
                            {
                                multiplier = 2;
                                chargedProjectile = ModContent.ProjectileType<ChargedShot2>();
                                if (JoJoFanStands.SoundsLoaded)
                                    SoundEngine.PlaySound(VirtualInsanityStandFinal.ChargeShot2, Projectile.Center);
                                else
                                    SoundEngine.PlaySound(SoundID.Item36.WithPitchOffset(-0.9f), Projectile.Center);
                            }
                            else
                            {
                                if (JoJoFanStands.SoundsLoaded)
                                    SoundEngine.PlaySound(VirtualInsanityStandFinal.ChargeShot1, Projectile.Center);
                                else
                                    SoundEngine.PlaySound(SoundID.Item36.WithPitchOffset(-0.8f), Projectile.Center);
                            }
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
                if (Main.myPlayer != Projectile.owner && otherClientMouseRightHoldStage > 0)
                {
                    Vector2 dustSpawnPosition = Projectile.position - new Vector2(0f, HalfStandHeight);
                    if (Projectile.direction == 1)
                        dustSpawnPosition += StandOffset;
                    else
                        dustSpawnPosition -= StandOffset;

                    if (Main.rand.Next(0, 1 + 1) == 0)
                        Main.dust[Dust.NewDust(dustSpawnPosition, Projectile.width, HalfStandHeight * 2, DustID.CoralTorch)].noGravity = true;
                    if (otherClientMouseRightHoldStage > 3)
                    {
                        if (Main.rand.Next(0, 1 + 1) == 0)
                            Main.dust[Dust.NewDust(dustSpawnPosition, Projectile.width, HalfStandHeight * 2, DustID.Firework_Green)].noGravity = true;
                        if (powerInstallBuff && Main.rand.Next(0, 1 + 1) == 0)
                            Main.dust[Dust.NewDust(dustSpawnPosition, Projectile.width, HalfStandHeight * 2, DustID.Electric)].noGravity = true;
                    }
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
                        if (portalFrameCounter >= VirtualInsanityStandFinal.PortalAnimations[portalAnimationIndex].frameDuration)
                        {
                            portalFrame += 1;
                            portalFrameCounter = 0;
                            OnPortalFrameChange();
                            if (portalFrame >= VirtualInsanityStandFinal.PortalAnimations[portalAnimationIndex].maxFrames)
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

                if (performingBigSlash)
                {
                    player.ChangeDir(bigSlashDirection);
                    Projectile.direction = Projectile.spriteDirection = player.direction;
                    GoInFront();

                    currentAnimationState = AnimationState.SecondaryAbility;
                    Projectile.netUpdate = true;
                    if (shootCount <= 0 && (bigSlashFrame == 16 || bigSlashFrame == 18))
                    {
                        shootCount += 6;
                        oldSlashFrame = bigSlashFrame;
                        int rectWidth = 400;
                        int rectHeight = 360;
                        Vector2 rectPosition = Projectile.Center;
                        int xPosition = Projectile.direction == 1 ? (int)rectPosition.X : (int)rectPosition.X - rectWidth;
                        Rectangle attackHitbox = new Rectangle(xPosition, (int)rectPosition.Y - (rectHeight / 2), rectWidth, rectHeight);
                        if (JoJoFanStands.SoundsLoaded)
                            SoundEngine.PlaySound(VirtualInsanityStandFinal.BiggerSlashSwing, Projectile.Center);
                        else
                            SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(-1f), Projectile.Center);

                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.CanBeChasedBy(this) && npc.Hitbox.Intersects(attackHitbox))
                            {
                                int damage = newPunchDamage * 4;
                                NPC.HitInfo hitInfo = new NPC.HitInfo()
                                {
                                    Damage = damage,
                                    Knockback = PunchKnockback * 12f,
                                    HitDirection = npc.direction
                                };
                                npc.StrikeNPC(hitInfo);
                                NetMessage.SendStrikeNPC(npc, hitInfo);
                            }
                        }
                    }
                }

                if (canPerformAction && SpecialKeyPressed(false))
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
                    currentAnimationState = AnimationState.Special;

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

        public void AddLightning(Vector2 start, Vector2 end)
        {
            lightningDatas.Add(new VirtualInsanityStandFinal.LightningData(Main.rand.Next(0, VirtualInsanityStandFinal.LightningSheets.Length), start, end));
        }

        public void AddTrackingLightning(int npcIndex)
        {
            lightningDatas.Add(new VirtualInsanityStandFinal.LightningData(Main.rand.Next(0, VirtualInsanityStandFinal.LightningSheets.Length), Projectile.whoAmI, npcIndex));
        }


        public override bool PreDrawExtras()
        {
            if (lightningShowTimer > 0)
            {
                for (int i = 0; i < lightningDatas.Count; i++)
                {
                    if (lightningDatas[i].startProjectileIndex == -1 && lightningDatas[i].targetNPCIndex == -1)
                        DrawLightning(lightningDatas[i].sheetIndex, lightningDatas[i].startPosition, lightningDatas[i].endPosition);
                    else
                    {
                        if (Main.projectile[lightningDatas[i].startProjectileIndex].active && Main.npc[lightningDatas[i].targetNPCIndex].active)
                            DrawLightning(lightningDatas[i].sheetIndex, Main.projectile[lightningDatas[i].startProjectileIndex].Center, Main.npc[lightningDatas[i].targetNPCIndex].Center);
                    }
                }
            }

            Color bodyColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
            if (!powerInstallAnimation && powerInstallBuff)
            {
                Vector2 standOrigin = new Vector2(standTexture.Width / 2f, 44);
                Vector2 drawOffset = StandOffset;
                drawOffset.X *= Projectile.spriteDirection;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                if (attackType == Attack_Barrage && attacking && Projectile.spriteDirection == -1)
                    drawPosition.X += 24f;
                if (attackType == Attack_Sword)
                {
                    if (attacking)
                    {
                        if (Projectile.spriteDirection == 1)
                            drawPosition.X += 16f;
                        else
                            drawPosition.X += 64f;
                    }
                    if (currentAnimationState == AnimationState.SecondaryAbility)
                    {
                        if (Projectile.spriteDirection == 1)
                            drawPosition.X += 16f;
                        else
                            drawPosition.X += 64f;
                    }
                    drawPosition.Y += 8f;
                }
                if (attackType == Attack_Cannon)
                    drawPosition.X -= 16f;
                if (performingBigSlash)
                {
                    if (Projectile.spriteDirection == -1)
                        drawPosition += new Vector2(96, -20);
                    else
                        drawPosition += new Vector2(180, -20);
                }

                Rectangle animRect = new Rectangle(0, (powerInstallAuraTimer / 8) * 88, 88, 88);
                Main.EntitySpriteDraw(VirtualInsanityStandFinal.PowerInstallAuraSpritesheet, drawPosition, animRect, bodyColor, Projectile.rotation, standOrigin, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }

            if (attackChangeEffectTimer > 0)
            {
                if (powerInstallAnimation)
                {
                    Vector2 drawOffset = StandOffset - new Vector2(0f, HalfStandHeight * 2);
                    drawOffset.X *= Projectile.spriteDirection;
                    Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                    Main.EntitySpriteDraw(VirtualInsanityStandFinal.PowerInstallKanji, drawPosition, null, bodyColor * Math.Clamp(attackChangeEffectTimer / 60f, 0f, 1f), 0f, new Vector2(123, 53), 1f, SpriteEffects.None);
                }
                else
                {

                    Vector2 drawOffset = StandOffset - new Vector2(0f, HalfStandHeight * 2);
                    drawOffset.X *= Projectile.spriteDirection;
                    Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                    Main.EntitySpriteDraw(VirtualInsanityStandFinal.AttackStyleTextures[attackType], drawPosition, null, bodyColor * Math.Clamp(attackChangeEffectTimer / 60f, 0f, 1f), 0f, new Vector2(43), 1f, SpriteEffects.None);
                }
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

                Main.EntitySpriteDraw(VirtualInsanityStandFinal.ArmCannonSpritesheets[textureType], drawPosition, animRect, bodyColor, rotation, armOrigin, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            }

            return true;
        }

        public override void CustomDrawStand(Color drawColor)
        {
            if (UseProjectileAlpha)
                drawColor *= Projectile.alpha / 255f;

            effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;

            if (standTexture != null && Main.netMode != NetmodeID.Server)
            {
                int frameHeight = standTexture.Height / amountOfFrames;
                Vector2 drawOffset = StandOffset;
                drawOffset.X *= Projectile.spriteDirection;
                if (performingBigSlash)
                    drawOffset += new Vector2(180 * Projectile.spriteDirection, -60);

                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                Rectangle animRect = new Rectangle(0, frameHeight * Projectile.frame, standTexture.Width, frameHeight);
                Vector2 standOrigin = new Vector2(standTexture.Width / 2f, frameHeight / 2f);
                Main.EntitySpriteDraw(standTexture, drawPosition, animRect, drawColor, Projectile.rotation, standOrigin, Projectile.scale, effects, 0);
            }
        }

        public override void PostDrawExtras()
        {
            Color bodyColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
            if (throwingProjectile && !portalSpawned)
            {
                Vector2 drawOffset = StandOffset - new Vector2(0f, HalfStandHeight * 2);
                drawOffset.X *= Projectile.spriteDirection;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                Main.EntitySpriteDraw(VirtualInsanityStandFinal.PortalTextures[portalAnimationIndex], drawPosition, new Rectangle(0, 100 * portalFrame, 100, 100), Color.White, 0f, new Vector2(50), 1f, SpriteEffects.None);
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
                        Main.EntitySpriteDraw(VirtualInsanityStandFinal.CannonHeadSpritesheets[textureIndex], drawPosition, animRect, bodyColor, Projectile.rotation, standOrigin, Projectile.scale, effects, 0);
                    else if (currentAnimationState == AnimationState.Attack)
                        Main.EntitySpriteDraw(VirtualInsanityStandFinal.CannonHeadFlashSpritesheets[textureIndex], drawPosition, animRect, bodyColor, Projectile.rotation, standOrigin, Projectile.scale, effects, 0);
                }
            }
        }

        private void DrawLightning(int sheetIndex, Vector2 start, Vector2 end)
        {
            float rotation = (end - start).ToRotation() + 3.14f / 2;
            float increment = 1 / (Vector2.Distance(start, end) / 48);
            for (float k = increment; k <= 1; k += increment)     //basically, getting the amount of space between the 2 points, dividing it by the textures width, then making it a fraction, so saying you 'each takes 1/x space, make x of them to fill it up to 1'
            {
                int randomFrame = lightningFrameOffset + (int)(2 * (4f * k));
                Vector2 pos = Vector2.Lerp(start, end, k) - Main.screenPosition;       //getting the distance and making points by 'k', then bringing it into view
                Main.EntitySpriteDraw(VirtualInsanityStandFinal.LightningSheets[sheetIndex], pos, new Rectangle(0, randomFrame * 64, 48, 64), Color.White * (lightningShowTimer / (float)lightningShowTime), rotation, new Vector2(24, 32), Projectile.scale, SpriteEffects.None, 0);
            }
        }

        private void OnPortalFrameChange()
        {
            if (JoJoFanStands.SoundsLoaded && portalAnimationIndex == 0 && portalFrame == 1)
                SoundEngine.PlaySound(VirtualInsanityStandFinal.PortalOpen, Projectile.Center);

            if (Main.myPlayer == Projectile.owner && portalAnimationIndex == 1 && portalFrame == 4)
            {
                int randomIndex = Main.rand.Next(0, ThrowProjectiles.Length);
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
                Main.player[Projectile.owner].AddBuff(ModContent.BuffType<AbilityCooldown>(), Main.player[Projectile.owner].GetModPlayer<MyPlayer>().AbilityCooldownTime(25 / TierNumber));
                if (JoJoFanStands.SoundsLoaded)
                    SoundEngine.PlaySound(Main.rand.Next(0, 1 + 1) == 0 ? VirtualInsanityStandFinal.ThrowableSpawn1 : VirtualInsanityStandFinal.ThrowableSpawn2, Projectile.Center);
            }
            if (JoJoFanStands.SoundsLoaded && Main.myPlayer != Projectile.owner && portalAnimationIndex == 1 && portalFrame == 4)
                SoundEngine.PlaySound(Main.rand.Next(0, 1 + 1) == 0 ? VirtualInsanityStandFinal.ThrowableSpawn1 : VirtualInsanityStandFinal.ThrowableSpawn2, Projectile.Center);
            if (portalAnimationIndex == 2 && portalFrame == 1)
            {
                if (JoJoFanStands.SoundsLoaded)
                    SoundEngine.PlaySound(VirtualInsanityStandFinal.PortalClose, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                    Projectile.netUpdate = true;
            }
        }

        public override void AnimationCompleted(string animationName)
        {
            if (animationName == "Throw")
                throwingProjectile = false;
            else if (animationName == "PowerInstall")
                powerInstallAnimation = false;
            else if (animationName == "BigSlashEmpty")
            {
                bigSlashFrame++;
                if (bigSlashFrame >= 25)
                {
                    bigSlashFrame = 0;
                    performingBigSlash = false;
                }

                standTexture = VirtualInsanityStandFinal.BigSlashFrames[bigSlashFrame];
            }

        }

        public override void StandKillEffects()
        {
            Main.player[Projectile.owner].GetModPlayer<FanPlayer>().virtualInsanityRangeBoost = false;
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(attackType);
            writer.Write(powerInstallAnimation);
            writer.Write(mouseRightHoldTimer / 60);
            writer.Write(throwingProjectile);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            attackType = reader.ReadByte();
            powerInstallAnimation = reader.ReadBoolean();
            otherClientMouseRightHoldStage = reader.ReadInt32();
            throwingProjectile = reader.ReadBoolean();
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
                {
                    if (!powerInstallBuff)
                        PlayAnimation("Spin");
                    else
                        PlayAnimation("BigSlashEmpty");
                }
                else if (attackType == Attack_Cannon)
                    PlayAnimation("Idle");
            }
            else if (currentAnimationState == AnimationState.Special)
                PlayAnimation("PowerInstall");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            if (!performingBigSlash)
            {
                if (currentAnimationState == AnimationState.Special || currentAnimationState == AnimationState.Pose)
                    standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/" + animationName).Value;
                else
                    standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/" + AttackStyleNames[attackType] + "_" + animationName).Value;
            }

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
            else if (animationName == "BigSlashEmpty")
                AnimateStand(animationName, 1, 5, false);
            else if (animationName == "PowerInstall")
                AnimateStand(animationName, 17, 4, false);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 300, false);
        }
    }
}