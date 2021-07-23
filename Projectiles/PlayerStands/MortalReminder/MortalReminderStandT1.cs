using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;

namespace JoJoFanStands.Projectiles.PlayerStands.MortalReminder
{
    public class MortalReminderStandT1 : StandClass
    {
        public override int punchDamage => 18;
        public override int altDamage => 96;
        public override int halfStandHeight => 28;
        public override int punchTime => 17;
        public override int standType => 1;
        public override int standOffset => -25;

        private int afterImageTimer = 60;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            SelectAnimation();
            UpdateStandInfo();
            if (mPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }
            if (projectile.ai[0] == 0f)
            {
                if (shootCount > 0)
                {
                    shootCount--;
                }

                if (Main.mouseLeft)
                {
                    attackFrames = true;
                    Punch();
                    projectile.ai[1] = 3f;
                }
                else
                {
                    normalFrames = true;
                    StayBehind();
                    projectile.ai[1] = 1f;
                }

                afterImageTimer--;
                if (afterImageTimer <= 0)
                {
                    int afterImage = Projectile.NewProjectile(projectile.position, Vector2.Zero, mod.ProjectileType(Name), 0, 0f, Main.myPlayer, 1f, projectile.ai[1]);
                    Main.projectile[afterImage].timeLeft = 300;
                    Main.projectile[afterImage].frame = projectile.frame;
                    Main.projectile[afterImage].spriteDirection = projectile.spriteDirection;
                    Main.projFrames[afterImage] = Main.projFrames[projectile.whoAmI];
                    afterImageTimer = 60;
                }
                LimitDistance();
            }
            else
            {
                projectile.alpha = 125 + (int)((float)projectile.timeLeft / 2.4f);
                if (projectile.ai[1] == 1f)
                {
                    normalFrames = true;
                }
                if (projectile.ai[1] == 2f)
                {
                    secondaryAbilityFrames = true;
                }
                if (projectile.ai[1] == 3f)
                {
                    attackFrames = true;
                }
            }
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            if (projectile.ai[0] == 1f)
            {
                int frameHeight = standTexture.Height / Main.projFrames[projectile.whoAmI];
                spriteBatch.Draw(standTexture, projectile.Center - Main.screenPosition + new Vector2(19f, 1f), new Rectangle(0, frameHeight * projectile.frame, standTexture.Width, frameHeight), Color.White * projectile.alpha, 0f, new Vector2(standTexture.Width / 2f, frameHeight / 2f), 1f, effects, 0);
            }
            return true;
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                normalFrames = false;
                PlayAnimation("Attack");
                projectile.ai[1] = 3f;
            }
            if (normalFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
                projectile.ai[1] = 1f;
            }
            if (secondaryAbilityFrames)     //Dash
            {
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Dash");
                projectile.ai[1] = 2f;
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = mod.GetTexture("Projectiles/PlayerStands/MortalReminder/MortalReminder_" + animationName);
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