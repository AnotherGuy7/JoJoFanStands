using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.Megalovania
{
    public class MegalovaniaStand : StandClass
    {
        public override int ProjectileDamage => 18;
        public override int ShootTime => 40;
        public override int AltDamage => 96;
        public override int HalfStandHeight => 34;      //28
        public override Vector2 StandOffset => new Vector2(-25, 0);
        public override float MaxDistance => 0f;

        public static int abilityNumber = 0;

        private string abilityName = "PushBack";
        private string direction = "Straight";
        private int maxFrames = 0;


        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (shootCount > 0)
                shootCount--;
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;
            DrawOriginOffsetY = -HalfStandHeight;
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
                                npc.StrikeNPC(97, 0f, Projectile.direction * -1);
                            }
                        }
                    }
                }
            }
            if (player.whoAmI == Main.myPlayer)
            {
                Projectile.direction = 1;
                if (Main.mouseX < Projectile.position.X)
                {
                    Projectile.direction = -1;
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
                        Vector2 pushBackVelocity = npc.position - Projectile.position;
                        pushBackVelocity.Normalize();
                        pushBackVelocity *= 25f;
                        npc.velocity += pushBackVelocity;
                    }
                }
            }

            if (abilityNumber == 2)     //forcefield
            {
                abilityName = "ForceField";
                //AnimateStand(1, 0.05f, false, true);
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && Projectile.Distance(npc.Center) <= 60f)
                    {
                        Vector2 pushBackVelocity = npc.position - Projectile.position;
                        pushBackVelocity.Normalize();
                        pushBackVelocity *= 10f;
                        npc.velocity += pushBackVelocity;
                    }
                }
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile otherProj = Main.projectile[p];
                    if (otherProj.active && otherProj.hostile && Projectile.Distance(otherProj.Center) <= 60f)
                    {
                        Vector2 pushBackVelocity = otherProj.position - Projectile.position;
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

            if (abilityNumber == 5)     //non-existant: Whatever this Projectile touches will no longer ever spawn in the game for the time you are in the world
            {
                abilityName = "Genocide";
                //AnimateStand(1, 0.16f, false, true);
                if (shootCount <= 0)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (npc.active && Vector2.Distance(Main.MouseWorld, npc.position) < 25f)
                        {
                            shootCount += 60;
                            //Main.npc[i].StrikeNPC(10, 0f, Projectile.direction * -1);
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

        /*public virtual void AnimateStand(int frameAmount, float fps, bool loop, bool ability)        //remember that 'fps' refers to how many frames is supposed to play every second, not how fast it plays
        {
            Main.projFrames[Projectile.whoAmI] = frameAmount;
            Projectile.frameCounter++;
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Megalovania/M" + direction);
            float framesPerSecond = 60f / fps;
            if (Projectile.frameCounter >= framesPerSecond)
            {
                Projectile.frameCounter = 0;
                Projectile.frame += 1;
            }
            if (Projectile.frame >= frameAmount && loop)
            {
                Projectile.frame = 0;
            }
            if (Projectile.frame >= frameAmount && !loop)
            {
                direction = "Idle";
            }
            if (Projectile.frame >= frameAmount && ability)
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
                Projectile.frame = 0;
                PlayAnimation(abilityName);
                maxFrames = 1;
                if (Projectile.frame >= maxFrames)
                {
                    abilityNumber = 0;
                }
            }
            /*if (attackFrames)
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
            }*/
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Megalovania/M" + animationName).Value;
            if (animationName == "Up")
            {
                AnimateStand(animationName, 2, 30, true);
            }
            if (animationName == "SlightUp")
            {
                AnimateStand(animationName, 2, 30, true);
            }
            if (animationName == "Straight")
            {
                AnimateStand(animationName, 2, 30, true);
            }
            if (animationName == "Down")
            {
                AnimateStand(animationName, 2, 30, true);
            }
            if (animationName == "PushBack")
            {
                AnimateStand(animationName, 1, 120, false);
            }
            if (animationName == "ForceField")
            {
                AnimateStand(animationName, 1, 1200, false);
            }
            if (animationName == "Crystal")
            {
                AnimateStand(animationName, 1, 375, false);
            }
            if (animationName == "Gravity")
            {
                AnimateStand(animationName, 1, 120, false);
            }
            if (animationName == "Genocide")
            {
                AnimateStand(animationName, 1, 375, false);
            }
        }
    }
}