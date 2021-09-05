using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Projectiles.PlayerStands.RoseColoredBoy
{
    public class RoseColoredBoyStand : StandClass
    {
        public override int punchDamage => 62;
        public override int punchTime => 10;
        public override int altDamage => 74;
        public override int halfStandHeight => 32;
        public override int standType => 1;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                projectile.timeLeft = 2;

            if (!mPlayer.standAutoMode)
            {
                if (Main.mouseLeft && player.whoAmI == projectile.owner)
                {
                    Punch();
                }
                else
                {
                    if (player.whoAmI == Main.myPlayer)
                        attackFrames = false;
                }
                if (Main.mouseRight && player.whoAmI == projectile.owner)
                {
                    projectile.direction = player.direction;
                    if (shootCount <= 0)
                    {
                        shootCount = 70;
                        Vector2 shootVel = Main.MouseWorld - projectile.position;
                        shootVel.Normalize();
                        shootVel *= 12f;
                        Projectile.NewProjectile(projectile.Center + new Vector2(28f * projectile.direction, -8f), shootVel, ProjectileType<RosePetal>(), altDamage, 6f, player.whoAmI);
                    }
                    Dust.NewDust(projectile.Center + new Vector2(28f * projectile.direction, -8f), 2, 2, DustID.Fire);
                    secondaryAbilityFrames = true;
                }
                else
                {
                    secondaryAbilityFrames = false;
                }
                if (!attackFrames)
                {
                    if (!secondaryAbilityFrames)
                    {
                        StayBehind();
                        normalFrames = true;
                    }
                    else
                    {
                        GoInFront();
                    }
                }
                if (SpecialKeyPressed() && !player.HasBuff(BuffType<JoJoStands.Buffs.Debuffs.AbilityCooldown>()))
                {
                    player.AddBuff(BuffType<Buffs.RoseColoredSunBuff>(), 120 * 60);
                    player.AddBuff(BuffType<JoJoStands.Buffs.Debuffs.AbilityCooldown>(), 240 * 60);
                }
            }
            if (mPlayer.standAutoMode)
            {
                BasicPunchAI();
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
            if (secondaryAbilityFrames)
            {
                attackFrames = false;
                normalFrames = false;
                PlayAnimation("Secondary");
            }
            if (Main.player[projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                attackFrames = false;
                normalFrames = false;
                secondaryAbilityFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = mod.GetTexture("Projectiles/PlayerStands/RoseColoredBoy/RoseColoredBoy_" + animationName);
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 12, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 7, newPunchTime, true);
            }
            if (animationName == "Secondary")
            {
                AnimateStand(animationName, 1, 180, true);
            }
            if (animationName == "Pose")
            {
                AnimateStand(animationName, 15, 20, true);
            }
        }
    }
}