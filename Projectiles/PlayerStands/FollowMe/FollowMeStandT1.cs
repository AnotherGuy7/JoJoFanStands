using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;

namespace JoJoFanStands.Projectiles.PlayerStands.FollowMe
{
    public class FollowMeStandT1 : StandClass
    {
        public override int punchDamage => 16;
        public override int altDamage => 31;
        public override int halfStandHeight => 39;
        public override int standType => 1;

        private Vector2 velocityAddition;
        private float mouseDistance;
        private bool grabbing = false;
        private float windUpForce = 1f;

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

            if (Main.mouseLeft || Main.mouseRight && !grabbing)
            {
                float rotaY = Main.MouseWorld.Y - projectile.Center.Y;
                projectile.rotation = MathHelper.ToRadians((rotaY * projectile.spriteDirection) / 6f);

                projectile.direction = 1;
                if (Main.MouseWorld.X < projectile.position.X)
                    projectile.direction = -1;
                projectile.spriteDirection = projectile.direction;
                /*if (projectile.position.X < Main.MouseWorld.X - 5f)
                {
                    velocityAddition.X = 5f;
                }
                if (projectile.position.X > Main.MouseWorld.X + 5f)
                {
                    velocityAddition.X = -5f;
                }
                if (projectile.position.X > Main.MouseWorld.X - 5f && projectile.position.X < Main.MouseWorld.X + 5f)
                {
                    velocityAddition.X = 0f;
                }
                if (projectile.position.Y > Main.MouseWorld.Y + 5f)
                {
                    velocityAddition.Y = -5f;
                }
                if (projectile.position.Y < Main.MouseWorld.Y - 5f)
                {
                    velocityAddition.Y = 5f;
                }
                if (projectile.position.Y < Main.MouseWorld.Y + 5f && projectile.position.Y > Main.MouseWorld.Y - 5f)
                {
                    velocityAddition.Y = 0f;
                }*/
                velocityAddition = Main.MouseWorld - projectile.position;
                velocityAddition.Normalize();
                velocityAddition *= 5f;
                mouseDistance = Vector2.Distance(Main.MouseWorld, projectile.Center);
                if (mouseDistance > 40f)
                {
                    projectile.velocity = player.velocity + velocityAddition;
                }
                if (mouseDistance <= 40f)
                {
                    projectile.velocity = Vector2.Zero;
                }
            }
            if (Main.mouseLeft && projectile.owner == Main.myPlayer)
            {
                normalFrames = false;
                attackFrames = true;
                windUpForce += 0.02f;
                Main.mouseRight = false;
                projectile.frame = 1;
            }
            else
            {
                if (!grabbing && !Main.mouseRight && windUpForce == 1f)
                {
                    Vector2 vector131 = player.Center;
                    vector131.X -= (float)((9 + player.width / 2) * player.direction);
                    vector131.Y -= -35f + halfStandHeight;
                    projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
                    projectile.velocity *= 0.8f;
                    projectile.direction = (projectile.spriteDirection = player.direction);
                    projectile.rotation = 0;
                    normalFrames = true;
                }
            }
            if (!Main.mouseLeft && windUpForce != 1f && projectile.frame == 6)
            {
                shootCount += 12;
                Vector2 shootVel = Main.MouseWorld - projectile.Center;
                if (shootVel == Vector2.Zero)
                {
                    shootVel = new Vector2(0f, 1f);
                }
                shootVel.Normalize();
                shootVel *= shootSpeed;
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, mod.ProjectileType("Fists"), (int)(newPunchDamage * windUpForce), 4f * windUpForce, Main.myPlayer);
                Main.projectile[proj].timeLeft = 6;
                Main.projectile[proj].netUpdate = true;
                projectile.netUpdate = true;
                windUpForce = 1f;
                normalFrames = true;
                attackFrames = false;
            }
            if (Main.mouseRight && projectile.owner == Main.myPlayer && shootCount <= 0 && !grabbing)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active)
                    {
                        if (projectile.Distance(npc.Center) <= 30f && !npc.boss && !npc.immortal && !npc.hide)
                        {
                            projectile.ai[0] = npc.whoAmI;
                            grabbing = true;
                        }
                    }
                }
            }
            if (grabbing && projectile.ai[0] != -1f)
            {
                projectile.velocity = Vector2.Zero;
                NPC npc = Main.npc[(int)projectile.ai[0]];
                npc.direction = -projectile.direction;
                npc.position = projectile.position + new Vector2(5f * projectile.direction, -2f - npc.height / 3f);
                npc.velocity = Vector2.Zero;
                if (projectile.frame == 7)
                {
                    npc.StrikeNPC(newPunchDamage, 7f, projectile.direction, true);
                    shootCount += 180;
                    projectile.ai[0] = -1f;
                    grabbing = false;
                }
            }
            if (!grabbing)
            {
                LimitDistance();
            }
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(grabbing);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            grabbing = reader.ReadBoolean();
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                normalFrames = false;
                PlayAnimation("WindUp");
            }
            if (normalFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
            if (grabbing)
            {
                normalFrames = false;
                attackFrames = false;
                secondaryAbilityFrames = false;
                PlayAnimation("Grab");
            }
            if (Main.mouseRight && projectile.owner == Main.myPlayer && shootCount <= 0 && !grabbing)       //loops grabbing
            {
                if (projectile.frame <= 0)
                {
                    projectile.frame = 1;
                }
                if (projectile.frame >= 3)
                {
                    projectile.frame = 1;
                }
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = mod.GetTexture("Projectiles/PlayerStands/FollowMe/FollowMe_" + animationName);
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 15, true);
            }
            if (animationName == "WindUp")
            {
                AnimateStand(animationName, 7, 6, false);
            }
            if (animationName == "Grab")
            {
                AnimateStand(animationName, 9, 12, false);
            }
        }
    }
}