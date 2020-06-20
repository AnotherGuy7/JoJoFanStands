using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;

namespace JoJoFanStands.Projectiles.PlayerStands.MortalReminder
{
    public class MortalReminderStandT1 : ModProjectile      //has 2 poses
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
        Mod JoJoStands = null;
        public int afterImageTimer = 60;

        public Texture2D standTexture;

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                shootCount--;
                Player player = Main.player[projectile.owner];
                MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();
                projectile.frameCounter++;
                if (player.HeldItem.type == mod.ItemType("MortalReminderT1") && modPlayer.StandOut)
                {
                    projectile.timeLeft = 2;
                }
                if (player.HeldItem.type != mod.ItemType("MortalReminderT1") || !modPlayer.StandOut || player.dead)
                {
                    projectile.active = false;
                }
                if (JoJoStands == null)
                {
                    JoJoStands = ModLoader.GetMod("JoJoStands");
                }
                drawOriginOffsetY = -halfStandHeight;
                if (Main.mouseLeft)
                {
                    SwitchStatesTo("Attack");
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
                    if (shootCount <= 0)
                    {
                        shootCount += 12;
                        Vector2 shootVel = Main.MouseWorld - projectile.Center;
                        if (shootVel == Vector2.Zero)
                        {
                            shootVel = new Vector2(0f, 1f);
                        }
                        shootVel.Normalize();
                        shootVel *= 1f;
                        int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, JoJoStands.ProjectileType("Fists"), (int)(punchDamage * modPlayer.standDamageBoosts), 2f, Main.myPlayer, 1f);
                        Main.projectile[proj].netUpdate = true;
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    Vector2 vector131 = player.Center;
                    vector131.X -= (float)((36 + player.width / 2) * player.direction);
                    vector131.Y -= -35f + halfStandHeight;
                    projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
                    projectile.velocity *= 0.8f;
                    projectile.direction = (projectile.spriteDirection = player.direction);
                    projectile.rotation = 0;
                    SwitchStatesTo("Idle");
                }
                afterImageTimer--;
                if (afterImageTimer <= 0)
                {
                    int afterImage = Projectile.NewProjectile(projectile.position, Vector2.Zero, mod.ProjectileType(Name), 0, 0f, Main.myPlayer, 1f, projectile.ai[1]);
                    Main.projectile[afterImage].timeLeft = 300;
                    Main.projectile[afterImage].frame = projectile.frame;
                    Main.projectile[afterImage].spriteDirection = projectile.spriteDirection;
                    Main.projFrames[afterImage] = Main.projFrames[projectile.whoAmI];
                    afterImageTimer = 60;
                }
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
                if (shootCount <= 0)
                {
                    shootCount = 0;
                }
            }
            else
            {
                projectile.alpha = 125 + (int)((float)projectile.timeLeft / 2.4f);
                if (projectile.ai[1] == 1f)
                {
                    standTexture = mod.GetTexture("Projectiles/PlayerStands/MortalReminder/MortalReminderT1_Idle");
                }
                if (projectile.ai[1] == 2f)
                {
                    standTexture = mod.GetTexture("Projectiles/PlayerStands/MortalReminder/MortalReminderT1_Dash");
                }
                if (projectile.ai[1] == 3f)
                {
                    SwitchStatesTo("Attack");
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
            if (projectile.ai[0] == 0f)
            {
                if (MyPlayer.RangeIndicators)
                {
                    Texture2D texture = JoJoStands.GetTexture("Extras/RangeIndicator");        //the initial tile amount the indicator covers is 20 tiles, 320 pixels, border is included in the measurements
                    spriteBatch.Draw(texture, player.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), maxDistance / 122.5f, SpriteEffects.None, 0);
                }
                int frameHeight = standTexture.Height / Main.projFrames[projectile.whoAmI];
                spriteBatch.Draw(standTexture, projectile.Center - Main.screenPosition + new Vector2(19f, 1f), new Rectangle(0, frameHeight * projectile.frame, standTexture.Width, frameHeight), Color.White, 0f, new Vector2(standTexture.Width / 2f, frameHeight / 2f), 1f, effects, 0);
            }

            if (projectile.ai[0] == 1f)
            {
                int frameHeight = standTexture.Height / Main.projFrames[projectile.whoAmI];
                spriteBatch.Draw(standTexture, projectile.Center - Main.screenPosition + new Vector2(19f, 1f), new Rectangle(0, frameHeight * projectile.frame, standTexture.Width, frameHeight), Color.White * (((float)projectile.alpha * 3.9215f) / 1000f), 0f, new Vector2(standTexture.Width / 2f, frameHeight / 2f), 1f, effects, 0);
            }
        }

        public virtual void SwitchStatesTo(string animationName)
        {
            if (animationName == "Idle")
            {
                AnimationStates("Idle", 4, 4, true);
                projectile.ai[1] = 1f;
            }
            if (animationName == "Dash")
            {
                AnimationStates(animationName, 4, 4, true);
                projectile.ai[1] = 2f;
            }
            if (animationName == "Attack")
            {
                AnimationStates(animationName, 12, 3, true);
                projectile.ai[1] = 3f;
            }
        }

        public virtual void AnimationStates(string stateName, int frameAmount, int fps, bool loop)
        {
            Main.projFrames[projectile.whoAmI] = frameAmount;
            projectile.frameCounter++;
            standTexture = mod.GetTexture("Projectiles/PlayerStands/MortalReminder/MortalReminderT1_" + stateName);
            int framesPerSecond = 60 / fps;
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
                stateName = "Idle";
            }
        }
    }
}