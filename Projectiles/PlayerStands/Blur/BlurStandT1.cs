using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using JoJoStands.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.Blur
{
    public class BlurStandT1 : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 98;
        public override int AltDamage => 112;
        public override int PunchTime => 8;
        public override int TierNumber => 1;
        public override float MaxDistance => 25 * 16;
        public override Vector2 StandOffset => new Vector2(-2 * 2, 0f);
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;
        public static Texture2D[] punchTextures;

        private int punchAnimationTimer = 0;
        private int afterImageTimer = 0;
        private int blurBarIncreaseTimer = 0;
        private List<PunchFrame> backPunchFrames = new List<PunchFrame>();
        private List<PunchFrame> frontPunchFrames = new List<PunchFrame>();
        private List<AfterimageData> afterImages = new List<AfterimageData>();
        public static readonly Vector2 PunchOrigin = new Vector2(12, 6);
        private readonly int BlurPercentageChargeTime = 2 * 60;
        private readonly float MaxPercentageGain = 0.2f;

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
                if (Projectile.whoAmI == Main.myPlayer)
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

                        Punch(5f * gainPercentage, afterImages: false);
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
                    else
                    {
                        StayBehind();
                        punchAnimationTimer = 0;
                        backPunchFrames.Clear();
                        frontPunchFrames.Clear();
                    }
                }

                if (fPlayer.blurStage == 4)
                {
                    afterImageTimer++;
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
                    afterImageTimer++;
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
                else
                    afterImageTimer = 0;
            }
            else
                BasicPunchAI();
            LimitDistance();
            if (mPlayer.posing)
                currentAnimationState = AnimationState.Pose;
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
                    Main.EntitySpriteDraw(punchTextures[backPunchFrames[i].textureType], drawPosition - Main.screenPosition, null, Color.Gray * (1f - percentageLife), Projectile.rotation, PunchOrigin, Projectile.scale, spriteEffects, 0f);
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
                    drawOffset.X *= Projectile.spriteDirection;
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
                    Main.EntitySpriteDraw(punchTextures[frontPunchFrames[i].textureType], drawPosition - Main.screenPosition, null, Color.White * (1f - percentageLife), Projectile.rotation, PunchOrigin, Projectile.scale, spriteEffects, 0f);
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

            writer.Write((byte)fPlayer.amountOfBlurEnergy);

        }
        public override void ReceiveExtraStates(BinaryReader reader)
        {
            Player player = Main.player[Projectile.owner];
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            fPlayer.amountOfBlurEnergy = reader.ReadByte();
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Blur/Blur_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 8, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime / 2, true);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 600, true);
        }
    }
}