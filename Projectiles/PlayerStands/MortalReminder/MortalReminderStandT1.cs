using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace JoJoFanStands.Projectiles.PlayerStands.MortalReminder
{
    public class MortalReminderStandT1 : StandClass
    {
        public override int PunchDamage => 18;
        public override int AltDamage => 96;
        public override int HalfStandHeight => 28;
        public override int PunchTime => 17;
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;
        public override Vector2 StandOffset => new Vector2(-25, 0);

        private int afterImageTimer = 60;
        private List<AfterimageData> afterImages = new List<AfterimageData>();

        public struct AfterimageData
        {
            public Vector2 position;
            public Rectangle animRect;
            public int lifeTime;
            public int afterImageTimeStart;
            public int direction;
            public float rotation;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            SelectAnimation();
            UpdateStandInfo();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (shootCount > 0)
                shootCount--;

            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.mouseLeft)
                {
                    Punch();
                    Projectile.ai[1] = 3f;
                }
                else
                {
                    StayBehind();
                    Projectile.ai[1] = 1f;
                }
            }

            afterImageTimer++;
            if (afterImageTimer % 4 == 0)
            {
                AfterimageData afterimageData = new AfterimageData()
                {
                    position = Projectile.Center,
                    animRect = new Rectangle(0, Projectile.frame * (HalfStandHeight * 2), standTexture.Width, HalfStandHeight * 2),
                    lifeTime = 12,
                    afterImageTimeStart = afterImageTimer,
                    direction = Projectile.spriteDirection,
                    rotation = Projectile.rotation
                };
                afterImages.Add(afterimageData);
            }
            LimitDistance();
        }

        public override bool PreDrawExtras()
        {
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
                        effect = SpriteEffects.FlipHorizontally;

                    Main.EntitySpriteDraw(standTexture, drawPosition, afterImages[i].animRect, Color.Green * (1f - percentageLife) * 0.8f, afterImages[i].rotation, standOrigin, Projectile.scale, effect, 0f);
                    if (percentageLife >= 1f)
                    {
                        afterImages.RemoveAt(i);
                        i--;
                    }
                }
            }
            return true;
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
                PlayAnimation("Dash");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/MortalReminder/MortalReminder_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 12, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 12, newPunchTime / 3, true);
            else if (animationName == "Dash")
                AnimateStand(animationName, 4, 10, true);
        }
    }
}