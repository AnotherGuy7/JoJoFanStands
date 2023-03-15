using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.MortalReminder
{
    public class MortalReminderStandT1 : StandClass
    {
        public override int PunchDamage => 18;
        public override int AltDamage => 96;
        public override int HalfStandHeight => 28;
        public override int PunchTime => 17;
        public override StandAttackType StandType => StandAttackType.Melee;
        public override Vector2 StandOffset => new Vector2(-25, 0);

        private int afterImageTimer = 60;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            SelectAnimation();
            UpdateStandInfo();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (Projectile.ai[0] == 0f)
            {
                if (shootCount > 0)
                    shootCount--;

                if (Main.mouseLeft)
                {
                    attackFrames = true;
                    Punch();
                    Projectile.ai[1] = 3f;
                }
                else
                {
                    idleFrames = true;
                    StayBehind();
                    Projectile.ai[1] = 1f;
                }

                afterImageTimer--;
                if (afterImageTimer <= 0)
                {
                    int afterImage = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<MortalReminderStandT1>(), 0, 0f, Main.myPlayer, 1f, Projectile.ai[1]);
                    Main.projectile[afterImage].timeLeft = 300;
                    Main.projectile[afterImage].frame = Projectile.frame;
                    Main.projectile[afterImage].spriteDirection = Projectile.spriteDirection;
                    Main.projFrames[afterImage] = Main.projFrames[Projectile.whoAmI];
                    afterImageTimer = 60;
                }
                LimitDistance();
            }
            else
            {
                Projectile.alpha = 125 + (int)((float)Projectile.timeLeft / 2.4f);
                if (Projectile.ai[1] == 1f)
                {
                    idleFrames = true;
                }
                if (Projectile.ai[1] == 2f)
                {
                    secondaryAbilityFrames = true;
                }
                if (Projectile.ai[1] == 3f)
                {
                    attackFrames = true;
                }
            }
        }
        public override bool PreDrawExtras()
        {
            if (Projectile.ai[0] == 1f)
            {
                int frameHeight = standTexture.Height / Main.projFrames[Projectile.whoAmI];
                Rectangle sourceRect = new Rectangle(0, frameHeight * Projectile.frame, standTexture.Width, frameHeight);
                Main.EntitySpriteDraw(standTexture, Projectile.Center - Main.screenPosition + new Vector2(19f, 1f), sourceRect, Color.White * Projectile.alpha, 0f, new Vector2(standTexture.Width / 2f, frameHeight / 2f), 1f, effects, 0);
            }
            return true;
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                idleFrames = false;
                PlayAnimation("Attack");
                Projectile.ai[1] = 3f;
            }
            if (idleFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
                Projectile.ai[1] = 1f;
            }
            if (secondaryAbilityFrames)     //Dash
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Dash");
                Projectile.ai[1] = 2f;
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/MortalReminder/MortalReminder_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 12, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 12, newPunchTime / 2, true);
            }
            if (animationName == "Dash")
            {
                AnimateStand(animationName, 4, 10, true);
            }
        }
    }
}