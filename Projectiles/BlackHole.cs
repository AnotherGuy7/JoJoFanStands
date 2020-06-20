using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace JoJoFanStands.Projectiles
{
    public class BlackHole : ModProjectile
    {
        public static float vacuumStrength = 0f;
        public static int whoAmI = 0;

        public override void SetDefaults()
        {
            projectile.width = 300;
            projectile.height = 300;
            projectile.aiStyle = 0;
            projectile.timeLeft = 1500;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 0;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = mod.GetTexture("Projectiles/BlackHole");
            Player player = Main.player[projectile.owner];
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0);
        }


        public override void AI()
        {
            if (projectile.scale <= 0.01)
            {
                projectile.Kill();
            }
            whoAmI = projectile.whoAmI;
            vacuumStrength = projectile.scale * 5f;
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC npc = Main.npc[k];
                if (npc.active && !npc.dontTakeDamage && !npc.immortal && !npc.townNPC && npc.velocity != Vector2.Zero)
                {
                    if (npc.position.X < projectile.position.X)
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
                    }
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
                    if (otherPlayer.active && !otherPlayer.dead && otherPlayer.team != Main.player[projectile.owner].team && otherPlayer.whoAmI != Main.player[projectile.owner].whoAmI)
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
                        otherPlayer.KillMe(PlayerDeathReason.ByCustomReason(otherPlayer.name + " has been consumed by " + Main.player[projectile.owner].name + "'s black hole."), 9999999999, otherPlayer.direction);
                    }
                }
            }
            for (int p = 0; p < Main.maxProjectiles; p++)
            {
                Projectile otherProj = Main.projectile[p];
                if (otherProj.active && otherProj.velocity != Vector2.Zero && otherProj.type != mod.ProjectileType("BackInBlackStand"))
                {
                    if (otherProj.position.X < projectile.position.X)
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
                    }
                }
                if (Collision.CheckAABBvAABBCollision(otherProj.position, new Vector2(otherProj.width, otherProj.height), projectile.position, new Vector2(projectile.width, projectile.height)))
                {
                    otherProj.Kill();
                }
            }
        }
    }
}