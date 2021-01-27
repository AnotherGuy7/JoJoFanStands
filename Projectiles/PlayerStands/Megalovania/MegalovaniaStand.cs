using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;

namespace JoJoFanStands.Projectiles.PlayerStands.Megalovania
{
    public class MegalovaniaStand : StandClass
    {
        public override int projectileDamage => 18;
        public override int shootTime => 40;
        public override int altDamage => 96;
        public override int halfStandHeight => 34;      //28
        public override int standOffset => -25;
        public override float maxDistance => 0f;

        public static int abilityNumber = 0;

        private string abilityName = "PushBack";
        private string direction = "Straight";
        private float mouseDistance = 0f;
        private int maxFrames = 0;


        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (shootCount > 0)
            {
                shootCount--;
            }
            if (mPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }
            drawOriginOffsetY = -halfStandHeight;
            StayBehind();

            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed && !UI.AbilityChooserUI.Visible)
            {
                UI.AbilityChooserUI.Visible = true;
            }

            if (Main.mouseLeft && abilityNumber == 0 && player.whoAmI == Main.myPlayer)
            {
                if (shootCount <= 0)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (npc.active)
                        {
                            float npcDist = Vector2.Distance(Main.MouseWorld, npc.position);
                            if (npcDist <= 25f)
                            {
                                shootCount += newShootTime;
                                npc.StrikeNPC(97, 0f, projectile.direction * -1);
                            }
                        }
                    }
                }
            }
            if (player.whoAmI == Main.myPlayer)
            {
                if (Main.mouseX >= projectile.position.X)
                {
                    projectile.direction = 1;
                }
                if (Main.mouseX < projectile.position.X)
                {
                    projectile.direction = -1;
                }
                if (abilityNumber == 0)
                {
                    if (Main.mouseY <= Main.screenHeight / 5)
                    {
                        direction = "Up";
                    }
                    else if (Main.mouseY > Main.screenHeight / 5 && Main.mouseY < (Main.screenHeight / 5) * 2)
                    {
                        direction = "SlightUp";
                    }
                    else if (Main.mouseY > (Main.screenHeight / 5) * 2 && Main.mouseY < (Main.screenHeight / 5) * 4)
                    {
                        direction = "Straight";
                    }
                    else if (Main.mouseY > (Main.screenHeight / 5) * 4)
                    {
                        direction = "Down";
                    }
                }
            }


            if (abilityNumber == 1)     //push everything away
            {
                abilityName = "PushBack";
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active)
                    {
                        Vector2 pushBackVelocity = npc.position - projectile.position;
                        pushBackVelocity.Normalize();
                        pushBackVelocity *= 25f;
                        npc.velocity += pushBackVelocity;
                    }
                }
            }

            if (abilityNumber == 2)     //forcefield
            {
                abilityName = "ForceField";
                //AnimationStates(1, 0.05f, false, true);
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && projectile.Distance(npc.Center) <= 60f)
                    {
                        Vector2 pushBackVelocity = npc.position - projectile.position;
                        pushBackVelocity.Normalize();
                        pushBackVelocity *= 10f;
                        npc.velocity += pushBackVelocity;
                    }
                }
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile otherProj = Main.projectile[p];
                    if (otherProj.active && otherProj.hostile && projectile.Distance(otherProj.Center) <= 60f)
                    {
                        Vector2 pushBackVelocity = otherProj.position - projectile.position;
                        pushBackVelocity.Normalize();
                        pushBackVelocity *= 25f;
                        otherProj.velocity += pushBackVelocity;
                    }
                }
            }

            if (abilityNumber == 3)     //crystal shower
            {
                abilityName = "Crystal";
            }

            if (abilityNumber == 4)     //make NPCs reverse gravity
            {
                abilityName = "Gravity";
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (!npc.noGravity)
                    {
                        npc.noGravity = true;
                        npc.velocity.Y -= 0.1f;
                    }
                    if (npc.noGravity)
                    {
                        npc.noGravity = false;
                        npc.velocity.Y += 1f;
                    }
                }
            }

            if (abilityNumber == 5)     //non-existant: Whatever this projectile touches will no longer ever spawn in the game for the time you are in the world
            {
                abilityName = "Genocide";
                //AnimationStates(1, 0.16f, false, true);
                if (shootCount <= 0)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (npc.active && Vector2.Distance(Main.MouseWorld, npc.position) < 25f)
                        {
                            shootCount += 60;
                            //Main.npc[i].StrikeNPC(10, 0f, projectile.direction * -1);
                            npc.GetGlobalNPC<NPCs.FanGlobalNPC>().nonExistant = true;
                            if (NPCs.FanGlobalNPC.nonExistantTypes.Count != NPCs.FanGlobalNPC.nonExistantTypes.Capacity)
                            {
                                NPCs.FanGlobalNPC.nonExistantTypes.Add(npc.type);
                                Main.NewText(npc.TypeName + "s don't exist anymore.");
                            }
                            else
                            {
                                Main.NewText("The amount of things that can't exist has already been exceeded...");
                            }
                        }
                    }
                }
            }

            if (abilityNumber == 6)
            {
                abilityName = "MonotoneReality";

                if (!Main.dedServ)
                {
                    if (!Filters.Scene["MonotoneRealityEffect"].IsActive())
                    {
                        Filters.Scene.Activate("MonotoneRealityEffect");
                    }
                }
                if (Main.mouseRight)
                {
                    abilityNumber = 0;
                }
            }
            else
            {
                if (!Main.dedServ)
                {
                    if (Filters.Scene["MonotoneRealityEffect"].IsActive())
                    {
                        Filters.Scene["MonotoneRealityEffect"].Deactivate();
                    }
                }
            }
        }

        /*public virtual void AnimationStates(int frameAmount, float fps, bool loop, bool ability)        //remember that 'fps' refers to how many frames is supposed to play every second, not how fast it plays
        {
            Main.projFrames[projectile.whoAmI] = frameAmount;
            projectile.frameCounter++;
            standTexture = mod.GetTexture("Projectiles/PlayerStands/Megalovania/M" + direction);
            float framesPerSecond = 60f / fps;
            if (projectile.frameCounter >= framesPerSecond)
            {
                projectile.frameCounter = 0;
                projectile.frame += 1;
            }
            if (projectile.frame >= frameAmount && loop)
            {
                projectile.frame = 0;
            }
            if (projectile.frame >= frameAmount && !loop)
            {
                direction = "Idle";
            }
            if (projectile.frame >= frameAmount && ability)
            {
                direction = "Idle";
                abilityNumber = 0;
            }
        }*/

        public override void SelectAnimation()
        {
            if (abilityNumber == 0)
            {
                PlayAnimation(direction);
                maxFrames = 2;
            }
            else
            {
                projectile.frame = 0;
                PlayAnimation(abilityName);
                maxFrames = 1;
                if (projectile.frame >= maxFrames)
                {
                    abilityNumber = 0;
                }
            }
            /*if (attackFrames)
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
            }*/
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = mod.GetTexture("Projectiles/PlayerStands/Megalovania/M" + animationName);
            if (animationName == "Up")
            {
                AnimationStates(animationName, 2, 30, true);
            }
            if (animationName == "SlightUp")
            {
                AnimationStates(animationName, 2, 30, true);
            }
            if (animationName == "Straight")
            {
                AnimationStates(animationName, 2, 30, true);
            }
            if (animationName == "Down")
            {
                AnimationStates(animationName, 2, 30, true);
            }
            if (animationName == "PushBack")
            {
                AnimationStates(animationName, 1, 120, false);
            }
            if (animationName == "ForceField")
            {
                AnimationStates(animationName, 1, 1200, false);
            }
            if (animationName == "Crystal")
            {
                AnimationStates(animationName, 1, 375, false);
            }
            if (animationName == "Gravity")
            {
                AnimationStates(animationName, 1, 120, false);
            }
            if (animationName == "Genocide")
            {
                AnimationStates(animationName, 1, 375, false);
            }
        }
    }
}