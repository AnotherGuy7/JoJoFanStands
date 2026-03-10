using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    public class HolyDiverWaterReplicant : ModProjectile
    {
        private const float SentryRange = 700f;
        private const float MeleeRange = 120f;
        private const int ShotDamage = 55;
        private const int MeleeDamage = 40;
        private const float ShotSpeed = 16f;
        private const int ShotCooldown = 30;
        private const int MeleeCooldown = 8;
        private const int Lifetime = 1800;

        private const int PunchFrameCount = 8;
        private const int ShotFrameCount = 1;
        private const int IdleFrameCount = 7;
        private const int AnimTicksPerFrame = 4;

        private int shotTimer = 0;
        private int meleeTimer = 0;

        private string drawMode = "idle";
        private int animFrame = 0;
        private int animTick = 0;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.damage = 0;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (Projectile.ai[0] == 1f)
            {
                SpawnWarpParticles();
                if (Projectile.timeLeft > 10)
                    Projectile.timeLeft = 10;
                return;
            }

            Projectile.velocity = Vector2.Zero;

            if (shotTimer > 0) shotTimer--;
            if (meleeTimer > 0) meleeTimer--;

            if (drawMode != "idle")
            {
                animTick++;
                if (animTick >= AnimTicksPerFrame)
                {
                    animTick = 0;
                    animFrame++;
                    int maxFrames = drawMode == "punch" ? PunchFrameCount : ShotFrameCount;
                    if (animFrame >= maxFrames)
                    {
                        animFrame = 0;
                        drawMode = "idle";
                    }
                }
            }

            NPC target = FindTarget();
            if (target != null)
            {
                Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? 1 : -1;

                float dist = Vector2.Distance(Projectile.Center, target.Center);

                if (dist <= MeleeRange && meleeTimer <= 0 && Projectile.owner == Main.myPlayer)
                {
                    DoPunch(target);
                }
                else if (dist <= SentryRange && shotTimer <= 0 && Projectile.owner == Main.myPlayer)
                {
                    DoShot(target);
                }
            }
            else
            {
                Projectile.spriteDirection = owner.Center.X > Projectile.Center.X ? 1 : -1;
            }

            if (Main.rand.NextBool(12))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Water, Main.rand.NextFloat(-0.5f, 0.5f), -1f, 100, Color.CornflowerBlue, 0.8f);
                Main.dust[d].noGravity = true;
            }
        }

        private NPC FindTarget()
        {
            NPC best = null;
            float bestD = SentryRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5 || npc.dontTakeDamage)
                    continue;

                float d = Vector2.Distance(Projectile.Center, npc.Center);
                if (d < bestD)
                {
                    bestD = d;
                    best = npc;
                }
            }
            return best;
        }

        private void DoShot(NPC target)
        {
            Vector2 dir = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * ShotSpeed;

            int proj = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                dir,
                ModContent.ProjectileType<HolyDiverWaterCannon>(),
                ShotDamage,
                3f,
                Projectile.owner);
            Main.projectile[proj].netUpdate = true;

            SoundEngine.PlaySound(SoundID.Splash, Projectile.Center);
            shotTimer = ShotCooldown;

            if (drawMode != "punch")
            {
                drawMode = "shot";
                animFrame = 0;
                animTick = 0;
            }

            Projectile.netUpdate = true;
        }

        private void DoPunch(NPC target)
        {
            Vector2 dir = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 6f;

            int proj = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                dir,
                ModContent.ProjectileType<HolyDiverReplicantPunch>(),
                MeleeDamage,
                5f,
                Projectile.owner);
            Main.projectile[proj].netUpdate = true;

            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            meleeTimer = MeleeCooldown;

            drawMode = "punch";
            animFrame = 0;
            animTick = 0;

            Projectile.netUpdate = true;
        }

        public void ConsumeAsWarp()
        {
            Player owner = Main.player[Projectile.owner];
            owner.Teleport(Projectile.Center - new Vector2(owner.width / 2f, owner.height / 2f),
                           TeleportationStyleID.DebugTeleport);
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item6, Projectile.Center);
        }

        private void SpawnWarpParticles()
        {
            for (int i = 0; i < 4; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float radius = Main.rand.NextFloat(10f, 50f);
                Vector2 off = new Vector2(radius, 0f).RotatedBy(angle);

                int d = Dust.NewDust(Projectile.Center + off, 1, 1, DustID.Water,
                    off.X * 0.05f, off.Y * 0.05f, 0, Color.DeepSkyBlue, 1.8f);
                Main.dust[d].noGravity = true;

                int spark = Dust.NewDust(Projectile.Center, 1, 1, DustID.Electric,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, Color.White, 1.2f);
                Main.dust[spark].noGravity = true;
            }
        }

        public override void SendExtraAI(System.IO.BinaryWriter writer)
        {
            writer.Write(shotTimer);
            writer.Write(meleeTimer);
            writer.Write(animFrame);
            writer.Write(animTick);
            // Encode drawMode as byte: 0=idle, 1=punch, 2=shot
            byte modeId = drawMode == "punch" ? (byte)1 : drawMode == "shot" ? (byte)2 : (byte)0;
            writer.Write(modeId);
        }

        public override void ReceiveExtraAI(System.IO.BinaryReader reader)
        {
            shotTimer = reader.ReadInt32();
            meleeTimer = reader.ReadInt32();
            animFrame = reader.ReadInt32();
            animTick = reader.ReadInt32();
            byte modeId = reader.ReadByte();
            drawMode = modeId == 1 ? "punch" : modeId == 2 ? "shot" : "idle";
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string texPath;
            int frameCount;

            switch (drawMode)
            {
                case "punch":
                    texPath = "JoJoFanStands/Projectiles/PlayerStands/HolyDiver/HolyDiver_Punch";
                    frameCount = PunchFrameCount;
                    break;
                case "shot":
                    texPath = "JoJoFanStands/Projectiles/PlayerStands/HolyDiver/HolyDiver_CannonShot";
                    frameCount = ShotFrameCount;
                    break;
                default:
                    texPath = "JoJoFanStands/Projectiles/PlayerStands/HolyDiver/HolyDiver_Idle";
                    frameCount = IdleFrameCount;
                    break;
            }

            Texture2D tex = ModContent.Request<Texture2D>(texPath).Value;
            int frameHeight = tex.Height / frameCount;

            int frame = drawMode == "idle"
                ? (int)(Main.GameUpdateCount / 8 % IdleFrameCount)
                : animFrame;

            frame = Math.Clamp(frame, 0, frameCount - 1);
            Rectangle src = new Rectangle(0, frame * frameHeight, tex.Width, frameHeight);

            Color tint = Color.Lerp(lightColor, Color.DeepSkyBlue, 0.55f) * 0.85f;
            SpriteEffects flip = Projectile.spriteDirection == -1
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            Vector2 origin = new Vector2(tex.Width / 2f, frameHeight / 2f);
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(tex, pos, src, tint, 0f, origin, Projectile.scale, flip, 0);
            return false;
        }
    }
}