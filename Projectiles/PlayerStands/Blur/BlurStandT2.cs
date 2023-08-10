using JoJoFanStands.Buffs;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles.PlayerStands;
using JoJoStands.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.Blur
{
    public class BlurStandT2 : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 28;
        public override int AltDamage => 45;
        public override int PunchTime => 7;
        public override int TierNumber => 2;
        public override float MaxDistance => 37.5f * 16f;
        public override Vector2 StandOffset => new Vector2(-2 * 2, 0f);
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        private int punchAnimationTimer = 0;
        private int afterImageTimer = 0;
        private int extraAfterImagePlayTime = 0;
        private int blurBarIncreaseTimer = 0;
        private bool dashChargeHitFrame = false;
        private int dashChargeTimer = 0;
        private int averageAmountOfDashAfterImages;
        private Vector2 dashStartPosition;
        private Vector2 dashTargetPosition;
        private List<PunchFrame> backPunchFrames = new List<PunchFrame>();
        private List<PunchFrame> frontPunchFrames = new List<PunchFrame>();
        private List<AfterimageData> afterImages = new List<AfterimageData>();
        private readonly int BlurPercentageChargeTime = (2 * 60) - 15;
        private readonly float MaxPercentageGain = 0.25f;

        public struct PunchFrame
        {
            public Vector2 offset;
            public Vector2 targetOffset;
            public int punchAnimationTimeStart;
            public int punchLifeTime;
            public bool flipped;
            public int textureType;
        }

        public struct AfterimageData
        {
            public Vector2 position;
            public Rectangle animRect;
            public int lifeTime;
            public int afterImageTimeStart;
            public int direction;
            public float rotation;
        }

        public new enum AnimationState
        {
            Idle,
            Attack,
            Secondary,
            Stab,
            Pose
        }

        public override void ExtraSpawnEffects()
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            if (Main.player[Projectile.owner].GetModPlayer<FanPlayer>().amountOfBlurEnergy > 20)
                Main.player[Projectile.owner].GetModPlayer<FanPlayer>().amountOfBlurEnergy -= 20;
            else
                Main.player[Projectile.owner].GetModPlayer<FanPlayer>().amountOfBlurEnergy = 0;
            BlurBar.ShowBlurBar();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            fPlayer.blurStage = fPlayer.amountOfBlurEnergy / 20;
            float gainPercentage = 1f + (MaxPercentageGain * (fPlayer.blurStage / 5f));
            mPlayer.standDamageBoosts *= gainPercentage;
            mPlayer.standSpeedBoosts = (int)(mPlayer.standSpeedBoosts * (1f / gainPercentage));

            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft)
                    {
                        if (fPlayer.amountOfBlurEnergy < 100)
                        {
                            blurBarIncreaseTimer++;
                            if (blurBarIncreaseTimer >= BlurPercentageChargeTime)
                            {
                                fPlayer.amountOfBlurEnergy += 1;
                                blurBarIncreaseTimer = 0;
                            }
                        }

                        Punch(6f * gainPercentage, afterImages: false);
                        currentAnimationState = AnimationState.Attack;
                        int amountOfPunches = Main.rand.Next(3, 5);
                        for (int i = 0; i < amountOfPunches; i++)
                        {
                            bool behind = Main.rand.Next(0, 1 + 1) == 0;
                            Vector2 punchOffset = new Vector2(Main.rand.Next(-8, 4 + 1) * Projectile.spriteDirection, Main.rand.Next(-HalfStandHeight + 6, HalfStandHeight - 6 + 1));
                            PunchFrame punchFrame = new PunchFrame()
                            {
                                offset = punchOffset,
                                targetOffset = punchOffset + new Vector2(Main.rand.Next(16, 24 + 1) * Projectile.spriteDirection, 0f),
                                punchAnimationTimeStart = punchAnimationTimer,
                                punchLifeTime = Main.rand.Next(5, 12 + 1),
                                flipped = Main.rand.Next(0, 1 + 1) == 0,
                                textureType = Main.rand.Next(0, 1 + 1)
                            };
                            if (behind)
                                backPunchFrames.Add(punchFrame);
                            else
                                frontPunchFrames.Add(punchFrame);
                        }
                        punchAnimationTimer++;
                    }
                    else if (Main.mouseRight && !playerHasAbilityCooldown && dashChargeTimer <= 0)
                    {
                        dashChargeTimer = 15;
                        dashStartPosition = Projectile.Center;
                        dashTargetPosition = Main.MouseWorld;
                        averageAmountOfDashAfterImages = (int)((Vector2.Distance(dashStartPosition, dashTargetPosition) / 16f) / 15f);
                        secondaryAbility = true;
                        dashChargeHitFrame = false;
                        fPlayer.amountOfBlurEnergy += 5;
                    }
                    else
                    {
                        if (dashChargeTimer == 0)
                        {
                            StayBehind();
                            currentAnimationState = AnimationState.Idle;
                            punchAnimationTimer = 0;
                            backPunchFrames.Clear();
                            frontPunchFrames.Clear();
                        }
                    }
                }
                if (dashChargeTimer == 0 && !attacking)
                {
                    StayBehind();
                    currentAnimationState = AnimationState.Idle;
                    punchAnimationTimer = 0;
                }

                bool usingAfterImages = false;
                if (dashChargeTimer > 0)
                {
                    dashChargeTimer--;
                    usingAfterImages = true;
                    currentAnimationState = AnimationState.Secondary;
                    if (dashChargeTimer > 0 && !dashChargeHitFrame)
                    {
                        Vector2 previousPosition = Projectile.Center;
                        Projectile.Center = Vector2.Lerp(dashStartPosition, dashTargetPosition, (15 - dashChargeTimer) / 15f);
                        Projectile.spriteDirection = previousPosition.X < Projectile.Center.X ? 1 : -1;
                        for (int i = 0; i < averageAmountOfDashAfterImages; i++)
                        {
                            AfterimageData afterimageData = new AfterimageData()
                            {
                                position = Vector2.Lerp(previousPosition, Projectile.Center, i / (float)averageAmountOfDashAfterImages),
                                animRect = new Rectangle(0, Projectile.frame * (HalfStandHeight * 2), standTexture.Width / 2, HalfStandHeight * 2),
                                lifeTime = 8,
                                afterImageTimeStart = afterImageTimer,
                                direction = Projectile.spriteDirection,
                                rotation = Projectile.rotation
                            };
                            afterImages.Add(afterimageData);
                        }
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.active && npc.lifeMax > 5 && npc.CanBeChasedBy(this))
                            {
                                if (Collision.CheckAABBvLineCollision(npc.position, npc.Size, previousPosition, Projectile.Center))
                                {
                                    int difference = (int)Projectile.Center.X - (int)npc.Center.X;
                                    NPC.HitInfo hitInfo = new NPC.HitInfo()
                                    {
                                        Damage = (int)(AltDamage * mPlayer.standDamageBoosts),
                                        Knockback = 6f,
                                        HitDirection = Math.Abs(difference) / difference,
                                        Crit = Main.rand.Next(1, 100 + 1) <= mPlayer.standCritChangeBoosts
                                    };
                                    npc.StrikeNPC(hitInfo);
                                }
                            }
                        }
                    }
                    if (dashChargeTimer <= 0)
                    {
                        if (!dashChargeHitFrame)
                        {
                            dashChargeHitFrame = true;
                            dashChargeTimer = 10;
                        }
                        else
                        {
                            dashChargeTimer = 0;
                            secondaryAbility = false;
                            dashChargeHitFrame = false;
                            extraAfterImagePlayTime = 15;
                            player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(10));
                        }
                    }
                }
                if (dashChargeHitFrame)
                    currentAnimationState = AnimationState.Stab;
                if (SpecialKeyPressed())
                    player.AddBuff(ModContent.BuffType<LightningFastReflex>(), 8 * 60);
                if (player.HasBuff<LightningFastReflex>())
                {
                    usingAfterImages = true;
                    player.armorEffectDrawShadow = true;
                    if (afterImageTimer % 2 == 0)
                    {
                        AfterimageData afterimageData = new AfterimageData()
                        {
                            position = Projectile.Center,
                            animRect = new Rectangle(0, Projectile.frame * (HalfStandHeight * 2), standTexture.Width / 2, HalfStandHeight * 2),
                            lifeTime = 60,
                            afterImageTimeStart = afterImageTimer,
                            direction = Projectile.spriteDirection,
                            rotation = Projectile.rotation
                        };
                        afterImages.Add(afterimageData);
                    }
                    if (player.buffTime[player.FindBuffIndex(ModContent.BuffType<LightningFastReflex>())] == 1)
                        extraAfterImagePlayTime = 60;
                }

                if (fPlayer.blurStage == 4)
                {
                    usingAfterImages = true;
                    player.armorEffectDrawShadow = true;
                    if (afterImageTimer % 30 == 0)
                    {
                        AfterimageData afterimageData = new AfterimageData()
                        {
                            position = Projectile.Center,
                            animRect = new Rectangle(0, Projectile.frame * (HalfStandHeight * 2), standTexture.Width / 2, HalfStandHeight * 2),
                            lifeTime = 8,
                            afterImageTimeStart = afterImageTimer,
                            direction = Projectile.spriteDirection,
                            rotation = Projectile.rotation
                        };
                        afterImages.Add(afterimageData);
                    }
                }
                else if (fPlayer.blurStage == 5)
                {
                    usingAfterImages = true;
                    player.armorEffectDrawShadow = true;
                    if (afterImageTimer % 5 == 0)
                    {
                        AfterimageData afterimageData = new AfterimageData()
                        {
                            position = Projectile.Center,
                            animRect = new Rectangle(0, Projectile.frame * (HalfStandHeight * 2), standTexture.Width / 2, HalfStandHeight * 2),
                            lifeTime = 8,
                            afterImageTimeStart = afterImageTimer,
                            direction = Projectile.spriteDirection,
                            rotation = Projectile.rotation
                        };
                        afterImages.Add(afterimageData);
                    }
                }
                if (extraAfterImagePlayTime > 0)
                {
                    extraAfterImagePlayTime--;
                    usingAfterImages = true;
                }

                if (!usingAfterImages)
                {
                    afterImageTimer = 0;
                    if (afterImages.Count > 0)
                        afterImages.Clear();
                }
                else
                    afterImageTimer++;
            }
            else
            {
                BasicPunchAI();
            }
            if (dashChargeTimer < 0)
                LimitDistance();
            if (mPlayer.posing)
                currentAnimationState = AnimationState.Pose;
        }

        public override byte SendAnimationState() => (byte)currentAnimationState;
        public override void ReceiveAnimationState(byte state) => currentAnimationState = (AnimationState)state;

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
            else if (currentAnimationState == AnimationState.Secondary)
                PlayAnimation("Dash");
            else if (currentAnimationState == AnimationState.Stab)
                PlayAnimation("Stab");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void StandKillEffects()
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            BlurBar.HideBlurBar();
        }


        public override bool PreDrawExtras()
        {
            if (attacking)
            {
                for (int i = 0; i < backPunchFrames.Count; i++)
                {
                    float percentageLife = (punchAnimationTimer - backPunchFrames[i].punchAnimationTimeStart) / (float)backPunchFrames[i].punchLifeTime;
                    Vector2 drawPosition = Projectile.Center + Vector2.Lerp(backPunchFrames[i].offset, backPunchFrames[i].targetOffset, percentageLife);
                    SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Main.EntitySpriteDraw(BlurStandT1.punchTextures[backPunchFrames[i].textureType], drawPosition - Main.screenPosition, null, Color.Gray * (1f - percentageLife), Projectile.rotation, BlurStandT1.PunchOrigin, Projectile.scale, spriteEffects, 0f);
                    if (percentageLife == 1f)
                    {
                        backPunchFrames.RemoveAt(i);
                        i--;
                    }
                }
            }
            if (afterImages.Count != 0)
            {
                for (int i = 0; i < afterImages.Count; i++)
                {
                    float percentageLife = (afterImageTimer - afterImages[i].afterImageTimeStart) / (float)afterImages[i].lifeTime;
                    int frameHeight = standTexture.Height / amountOfFrames;

                    Vector2 drawOffset = StandOffset;
                    drawOffset.X *= afterImages[i].direction;
                    Vector2 drawPosition = afterImages[i].position - Main.screenPosition + drawOffset;
                    Vector2 standOrigin = new Vector2(standTexture.Width / 2f, frameHeight / 2f);
                    SpriteEffects effect = SpriteEffects.None;
                    if (afterImages[i].direction == -1)
                    {
                        drawPosition.X += 32f;
                        effect = SpriteEffects.FlipHorizontally;
                    }
                    Main.EntitySpriteDraw(standTexture, drawPosition, afterImages[i].animRect, Color.White * (1f - percentageLife) * 0.4f, afterImages[i].rotation, standOrigin, Projectile.scale, effect, 0f);
                    if (percentageLife == 1f)
                    {
                        afterImages.RemoveAt(i);
                        i--;
                    }
                }
            }
            return false;
        }

        public override void PostDrawExtras()
        {
            if (attacking)
            {
                for (int i = 0; i < frontPunchFrames.Count; i++)
                {
                    float percentageLife = (punchAnimationTimer - frontPunchFrames[i].punchAnimationTimeStart) / (float)frontPunchFrames[i].punchLifeTime;
                    Vector2 drawPosition = Projectile.Center + Vector2.Lerp(frontPunchFrames[i].offset, frontPunchFrames[i].targetOffset, percentageLife);
                    SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Main.EntitySpriteDraw(BlurStandT1.punchTextures[frontPunchFrames[i].textureType], drawPosition - Main.screenPosition, null, Color.White * (1f - percentageLife), Projectile.rotation, BlurStandT1.PunchOrigin, Projectile.scale, spriteEffects, 0f);
                    if (percentageLife == 1f)
                    {
                        frontPunchFrames.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            Player player = Main.player[Projectile.owner];
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            writer.Write(secondaryAbility);
            writer.Write(dashChargeHitFrame);
            writer.Write((byte)fPlayer.amountOfBlurEnergy);
            writer.Write(dashStartPosition.X);
            writer.Write(dashStartPosition.Y);
            writer.Write(dashTargetPosition.X);
            writer.Write(dashTargetPosition.Y);
            writer.Write((byte)averageAmountOfDashAfterImages);

        }
        public override void ReceiveExtraStates(BinaryReader reader)
        {
            Player player = Main.player[Projectile.owner];
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            secondaryAbility = reader.ReadBoolean();
            dashChargeHitFrame = reader.ReadBoolean();
            fPlayer.amountOfBlurEnergy = reader.ReadByte();
            dashStartPosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            dashTargetPosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            averageAmountOfDashAfterImages = reader.ReadByte();
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Blur/Blur_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 8, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime / 2, true);
            else if (animationName == "Dash")
                AnimateStand(animationName, 1, 600, true);
            else if (animationName == "Stab")
                AnimateStand(animationName, 1, 600, true);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 600, true);
        }
    }
}