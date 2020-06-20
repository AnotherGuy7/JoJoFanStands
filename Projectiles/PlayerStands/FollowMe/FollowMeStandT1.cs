using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;

namespace JoJoFanStands.Projectiles.PlayerStands.FollowMe
{
    public class FollowMeStandT1 : ModProjectile      //has 2 poses
    {
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
            projectile.timeLeft = 2;
            projectile.ai[0] = -1f;
            MyPlayer.stopImmune.Add(mod.ProjectileType(Name));
        }

        public Vector2 velocityAddition = Vector2.Zero;
        public float mouseDistance = 0f;
        protected float shootSpeed = 16f;
        public float maxDistance = 0f;
        public int punchDamage = 21;
        public int altDamage = 48;
        int shootCount = 0;
        public int halfStandHeight = 39;
        public Mod JoJoStands2 = null;
        public bool grabbing = false;
        public bool intangible = false;
        public float windUpForce = 0f;

        public Texture2D standTexture;

        //projectile.ai[0] = the enemy being grabbed's whoAmI

        public override void AI()
        {
            if (shootCount > 0)
            {
                shootCount--;
            }
            Player player = Main.player[projectile.owner];
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer Fplayer = player.GetModPlayer<FanPlayer>();
            projectile.frameCounter++;
            if (player.HeldItem.type == mod.ItemType("FollowMeT1") && Fplayer.StandOut)
            {
                projectile.timeLeft = 2;
            }
            if (player.HeldItem.type != mod.ItemType("FollowMeT1") || !Fplayer.StandOut || player.dead)
            {
                projectile.active = false;
            }
            if (JoJoStands2 == null)
            {
                JoJoStands2 = ModLoader.GetMod("JoJoStands");
            }
            drawOriginOffsetY = -halfStandHeight;

            if (projectile.direction == -1)
            {
                drawOffsetX = -30;
            }
            else
            {
                drawOffsetX = 0;
            }

            if (Main.mouseLeft || Main.mouseRight && !grabbing)
            {
                float rotaY = Main.MouseWorld.Y - projectile.Center.Y;
                projectile.rotation = MathHelper.ToRadians((rotaY * projectile.spriteDirection) / 6f);
                if (Main.MouseWorld.X > projectile.position.X)
                {
                    projectile.spriteDirection = 1;
                    projectile.direction = 1;
                }
                if (Main.MouseWorld.X < projectile.position.X)
                {
                    projectile.spriteDirection = -1;
                    projectile.direction = -1;
                }
                if (projectile.position.X < Main.MouseWorld.X - 5f)
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
                }
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
                windUpForce += 0.02f;
                SwitchStatesTo("WindUp");
                Main.mouseRight = false;
            }
            else
            {
                if (!grabbing && !Main.mouseRight && windUpForce == 0f)
                {
                    Vector2 vector131 = player.Center;
                    vector131.X -= (float)((36 + player.width / 2) * player.direction);
                    vector131.Y -= -35f + halfStandHeight;
                    projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
                    projectile.velocity *= 0.8f;
                    projectile.direction = (projectile.spriteDirection = player.direction);
                    projectile.rotation = 0;
                    if (!intangible)
                    {
                        SwitchStatesTo("Idle");
                    }
                    else
                    {
                        SwitchStatesTo("NoClip");
                    }
                }
            }
            if (!Main.mouseLeft && windUpForce != 0f)
            {
                shootCount += 12;
                Vector2 shootVel = Main.MouseWorld - projectile.Center;
                if (shootVel == Vector2.Zero)
                {
                    shootVel = new Vector2(0f, 1f);
                }
                shootVel.Normalize();
                shootVel *= shootSpeed;
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, JoJoStands2.ProjectileType("Fists"), (int)((punchDamage * modPlayer.standDamageBoosts) * windUpForce), 2f, Main.myPlayer, -1f, -1f);
                Main.projectile[proj].timeLeft = 6;
                Main.projectile[proj].netUpdate = true;
                projectile.netUpdate = true;
                windUpForce = 0f;
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
                    npc.StrikeNPC(punchDamage, 7f, projectile.direction, true);
                    shootCount += 180;
                    projectile.ai[0] = -1f;
                    grabbing = false;
                }
                SwitchStatesTo("Grab");
            }
            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed && !player.HasBuff(mod.BuffType("Intangible")) && !player.HasBuff(mod.BuffType("IntangibleCooldown")))
            {
                player.AddBuff(mod.BuffType("Intangible"), 7200);
            }
            intangible = player.HasBuff(mod.BuffType("Intangible"));
            if (!grabbing)
            {
                Vector2 direction = player.Center - projectile.Center;
                float distanceTo = direction.Length();
                maxDistance = 98f + modPlayer.standRangeBoosts;
                if (distanceTo > maxDistance)
                {
                    if (projectile.position.X <= player.position.X - 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X + 0.2f, projectile.position.Y);
                        projectile.velocity = Vector2.Zero;
                    }
                    if (projectile.position.X >= player.position.X + 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X - 0.2f, projectile.position.Y);
                        projectile.velocity = Vector2.Zero;
                    }
                    if (projectile.position.Y >= player.position.Y + 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X, projectile.position.Y - 0.2f);
                        projectile.velocity = Vector2.Zero;
                    }
                    if (projectile.position.Y <= player.position.Y - 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X, projectile.position.Y + 0.2f);
                        projectile.velocity = Vector2.Zero;
                    }
                }
                if (distanceTo >= maxDistance + 22f)
                {
                    Main.mouseLeft = false;
                    Main.mouseRight = false;
                    projectile.Center = player.Center;
                }
            }
        }

        public SpriteEffects effects = SpriteEffects.None;

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            if (projectile.spriteDirection == -1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            if (projectile.spriteDirection == 1)
            {
                effects = SpriteEffects.None;
            }
            /*if (MyPlayer.RangeIndicators)
            {
                Texture2D texture = JoJoStands.GetTexture("Extras/RangeIndicator");        //the initial tile amount the indicator covers is 20 tiles, 320 pixels, border is included in the measurements
                spriteBatch.Draw(texture, player.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), maxDistance / 122.5f, SpriteEffects.None, 0);
            }*/
            if (intangible)
            {
                lightColor.A = 40;
            }
            int frameHeight = standTexture.Height / Main.projFrames[projectile.whoAmI];
            spriteBatch.Draw(standTexture, projectile.Center - Main.screenPosition + new Vector2(19f, 1f), new Rectangle(0, frameHeight * projectile.frame, standTexture.Width, frameHeight), lightColor, 0f, new Vector2(standTexture.Width / 2f, frameHeight / 2f), 1f, effects, 0);
        }

        public virtual void SwitchStatesTo(string animationName)
        {
            if (animationName == "Idle")
            {
                AnimationStates(animationName, 4, 4, true);
            }
            if (animationName == "Grab")
            {
                AnimationStates(animationName, 9, 3, false);
            }
            if (animationName == "Attack")
            {
                AnimationStates(animationName, 8, 4, true);
            }
            if (animationName == "NoClip")
            {
                AnimationStates(animationName, 5, 4, true);
            }
            if (animationName == "WindUp")
            {
                AnimationStates(animationName, 9, 3, false, true, 5, 7);
            }
        }

        public virtual void AnimationStates(string stateName, int frameAmount, int fps, bool loop, bool loopCertainFrames = false, int loopFrameStart = 0, int loopFrameEnd = 0)
        {
            Main.projFrames[projectile.whoAmI] = frameAmount;
            projectile.frameCounter++;
            standTexture = mod.GetTexture("Projectiles/PlayerStands/FollowMe/FollowMe_" + stateName);
            int framesPerSecond = 60 / fps;
            if (projectile.frameCounter >= framesPerSecond)
            {
                projectile.frameCounter = 0;
                projectile.frame += 1;
            }
            if (loopCertainFrames)
            {
                if (projectile.frame >= loopFrameEnd)
                {
                    projectile.frame = loopFrameStart;
                }
            }
            if (projectile.frame >= frameAmount && loop)
            {
                projectile.frame = 0;
            }
            if (projectile.frame >= frameAmount && !loop)
            {
                SwitchStatesTo("Idle");
            }
        }
    }
}