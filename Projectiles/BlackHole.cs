using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using JoJoStands;
using JoJoFanStands.Projectiles.PlayerStands.BackInBlack;

namespace JoJoFanStands.Projectiles
{
    public class BlackHole : ModProjectile
    {
        private float vacuumStrength = 0f;

        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 1500;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        private Texture2D blackHoleTexture;
        private Rectangle blackHoleSourceRect;
        private Vector2 blackHoleOrigin;

        public override void PostDraw(Color lightColor)
        {
            if (blackHoleTexture == null)
            {
                blackHoleTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/BlackHole", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                blackHoleSourceRect = new Rectangle(0, 0, blackHoleTexture.Width, blackHoleTexture.Height);
                blackHoleOrigin = new Vector2(blackHoleSourceRect.Width / 2f, blackHoleSourceRect.Height / 2f);
            }

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(blackHoleTexture, drawPosition, blackHoleSourceRect, Color.White, 0f, blackHoleOrigin, Projectile.scale, SpriteEffects.None, 0);
        }


        public override void AI()
        {
            if (Projectile.scale <= 0.01f)
                Projectile.Kill();

            vacuumStrength = Projectile.scale * 17f;
            if (Projectile.scale >= 0.8f)
                Projectile.scale = 0.8f;

            Player player = Main.player[Projectile.owner];
            if (!player.GetModPlayer<MyPlayer>().timestopActive)
            {
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && !npc.dontTakeDamage && !npc.immortal && npc.velocity != Vector2.Zero)
                    {
                        Vector2 positionDifference = Projectile.Center - npc.Center;
                        positionDifference.Normalize();
                        npc.velocity = positionDifference * vacuumStrength;

                        if (Collision.CheckAABBvAABBCollision(npc.position, npc.Size, Projectile.position, new Vector2(Projectile.width, Projectile.height)))
                            npc.StrikeInstantKill();
                    }
                }
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile otherProj = Main.projectile[p];
                    if (otherProj.active && otherProj.whoAmI != Projectile.whoAmI && otherProj.type != ModContent.ProjectileType<BackInBlackStand>())
                    {
                        if (otherProj.active && otherProj.velocity != Vector2.Zero)
                        {
                            Vector2 positionDifference = Projectile.Center - Projectile.Center;
                            positionDifference.Normalize();
                            Projectile.velocity = positionDifference * vacuumStrength;
                        }
                        if (Collision.CheckAABBvAABBCollision(otherProj.position, otherProj.Size, Projectile.position, Projectile.Size))
                        {
                            otherProj.Kill();
                        }
                        if (otherProj.type == Projectile.type && JoJoStands.JoJoStands.SecretReferences)      //now this is where things get fun
                        {
                            if (otherProj.whoAmI > Projectile.whoAmI)       //so that just 1 black hole spawns the boss
                                NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.position.X, (int)Projectile.position.Y, NPCID.MoonLordCore);
                        }
                    }
                }
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    for (int p = 0; p < Main.maxPlayers; p++)
                    {
                        Player otherPlayer = Main.player[p];
                        if (otherPlayer.active && !otherPlayer.dead && otherPlayer.team != player.team && otherPlayer.whoAmI != player.whoAmI)
                        {
                            Vector2 vacuumVelocity = Projectile.Center - otherPlayer.Center;
                            vacuumVelocity.Normalize();
                            vacuumVelocity *= vacuumStrength;
                            otherPlayer.velocity += vacuumVelocity;

                            if (Collision.CheckAABBvAABBCollision(otherPlayer.position, otherPlayer.Size, Projectile.position, new Vector2(Projectile.width, Projectile.height)))
                                otherPlayer.KillMe(PlayerDeathReason.ByCustomReason(otherPlayer.name + " has been consumed by " + player.name + "'s black hole."), 9999999999, otherPlayer.direction);
                        }
                    }
                }
                for (int i = 0; i < Main.maxItems; i++)
                {
                    Item droppedItem = Main.item[i];
                    if (droppedItem.active && droppedItem.velocity != Vector2.Zero)
                    {
                        Vector2 positionDifference = Projectile.Center - droppedItem.Center;
                        positionDifference.Normalize();
                        droppedItem.velocity = positionDifference * vacuumStrength;

                        if (Collision.CheckAABBvAABBCollision(droppedItem.position, droppedItem.Size, Projectile.position, new Vector2(Projectile.width, Projectile.height)))
                            droppedItem.TurnToAir();
                    }

                }
            }
        }
    }
}