using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.WaywardSon
{
    public class WaywardSonStandT1 : StandClass
    {
        public override int PunchDamage => 23;
        public override int PunchTime => 13;
        public override int AltDamage => 96;
        public override int HalfStandHeight => 37;
        public override int TierNumber => 1;
        public override int FistID => 1;
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;

        private const float AttackVacuumRange = 8f * 16f;
        private const float AttackVacuumForce = 0.15f;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft)
                    {
                        Punch(afterImages: false);
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            if (Main.npc[n].active)
                            {
                                NPC npc = Main.npc[n];
                                bool directionCheck = Projectile.direction == 1 ? npc.Center.X > Projectile.Center.X : npc.Center.X < Projectile.Center.X;
                                float npcDistance = Vector2.Distance(npc.Center, Projectile.Center);
                                if (directionCheck && npcDistance < AttackVacuumRange)
                                {
                                    Vector2 direction = Projectile.Center - npc.Center;
                                    direction.Normalize();
                                    direction *= AttackVacuumForce;
                                    npc.velocity += direction;
                                }
                            }
                        }
                    }
                    else
                        StayBehind();
                }
                LimitDistance();
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
                BasicPunchAI();
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
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/WaywardSon/WaywardSon_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
        }
    }
}