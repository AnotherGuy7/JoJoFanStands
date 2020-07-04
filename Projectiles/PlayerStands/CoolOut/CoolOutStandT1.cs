using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Terraria.ModLoader;
using System.Security.Policy;

namespace JoJoFanStands.Projectiles.PlayerStands
{  
    public class CoolOutStandT1 : StandClass
    {
        public override int projectileDamage => 17;
        public override int shootTime => 12;
        public override int altDamage => 20;
        public override int standType => 2;
        public override int standOffset => 20;
        public override int halfStandHeight => 32;

        private int specialDamage = 25;
        private bool altAttacking = false;
        private int spearWhoAmI = -1;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            SelectAnimation();
            UpdateStandInfo();
            StayBehind();
            Lighting.AddLight(projectile.position, 1.78f, 2.21f, 2.54f);
            if (mPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }

            if (Main.mouseLeft)
            {
                attackFrames = true;
                if (shootCount <= 0f)
                {
                    Main.PlaySound(SoundID.Item28, projectile.position);
                    shootCount += 30;
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    shootVel.Normalize();
                    shootVel *= shootSpeed;
                    int proj = Projectile.NewProjectile(projectile.Center, shootVel, mod.ProjectileType("IceBolt"), projectileDamage, 8f, Main.myPlayer);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                normalFrames = true;
                attackFrames = false;
                secondaryAbilityFrames = false;
            }
            if (Main.mouseRight && shootCount <= 0f && player.ownedProjectileCounts[mod.ProjectileType("IceSpear")] == 0 && projectile.owner == Main.myPlayer)
            {
                spearWhoAmI = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("IceSpear"), altDamage, 10f, Main.myPlayer, projectile.whoAmI);
                Main.projectile[spearWhoAmI].netUpdate = true;
                projectile.netUpdate = true;
            }
            if (Main.mouseRight && spearWhoAmI != -1)
            {
                Projectile spear = Main.projectile[spearWhoAmI];
                projectile.ai[0] += 0.005f;
                if (projectile.ai[0] >= 2f)
                {
                    player.AddBuff(BuffID.Chilled, 2);
                }
                Vector2 direction = Main.MouseWorld - projectile.Center;
                spear.scale += projectile.ai[0];
                spear.rotation = direction.ToRotation();
                spear.velocity = Vector2.Zero;
                spear.position = projectile.position;
            }
            if (!Main.mouseRight && spearWhoAmI != -1)
            {
                Projectile spear = Main.projectile[spearWhoAmI];
                spear.damage *= (int)projectile.ai[0] + 1;
                Vector2 direction = Main.MouseWorld - projectile.Center;
                direction.Normalize();
                //spear.velocity = (direction * 5f) * projectile.ai[0];
            }
            if (spearWhoAmI != -1)
            {
                Projectile spear = Main.projectile[spearWhoAmI];
                if (!spear.active)
                {
                    projectile.ai[0] = 0f;
                    spearWhoAmI = -1;
                }
            }
            if (SpecialKeyPressed() && shootCount <= 0f)
            {
                normalFrames = false;
                attackFrames = false;
                secondaryAbilityFrames = true;
                Main.PlaySound(SoundID.Item28, projectile.position);
                shootCount += 600;        //make it 600, 10 seconds
                int proj = Projectile.NewProjectile(projectile.Center.X + 7f, projectile.Center.Y + 3f, 0f, 0f, mod.ProjectileType("IceSpike"), specialDamage, 2f, Main.myPlayer);
                Main.projectile[proj].netUpdate = true;
                projectile.netUpdate = true;
            }
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
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
            if (Main.player[projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                normalFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = mod.GetTexture("Projectiles/PlayerStands/CoolOut/CoolOut_" + animationName);
            if (animationName == "Idle")
            {
                AnimationStates(animationName, 8, 15, true);
            }
            if (animationName == "Attack")
            {
                AnimationStates(animationName, 4, 14, true);
            }
            if (animationName == "Slam")
            {
                AnimationStates(animationName, 1, 300, true);
            }
            if (animationName == "Pose")
            {
                AnimationStates(animationName, 2, 15, true);
            }
        }
    }
}