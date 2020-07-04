using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using JoJoStands;

namespace JoJoFanStands.Projectiles
{
    public class BlackHole : ModProjectile
    {
        private float vacuumStrength = 0f;

        public override void SetDefaults()
        {
            projectile.width = 250;
            projectile.height = 250;
            projectile.aiStyle = 0;
            projectile.timeLeft = 1500;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = mod.GetTexture("Projectiles/BlackHole");
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0);
        }


        public override void AI()
        {
            if (projectile.scale <= 0.01f)
            {
                projectile.Kill();
            }
            vacuumStrength = projectile.scale * 17f;
            if (projectile.scale >= 0.8f)
            {
                projectile.scale = 0.8f;
            }

            Player player = Main.player[projectile.owner];
            if (!player.GetModPlayer<MyPlayer>().TheWorldEffect)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.active && !npc.dontTakeDamage && !npc.immortal && npc.velocity != Vector2.Zero)
                    {
                        /*if (npc.position.X < projectile.position.X)
                        {
                            npc.velocity.X += vacuumStrength;
                        }
                        if (npc.position.X > projectile.position.X)
                        {
                            npc.velocity.X -= vacuumStrength;
                        }
                        if (npc.position.Y < projectile.position.Y)
                        {
                            npc.velocity.Y += vacuumStrength;
                        }
                        if (npc.position.Y > projectile.position.Y)
                        {
                            npc.velocity.Y -= vacuumStrength;
                        }*/
                        Vector2 positionDifference = projectile.Center - npc.Center;
                        positionDifference.Normalize();
                        npc.velocity = positionDifference * vacuumStrength;
                    }
                    if (Collision.CheckAABBvAABBCollision(npc.position, new Vector2(npc.width, npc.height), projectile.position, new Vector2(projectile.width, projectile.height)))
                    {
                        npc.StrikeNPC(npc.lifeMax + 50, 90f, 1);
                    }
                }
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    for (int l = 0; l < Main.maxPlayers; l++)
                    {
                        Player otherPlayer = Main.player[l];
                        if (otherPlayer.active && !otherPlayer.dead && otherPlayer.team != player.team && otherPlayer.whoAmI != player.whoAmI)
                        {
                            if (otherPlayer.position.X < projectile.position.X)
                            {
                                otherPlayer.velocity.X += vacuumStrength;
                            }
                            if (otherPlayer.position.X > projectile.position.X)
                            {
                                otherPlayer.velocity.X -= vacuumStrength;
                            }
                            if (otherPlayer.position.Y < projectile.position.Y)
                            {
                                otherPlayer.velocity.Y += vacuumStrength;
                            }
                            if (otherPlayer.position.Y > projectile.position.Y)
                            {
                                otherPlayer.velocity.Y -= vacuumStrength;
                            }
                        }
                        if (Collision.CheckAABBvAABBCollision(otherPlayer.position, new Vector2(otherPlayer.width, otherPlayer.height), projectile.position, new Vector2(projectile.width, projectile.height)))
                        {
                            otherPlayer.KillMe(PlayerDeathReason.ByCustomReason(otherPlayer.name + " has been consumed by " + player.name + "'s black hole."), 9999999999, otherPlayer.direction);
                        }
                    }
                }
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile otherProj = Main.projectile[p];
                    if (otherProj.type != mod.ProjectileType("BackInBlackStand") && otherProj.whoAmI != projectile.whoAmI)
                    {
                        if (otherProj.active && otherProj.velocity != Vector2.Zero)
                        {
                            /*if (otherProj.position.X < projectile.position.X)
                            {
                                otherProj.velocity.X += vacuumStrength;
                            }
                            if (otherProj.position.X > projectile.position.X)
                            {
                                otherProj.velocity.X -= vacuumStrength;
                            }
                            if (otherProj.position.Y < projectile.position.Y)
                            {
                                otherProj.velocity.Y += vacuumStrength;
                            }
                            if (otherProj.position.Y > projectile.position.Y)
                            {
                                otherProj.velocity.Y -= vacuumStrength;
                            }*/
                            Vector2 positionDifference = projectile.Center - projectile.Center;
                            positionDifference.Normalize();
                            projectile.velocity = positionDifference * vacuumStrength;
                        }
                        if (Collision.CheckAABBvAABBCollision(otherProj.position, new Vector2(otherProj.width, otherProj.height), projectile.position, new Vector2(projectile.width, projectile.height)))
                        {
                            otherProj.Kill();
                        }
                        if (otherProj.type == projectile.type && MyPlayer.SecretReferences)      //now this is where things get fun
                        {
                            if (otherProj.whoAmI > projectile.whoAmI)       //so that just 1 black hole spawns the boss
                            {
                                NPC.NewNPC((int)projectile.position.X, (int)projectile.position.Y, NPCID.MoonLordCore);
                            }
                        }
                    }
                }
                for (int v = 0; v < Main.maxItems; v++)
                {
                    Item droppedItem = Main.item[v];
                    if (droppedItem.active && droppedItem.velocity != Vector2.Zero)
                    {
                        Vector2 positionDifference = projectile.Center - droppedItem.Center;
                        positionDifference.Normalize();
                        droppedItem.velocity = positionDifference * vacuumStrength;
                    }
                    if (Collision.CheckAABBvAABBCollision(droppedItem.position, new Vector2(droppedItem.width, droppedItem.height), projectile.position, new Vector2(projectile.width, projectile.height)))
                    {
                        droppedItem.TurnToAir();
                    }
                }
            }
        }
    }
}