using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.WaywardSon
{
    public class WaywardSonAfterImage : StandClass
    {
        public override void SetStaticDefaults()
        { }

        public override void SetDefaults()
        {
            Projectile.alpha = 186;
            Projectile.width = 38;
            Projectile.height = 1;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.shouldFallThrough = true;
        }

        public override float MaxDistance => 98f;
        public override int PunchDamage => 26;
        public override int PunchTime => 9;
        public override int HalfStandHeight => 37;
        public override int FistID => 10;
        public override bool UseProjectileAlpha => true;
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        public new enum AnimationState
        {
            Idle,
            Attack,
            Secondary,
            Parry,
            Pose
        }

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            UpdateStandSync();
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.silverChariotShirtless)
                Projectile.timeLeft = 2;
            if (shootCount > 0)
                shootCount--;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft)
                    {
                        currentAnimationState = AnimationState.Attack;
                        Punch(7.5f);
                    }
                    else
                    {
                        attacking = false;
                        currentAnimationState = AnimationState.Idle;
                    }

                }

                if (!attacking)
                {
                    float angle = 360f / Projectile.ai[1];
                    angle *= Projectile.ai[0];

                    Projectile.position = player.Center + (MathHelper.ToRadians(angle).ToRotationVector2() * (4f * 16f));
                    currentAnimationState = AnimationState.Idle;
                    Projectile.spriteDirection = Projectile.direction = player.direction;
                }
            }
            else
                BasicPunchAI();

            if (mPlayer.posing)
                currentAnimationState = AnimationState.Pose;
        }

        public override bool PreDraw(ref Color drawColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);        //starting a draw with dyes that work

            SyncAndApplyDyeSlot();
            DrawStand(drawColor);

            return true;
        }

        private void DrawStand(Color drawColor)
        {
            effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;

            drawColor *= Projectile.alpha / 255f;
            if (standTexture != null && Main.netMode != NetmodeID.Server)
            {
                int frameHeight = standTexture.Height / amountOfFrames;
                Main.EntitySpriteDraw(standTexture, Projectile.Center - Main.screenPosition + new Vector2(DrawOffsetX / 2f, 0f), new Rectangle(0, frameHeight * Projectile.frame, standTexture.Width, frameHeight), drawColor, 0f, new Vector2(standTexture.Width / 2f, frameHeight / 2f), 1f, effects, 0);
            }
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
                PlayAnimation("Secondary");
            else if (currentAnimationState == AnimationState.Parry)
                PlayAnimation("Parry");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            if (Main.netMode != NetmodeID.Server)
                standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/WaywardSon/WaywardSon_" + animationName).Value;

            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
            else if (animationName == "Secondary")
                AnimateStand(animationName, 1, 15, true);
            else if (animationName == "Weld")
                AnimateStand(animationName, 1, 15, true);
        }
    }
}
