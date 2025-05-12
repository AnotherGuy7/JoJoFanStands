using JoJoFanStands.Buffs;
using JoJoStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.GreenDevilDir
{
    public class GoopProjectile : ModProjectile
    {
        public static Texture2D stuckOnSurfaceSpritesheet;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        private bool stuckToTile = false;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 30 * 60;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.damage = 48;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;

                if ((stuckToTile && Projectile.frame >= 3) || (!stuckToTile && Projectile.frame >= 2))
                    Projectile.frame = 0;
            }

            if (!stuckToTile)
            {
                Projectile.velocity.Y += 0.1f;
                if (Projectile.velocity.Y >= 5f)
                    Projectile.velocity.Y = 5f;

                for (int d = 0; d < 12; d++)
                    Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenBlood)].noGravity = true;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            }
            else
            {
                for (int d = 0; d < 3; d++)
                    Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenBlood)].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            if (!stuckToTile)
            {
                target.AddBuff(ModContent.BuffType<GreenDevilGoop>(), 30 * 60);
                Projectile.Kill();
            }
            else
            {
                if (mPlayer.standTier >= 3 && Main.rand.Next(0, 100 + 1) < 5 * (mPlayer.standTier - 2))
                    player.Heal(damageDone / 2);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            stuckToTile = true;
            Projectile.velocity = Vector2.Zero;
            Projectile.frame = 0;
            bool foundOpenTile = false;
            for (int x = 0; x < 6; x++)
            {
                int xSearch = (x / 2) * (x % 2 == 0 ? 1 : -1);      //osicllates between center
                for (int y = 0; y < 6; y++)
                {
                    int ySearch = (y / 2) * (y % 2 == 0 ? 1 : -1);      //osicllates between center
                    Tile tileTarget = Main.tile[(int)(Projectile.position.X / 16f) + xSearch, (int)(Projectile.position.Y / 16f) + ySearch];
                    if (tileTarget.HasTile)
                        continue;

                    for (int i = 0; i < 4; i++)
                    {
                        int Xadd = 0;
                        int Yadd = 0;
                        if (i == 0)
                            Xadd = 1;
                        else if (i == 1)
                            Xadd = -1;
                        if (i == 2)
                            Yadd = -1;
                        else if (i == 3)
                            Yadd = 1;

                        tileTarget = Main.tile[(int)(Projectile.position.X / 16f) + xSearch + Xadd, (int)(Projectile.position.Y / 16f) + ySearch + Yadd];
                        if (tileTarget.HasTile)
                        {
                            if (Xadd == 1)
                                Projectile.rotation = 3.14f * 3f / 2f;
                            else if (Xadd == -1)
                                Projectile.rotation = 3.14f / 2f;

                            if (Yadd == -1)
                                Projectile.rotation = 3.14f;
                            else if (Yadd == 1)
                                Projectile.rotation = 0;

                            foundOpenTile = true;
                            Projectile.position = new Vector2((int)(Projectile.position.X / 16f) + xSearch, (int)(Projectile.position.Y / 16f) + ySearch) * 16;     //proj pos snap
                            Projectile.netUpdate = true;
                            break;
                        }
                    }
                }

                if (foundOpenTile)
                    break;
            }
            if (!foundOpenTile)
                Projectile.Kill();

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(stuckToTile);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            stuckToTile = reader.ReadBoolean();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (stuckToTile)
                Main.EntitySpriteDraw(stuckOnSurfaceSpritesheet, Projectile.Center - Main.screenPosition, new Rectangle(0, 16 * Projectile.frame, 16, 16), lightColor, Projectile.rotation, new Vector2(13f), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return !stuckToTile;
        }
    }
}