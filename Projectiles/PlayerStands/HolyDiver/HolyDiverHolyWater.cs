using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    public class HolyDiverHolyWater : ModProjectile
    {
        private const int BeamLifetime = 40;
        private const int BurnDuration = 180;
        private float Alpha => MathHelper.Clamp((float)Projectile.timeLeft / BeamLifetime, 0f, 1f);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = BeamLifetime;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            // Manual gravity arc
            Projectile.velocity.Y += 0.18f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 vel = Projectile.velocity;
            Vector2 perp = new Vector2(-vel.Y, vel.X);
            if (perp != Vector2.Zero) perp.Normalize();

            // Core holy water stream
            for (int i = 0; i < 3; i++)
            {
                Vector2 offset = perp * Main.rand.NextFloat(-6f, 6f);
                int d = Dust.NewDust(
                    Projectile.Center + offset - vel * Main.rand.NextFloat(0.2f, 0.8f),
                    1, 1, DustID.Water,
                    vel.X * 0.2f, vel.Y * 0.2f,
                    80, Color.Aquamarine, Main.rand.NextFloat(1.2f, 2.0f));
                Main.dust[d].noGravity = true;
            }

            // Side ribbons
            for (int side = -1; side <= 1; side += 2)
            {
                int r = Dust.NewDust(
                    Projectile.Center + perp * side * Main.rand.NextFloat(4f, 10f),
                    1, 1, DustID.Water,
                    vel.X * 0.1f + perp.X * side * Main.rand.NextFloat(0.3f, 1.0f),
                    vel.Y * 0.1f + perp.Y * side * Main.rand.NextFloat(0.3f, 1.0f),
                    100, Color.LightCyan, Main.rand.NextFloat(0.8f, 1.4f));
                Main.dust[r].noGravity = true;
            }

            // Holy glow
            if (Main.rand.NextBool(3))
            {
                int g = Dust.NewDust(Projectile.Center - new Vector2(5f), 10, 10,
                    DustID.BlueFairy, 0f, 0f, 0, Color.White, Main.rand.NextFloat(1.2f, 2.2f));
                Main.dust[g].noGravity = true;
                Main.dust[g].velocity = Vector2.Zero;
            }

            // Gold shimmer
            if (Main.rand.NextBool(4))
            {
                int gold = Dust.NewDust(
                    Projectile.Center + Main.rand.NextVector2Circular(6f, 6f),
                    1, 1, DustID.GoldFlame,
                    vel.X * 0.05f, vel.Y * 0.05f,
                    120, Color.Gold, Main.rand.NextFloat(0.5f, 1.0f));
                Main.dust[gold].noGravity = true;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || !npc.friendly || npc.lifeMax <= 5) continue;
                    if (!Projectile.Hitbox.Intersects(npc.Hitbox)) continue;

                    npc.life = System.Math.Min(npc.life + 3, npc.lifeMax);
                    npc.HealEffect(3);
                    npc.AddBuff(BuffID.Ironskin, 300);

                    for (int j = 0; j < 5; j++)
                    {
                        int h = Dust.NewDust(npc.Center, npc.width, npc.height,
                            DustID.GoldFlame,
                            Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f),
                            0, Color.Gold, Main.rand.NextFloat(0.8f, 1.4f));
                        Main.dust[h].noGravity = true;
                    }

                    SoundEngine.PlaySound(SoundID.Item4, npc.Center);
                    Projectile.Kill();
                    break;
                }
            }
        }

        // Enemies: damage + burn , Friendly NPCs: heal + ironskin, don't deal damage
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, BurnDuration);

            for (int i = 0; i < 8; i++)
            {
                int s = Dust.NewDust(target.Center, target.width, target.height,
                    DustID.Water,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f),
                    0, Color.Aqua, Main.rand.NextFloat(1.2f, 2.0f));
                Main.dust[s].noGravity = true;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.friendly && target.lifeMax > 5)
                return false;
            return null;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.friendly)
                modifiers.FinalDamage.Base -= modifiers.FinalDamage.Base;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Water,
                    Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f),
                    0, Color.LightCyan, Main.rand.NextFloat(1.2f, 2.0f));
                Main.dust[d].noGravity = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() / 2f;
            Vector2 pos = Projectile.Center - Main.screenPosition;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float t = 1f - i / (float)Projectile.oldPos.Length;
                Color trail = Color.Lerp(Color.Aquamarine, Color.Gold, 1f - t) * t * Alpha * 0.7f;
                Vector2 tp = Projectile.oldPos[i] - Main.screenPosition
                                + new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                float scale = Projectile.scale * MathHelper.Lerp(1.8f, 0.3f, i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(tex, tp, null, trail, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            }

            // Glow
            Main.EntitySpriteDraw(tex, pos, null, Color.White * Alpha * 0.45f,
                Projectile.rotation, origin, Projectile.scale * 2.0f, SpriteEffects.None, 0);

            // Core — aquamarine/gold blend instead of blue/orange
            Main.EntitySpriteDraw(tex, pos, null,
                Color.Lerp(Color.White, Color.Aquamarine, 0.4f) * Alpha,
                Projectile.rotation, origin, Projectile.scale * 1.1f, SpriteEffects.None, 0);

            return false;
        }
    }
}