using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;

namespace JoJoFanStands.Projectiles.PlayerStands.SlavesOfFear
{
    public class SlavesOfFearStandT1 : StandClass
    {
        public override int punchDamage => 23;
        public override int punchTime => 13;
        public override int altDamage => 96;
        public override int halfStandHeight => 37;
        public override int standType => 1;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
            {
                shootCount--;
            }
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }

            if (Main.mouseLeft)
            {
                Punch();
            }
            else
            {
                StayBehind();
            }
            LimitDistance();
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                normalFrames = false;
                PlayAnimation("Attack");
            }
            if (normalFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = mod.GetTexture("Projectiles/PlayerStands/SlavesOfFear/SlavesOfFear_" + animationName);
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 2, 14, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 4, newPunchTime, true);
            }
        }
    }
}