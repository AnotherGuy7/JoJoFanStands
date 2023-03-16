using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.SlavesOfFear
{
    public class SlavesOfFearStandT2 : StandClass
    {
        public override float MaxDistance
        {
            get { return 98f; }
        }

        public override int HalfStandHeight => 37;          //a simpler version of doing the above

        public override int PunchDamage => 47;
        public override int AltDamage => 54;
        public override int PunchTime => 12;
        public override int TierNumber => 2;
        public override StandAttackType StandType => StandAttackType.Melee;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            shootCount--;
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (Main.mouseLeft)
            {
                Punch();
            }
            else
            {
                if (!secondaryAbilityFrames)
                {
                    idleFrames = true;
                    StayBehind();
                }
            }
            if (Main.mouseRight)
            {
                secondaryAbilityFrames = true;
            }
            Vector2 direction = player.Center - Projectile.Center;
            float distanceTo = direction.Length();
            if (secondaryAbilityFrames)
            {
                idleFrames = false;
                attackFrames = false;
                Projectile.velocity.X = 10f * Projectile.direction;
                Projectile.position.Y = player.position.Y;
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && Projectile.Distance(npc.Center) <= 15f)
                        npc.StrikeNPC(AltDamage, 8f, Projectile.direction);
                }
                if (distanceTo > newMaxDistance * 2)
                    secondaryAbilityFrames = false;
            }
            else
            {
                LimitDistance();
            }
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                idleFrames = false;
                PlayAnimation("Attack");
            }
            if (idleFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
            if (secondaryAbilityFrames)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Secondary");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/SlavesOfFear/SlavesOfFear_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 2, 14, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 4, newPunchTime, true);
            }
            if (animationName == "Secondary")
            {
                AnimateStand(animationName, 1, 15, true);
            }
        }
    }
}