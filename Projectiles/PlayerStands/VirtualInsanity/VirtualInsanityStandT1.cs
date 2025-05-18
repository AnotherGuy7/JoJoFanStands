using JoJoStands;
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
    public class VirtualInsanityStandT1 : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 20;
        public override int PunchTime => 12;
        public override int TierNumber => 1;
        public override bool CanUseAfterImagePunches => false;
        public override Vector2 StandOffset => new Vector2(-24, 0);
        public override int FistID => FanStandFists.VirtualInsanityFists;
        public override StandAttackType StandType => StandAttackType.Melee;

        private int attackChangeEffectTimer = 0;
        private byte attackType = Attack_Barrage;

        private const byte Attack_Barrage = 0;
        private const byte Attack_Sword = 1;
        private const byte Attack_Cannon = 2;
        private readonly string[] AttackStyleNames = new string[3] { "Barrage", "Sword", "Cannon" };
        private readonly int[] AttackStyleIdleFrameAmounts = new int[3] { 5, 5, 4 };
        private readonly int[] AttackStyleAttackFrameAmounts = new int[3] { 17, 18, 4 };
        private readonly Vector2 ArmPlacementOffset = new Vector2(40 - 29, 18 + 39);

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

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft)
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
                                int projectileToShoot = ModContent.ProjectileType<GunPellet>();
                                int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition, shootVel, projectileToShoot, newPunchDamage / 3, 1f, Projectile.owner);
                                Main.projectile[projIndex].netUpdate = true;
                                Projectile.netUpdate = true;
                                if (JoJoFanStands.SoundsLoaded)
                                    SoundEngine.PlaySound(VirtualInsanityStandFinal.ShootSound, Projectile.Center);
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
                Main.EntitySpriteDraw(VirtualInsanityStandFinal.AttackStyleTextures[attackType], drawPosition, null, bodyColor * Math.Clamp(attackChangeEffectTimer / 60f, 0f, 1f), 0f, new Vector2(43), 1f, SpriteEffects.None);
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

        public override void PostDrawExtras()
        {
            Color bodyColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
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
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            if (currentAnimationState == AnimationState.Special || currentAnimationState == AnimationState.Pose)
                standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/" + animationName).Value;
            else
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
                AnimateStand(animationName, 17, 4, false);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 300, false);
        }
    }
}