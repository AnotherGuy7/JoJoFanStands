using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.Banks
{
    public class BanksStandT1 : StandClass
    {
        public override float ProjectileSpeed => 16f;
        public override int ShootTime => 12;
        public override int ProjectileDamage => 5;
        public override int TierNumber => 1;
        public override StandAttackType StandType => StandAttackType.Ranged;
        public override int HalfStandHeight => 32;
        public override Vector2 StandOffset => Vector2.Zero;


        private const float TargetDetectionRange = 32f * 16f;
        private NPC target = null;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            //Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), 0.6f, 0.9f, 0.3f);
            //Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 35, Projectile.velocity.X * -0.5f, Projectile.velocity.Y * -0.5f);

            if (shootCount > 0)
                shootCount--;
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (Main.mouseLeft && player.whoAmI == Projectile.owner)
            {
                if (target == null)
                {
                    target = FindNearestTarget(TargetDetectionRange);
                }
                else
                {
                    attackFrames = true;
                    Projectile.position = target.Center + new Vector2(((target.width / 2f) + Projectile.width) * -target.direction, 0f);
                    Projectile.direction = target.direction;
                    if (Projectile.Distance(target.Center) >= TargetDetectionRange)
                    {
                        target = null;
                    }

                    if (shootCount <= 0 && Projectile.frame == 2)
                    {
                        shootCount += ShootTime;
                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                        {
                            Damage = newProjectileDamage,
                            Knockback = 0.2f,
                            HitDirection = Projectile.direction
                        };
                        target.StrikeNPC(hitInfo);
                        SoundStyle item41 = SoundID.Item41;
                        item41.Pitch = 3f;
                        SoundEngine.PlaySound(item41, Projectile.Center);
                    }
                }
            }
            else
            {
                attackFrames = false;
                idleFrames = true;
                target = null;
            }

            if (!attackFrames)
            {
                StayBehind();
            }
            Projectile.spriteDirection = Projectile.direction;
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
                PlayAnimation("Pose");
            }
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().posing)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Banks/BanksStand_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 15, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 5, ShootTime / 5, true);
            }
            if (animationName == "Pose")
            {
                AnimateStand(animationName, 1, 300, true);
            }
        }
    }
}