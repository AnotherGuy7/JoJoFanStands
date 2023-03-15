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
                if (Main.mouseLeft && player.whoAmI == Projectile.owner)
                {
                    Punch();
                }
                else
                {
                    if (player.whoAmI == Main.myPlayer)
                        attackFrames = false;
                }
                if (Main.mouseRight && player.whoAmI == Projectile.owner)
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
                        idleFrames = true;
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
            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                BasicPunchAI();
            }
            LimitDistance();
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
                attackFrames = false;
                idleFrames = false;
                PlayAnimation("Secondary");
            }
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().posing)
            {
                attackFrames = false;
                idleFrames = false;
                secondaryAbilityFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/RoseColoredBoy/RoseColoredBoy_" + animationName).Value;
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