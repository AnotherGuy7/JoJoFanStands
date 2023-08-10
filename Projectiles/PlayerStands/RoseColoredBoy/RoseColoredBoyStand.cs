using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Projectiles.PlayerStands.RoseColoredBoy
{
    public class RoseColoredBoyStand : StandClass
    {
        public override int PunchDamage => 62;
        public override int PunchTime => 10;
        public override int AltDamage => 74;
        public override int HalfStandHeight => 32;
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;

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
                        Punch(afterImages: false);
                    else if (Main.mouseRight)
                    {
                        Projectile.direction = player.direction;
                        if (shootCount <= 0)
                        {
                            shootCount = 70;
                            Vector2 shootVel = Main.MouseWorld - Projectile.position;
                            shootVel.Normalize();
                            shootVel *= 12f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(28f * Projectile.direction, -8f), shootVel, ProjectileType<RosePetal>(), AltDamage, 6f, player.whoAmI);
                        }
                        Dust.NewDust(Projectile.Center + new Vector2(28f * Projectile.direction, -8f), 2, 2, DustID.Torch);
                        secondaryAbility = true;
                    }
                    else
                        secondaryAbility = false;
                }
                
                if (!attacking)
                {
                    if (!secondaryAbility)
                        StayBehind();
                    else
                        GoInFront();
                }
                if (SpecialKeyPressed() && !player.HasBuff(BuffType<JoJoStands.Buffs.Debuffs.AbilityCooldown>()))
                {
                    player.AddBuff(BuffType<Buffs.RoseColoredSunBuff>(), 120 * 60);
                    player.AddBuff(BuffType<JoJoStands.Buffs.Debuffs.AbilityCooldown>(), 240 * 60);
                }
            }
            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
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
            else if (currentAnimationState == AnimationState.SecondaryAbility)
                PlayAnimation("Secondary");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/RoseColoredBoy/RoseColoredBoy_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 12, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 7, newPunchTime, true);
            else if (animationName == "Secondary")
                AnimateStand(animationName, 1, 180, true);
            else if (animationName == "Pose")
                AnimateStand(animationName, 15, 20, true);
        }
    }
}