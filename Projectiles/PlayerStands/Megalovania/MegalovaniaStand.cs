using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;

namespace JoJoFanStands.Projectiles.PlayerStands.Megalovania
{
    public class MegalovaniaStand : ModProjectile      //has 2 poses
    {
        public override void SetStaticDefaults()
        {
            //Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 38;
            projectile.height = 1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.netImportant = true;
            //projectile.minionSlots = 1;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            MyPlayer.stopImmune.Add(mod.ProjectileType(Name));
        }

        public Vector2 velocityAddition = Vector2.Zero;
        public float mouseDistance = 0f;
        protected float shootSpeed = 16f;
        public float maxDistance = 0f;
        public int punchDamage = 18;
        public int altDamage = 96;
        int shootCount = 0;
        public int halfStandHeight = 28;
        public Mod JoJoStands2 = null;
        public static int abilityNumber = 0;
        public string animationName;

        public Texture2D standTexture;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            if (player.HeldItem.type == mod.ItemType("Megalovania"))
            {
                projectile.timeLeft = 2;
            }
            if (player.HeldItem.type != mod.ItemType("Megalovania") || player.dead)
            {
                projectile.active = false;
            }
            if (JoJoStands2 == null)
            {
                JoJoStands2 = ModLoader.GetMod("JoJoStands");
            }
            drawOriginOffsetY = -halfStandHeight;
            if (Main.mouseLeft && abilityNumber == 0)
            {
                if (shootCount <= 0)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        float NPCdist = Vector2.Distance(Main.MouseWorld, Main.npc[i].position);
                        if (NPCdist < 25f)
                        {
                            shootCount += 60;
                            Main.npc[i].StrikeNPC(97, 0f, projectile.direction * -1);
                        }
                    }
                }
            }
            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed && !UI.AbilityChooserUI.Visible)
            {
                UI.AbilityChooserUI.Visible = true;
            }
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
                    animationName = "Up";
                    AnimationStates(2, 1.5f, true, false);
                }
                if (Main.mouseY > Main.screenHeight / 5 && Main.mouseY < (Main.screenHeight / 5) * 2)
                {
                    animationName = "SlightUp";
                    AnimationStates(2, 1.5f, true, false);
                }
                if (Main.mouseY > (Main.screenHeight / 5) * 2 && Main.mouseY < (Main.screenHeight / 5) * 4)
                {
                    animationName = "Straight";
                    AnimationStates(2, 1.5f, true, false);
                }
                if (Main.mouseY > (Main.screenHeight / 5) * 4)
                {
                    animationName = "Down";
                    AnimationStates(2, 1.5f, true, false);
                }
            }
            //Main.NewText(Main.mouseY);
            Vector2 vector131 = player.Center;
            vector131.X -= (float)((39 + player.width / 2) * player.direction);
            vector131.Y -= -35f + halfStandHeight;
            if (!Main.mouseLeft)
            {
                projectile.direction = player.direction;
            }
            projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
            projectile.velocity *= 0.8f;
            projectile.rotation = 0;
            if (projectile.direction == -1)
            {
                drawOffsetX = -30;
            }
            else
            {
                drawOffsetX = 0;
            }
            if (shootCount > 0)
            {
                shootCount--;
            }

            if (abilityNumber == 1)     //push everything away
            {
                animationName = "PushBack";
                AnimationStates(1, 0.5f, false, true);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.position.X >= projectile.position.X)
                    {
                        npc.velocity.X = 25f;
                    }
                    if (npc.position.X < projectile.position.X)
                    {
                        npc.velocity.X = -25f;
                    }
                    if (npc.position.Y >= projectile.position.Y)
                    {
                        npc.velocity.Y = 25f;
                    }
                    if (npc.position.Y < projectile.position.Y)
                    {
                        npc.velocity.Y = -25f;
                    }
                }
            }
            if (abilityNumber == 2)     //forcefield
            {
                animationName = "ForceField";
                AnimationStates(1, 0.05f, false, true);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    float npcDist = Vector2.Distance(npc.Center, player.Center);
                    if (npcDist <= 60f)
                    {
                        npc.velocity.X = 10f * -npc.direction;
                        if (npc.position.Y >= projectile.position.Y)
                        {
                            npc.velocity.Y = 10f;
                        }
                        if (npc.position.Y < projectile.position.Y)
                        {
                            npc.velocity.Y = -10f;
                        }
                    }
                }
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile otherProjectile = Main.projectile[i];
                    float projectileDist = Vector2.Distance(otherProjectile.Center, player.Center);
                    if (projectileDist <= 60f && otherProjectile.hostile)
                    {
                        projectile.velocity = -projectile.velocity * 10f;
                    }
                }
            }
            if (abilityNumber == 3)     //crystal shower
            {
                animationName = "Crystal";
                AnimationStates(1, 0.16f, false, true);
                if (shootCount <= 0)
                {
                    shootCount += 85;
                    Projectile.NewProjectile(projectile.position.X + 5f, projectile.position.Y, 0f, 0f, mod.ProjectileType("Crystal"), 82, 2f, Main.myPlayer);
                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y - 5f, 0f, 0f, mod.ProjectileType("Crystal"), 82, 2f, Main.myPlayer);
                    Projectile.NewProjectile(projectile.position.X, projectile.position.Y + 5f, 0f, 0f, mod.ProjectileType("Crystal"), 82, 2f, Main.myPlayer);
                    Projectile.NewProjectile(projectile.position.X - 5f, projectile.position.Y, 0f, 0f, mod.ProjectileType("Crystal"), 82, 2f, Main.myPlayer);
                }
            }
            if (abilityNumber == 4)     //make NPCs reverse gravity
            {
                animationName = "Gravity";
                AnimationStates(1, 0.5f, false, true);
                if (shootCount <= 0)
                {
                    shootCount += 120;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
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
            }
            if (abilityNumber == 5)     //non-existant: Whatever this projectile touches will no longer ever spawn in the game for the time you are in the world
            {
                animationName = "Genocide";
                AnimationStates(1, 0.16f, false, true);
                if (shootCount <= 0)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        float NPCdist = Vector2.Distance(Main.MouseWorld, Main.npc[i].position);
                        if (NPCdist < 25f)
                        {
                            shootCount += 60;
                            //Main.npc[i].StrikeNPC(10, 0f, projectile.direction * -1);
                            Main.npc[i].GetGlobalNPC<NPCs.FanGlobalNPC>().nonExistant = true;
                            if (NPCs.FanGlobalNPC.nonExistantTypes.Count != NPCs.FanGlobalNPC.nonExistantTypes.Capacity)
                            {
                                NPCs.FanGlobalNPC.nonExistantTypes.Add(Main.npc[i].type);
                                Main.NewText(Main.npc[i].TypeName + "s don't exist anymore.");
                            }
                            else
                            {
                                Main.NewText("The amount of things that can't exist has already been exceeded...");
                            }
                        }
                    }
                }
            }
        }

        public SpriteEffects effects = SpriteEffects.None;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.direction == -1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            if (projectile.direction == 1)
            {
                effects = SpriteEffects.None;
            }
            int frameHeight = standTexture.Height / Main.projFrames[projectile.whoAmI];
            spriteBatch.Draw(standTexture, projectile.Center - Main.screenPosition + new Vector2(19f, 1f), new Rectangle(0, frameHeight * projectile.frame, standTexture.Width, frameHeight), Color.White, 0f, new Vector2(standTexture.Width / 2f, frameHeight / 2f), 1f, effects, 0);

            if (Main.mouseLeft && abilityNumber == 0)
            {
                Texture2D texture = mod.GetTexture("Projectiles/PlayerStands/Megalovania/M" + animationName + "A");     //the attack stare stuff
                spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(19f, 1f), new Rectangle(0, frameHeight * projectile.frame, texture.Width, frameHeight), Color.White, 0f, new Vector2(texture.Width / 2f, frameHeight / 2f), 1f, effects, 0);
            }
        }

        public virtual void AnimationStates(int frameAmount, float fps, bool loop, bool ability)        //remember that 'fps' refers to how many frames is supposed to play every second, not how fast it plays
        {
            Main.projFrames[projectile.whoAmI] = frameAmount;
            projectile.frameCounter++;
            standTexture = mod.GetTexture("Projectiles/PlayerStands/Megalovania/M" + animationName);
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
                animationName = "Idle";
            }
            if (projectile.frame >= frameAmount && ability)
            {
                animationName = "Idle";
                abilityNumber = 0;
            }
        }
    }
}