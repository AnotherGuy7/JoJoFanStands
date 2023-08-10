using JoJoFanStands.UI;
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
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        public static int abilityNumber = 0;

        private string abilityName = "PushBack";
        private string lookDirection = "Straight";

        public new enum AnimationState
        {
            Up,
            SlightUp,
            Straight,
            Down,
            PushBack,
            ForceField,
            Crystal,
            Gravity,
            Genocide
        }


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

            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed && !AbilityChooserUI.Visible)
                AbilityChooserUI.Visible = true;

            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.mouseLeft && abilityNumber == 0)
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
                                    NPC.HitInfo hitInfo = new NPC.HitInfo()
                                    {
                                        Damage = 97,
                                        HitDirection = -Projectile.direction
                                    };
                                    npc.StrikeNPC(hitInfo);
                                    NetMessage.SendStrikeNPC(npc, hitInfo, Main.myPlayer);
                                }
                            }
                        }
                    }
                }

                if (abilityNumber == 0)
                {
                    if (Main.mouseY <= Main.screenHeight / 5)
                        lookDirection = "Up";
                    if (Main.mouseY > Main.screenHeight / 5 && Main.mouseY < (Main.screenHeight / 5) * 2)
                        lookDirection = "SlightUp";
                    if (Main.mouseY > (Main.screenHeight / 5) * 2 && Main.mouseY < (Main.screenHeight / 5) * 4)
                        lookDirection = "Straight";
                    if (Main.mouseY > (Main.screenHeight / 5) * 4)
                        lookDirection = "Down";
                }
                else if (abilityNumber == 1)     //push everything away
                {
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
                else if (abilityNumber == 2)     //forcefield
                {
                    abilityName = "ForceField";
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
                else if (abilityNumber == 3)     //crystal shower
                {
                    abilityName = "Crystal";
                }
                else if (abilityNumber == 4)     //make NPCs reverse gravity
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
                        else
                        {
                            npc.noGravity = false;
                            npc.velocity.Y += 1f;
                        }
                    }
                }
                else if (abilityNumber == 5)     //non-existant: Whatever this Projectile touches will no longer ever spawn in the game for the time you are in the world
                {
                    abilityName = "Genocide";
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
                                    Main.NewText("The amount of things that can't exist has already been exceeded...");
                            }
                        }
                    }
                }
                else if (abilityNumber == 6)
                {
                    abilityName = "MonotoneReality";
                    if (!Main.dedServ)
                    {
                        if (!Filters.Scene["MonotoneRealityEffect"].IsActive())
                            Filters.Scene.Activate("MonotoneRealityEffect");
                    }
                    abilityNumber = 0;
                }
            }

            if (!Main.dedServ)
            {
                if (Filters.Scene["MonotoneRealityEffect"].IsActive())
                    Filters.Scene["MonotoneRealityEffect"].Deactivate();
            }

            if (abilityNumber == 0)
            {
                if (lookDirection == "Up")
                    currentAnimationState = AnimationState.Up;
                else if (lookDirection == "SlightUp")
                    currentAnimationState = AnimationState.SlightUp;
                else if (lookDirection == "Straight")
                    currentAnimationState = AnimationState.Straight;
                else if (lookDirection == "Down")
                    currentAnimationState = AnimationState.Down;
            }
            else
            {
                if (abilityNumber == 0)
                    currentAnimationState = AnimationState.PushBack;
                else if (abilityNumber == 1)
                    currentAnimationState = AnimationState.ForceField;
                else if (abilityNumber == 2)
                    currentAnimationState = AnimationState.Crystal;
                else if (abilityNumber == 3)
                    currentAnimationState = AnimationState.Gravity;
                else if (abilityNumber == 4)
                    currentAnimationState = AnimationState.Genocide;
            }
        }


        public override byte SendAnimationState() => (byte)currentAnimationState;
        public override void ReceiveAnimationState(byte state) => currentAnimationState = (AnimationState)state;

        public override void SelectAnimation()
        {
            if (oldAnimationState != currentAnimationState)
            {
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
                oldAnimationState = currentAnimationState;
                Projectile.netUpdate = true;
            }

            if (currentAnimationState == AnimationState.Up)
                PlayAnimation("Up");
            else if (currentAnimationState == AnimationState.SlightUp)
                PlayAnimation("SlightUp");
            else if (currentAnimationState == AnimationState.Straight)
                PlayAnimation("Straight");
            else if (currentAnimationState == AnimationState.Down)
                PlayAnimation("Down");
            else if (currentAnimationState == AnimationState.PushBack)
                PlayAnimation("PushBack");
            else if (currentAnimationState == AnimationState.ForceField)
                PlayAnimation("ForceField");
            else if (currentAnimationState == AnimationState.Crystal)
                PlayAnimation("Crystal");
            else if (currentAnimationState == AnimationState.Gravity)
                PlayAnimation("Gravity");
            else if (currentAnimationState == AnimationState.Genocide)
                PlayAnimation("Genocide");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Megalovania/M" + animationName).Value;
            if (animationName == "Up")
                AnimateStand(animationName, 2, 30, true);
            else if (animationName == "SlightUp")
                AnimateStand(animationName, 2, 30, true);
            else if (animationName == "Straight")
                AnimateStand(animationName, 2, 30, true);
            else if (animationName == "Down")
                AnimateStand(animationName, 2, 30, true);
            else if (animationName == "PushBack")
                AnimateStand(animationName, 1, 120, false);
            else if (animationName == "ForceField")
                AnimateStand(animationName, 1, 1200, false);
            else if (animationName == "Crystal")
                AnimateStand(animationName, 1, 375, false);
            else if (animationName == "Gravity")
                AnimateStand(animationName, 1, 120, false);
            else if (animationName == "Genocide")
                AnimateStand(animationName, 1, 375, false);
        }
    }
}