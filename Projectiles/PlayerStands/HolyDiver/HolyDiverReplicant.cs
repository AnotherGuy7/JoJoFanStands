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
    /// <summary>
    /// Water Replicant — a defensive sentry copy of Holy Diver.
    ///
    /// ai[0] : phase  (0 = sentry, 1 = warp-consumed / dying)
    /// ai[1] : unused / reserved
    ///
    /// Behaviour:
    ///   • Stands still at the placement spot.
    ///   • If a hostile NPC enters SentryRange it fires HolyDiverWaterCannon bolts.
    ///   • If a hostile NPC enters MeleeRange it rapidly punches with HolyDiverMeleeHit.
    ///   • Only one Replicant can exist per player at a time (enforced on spawn by the stand).
    ///   • When the stand owner activates the ability again the Replicant teleports the
    ///     player to its position, plays a warp effect, then self-destructs.
    /// </summary>
    public class HolyDiverWaterReplicant : ModProjectile
    {
        // -------------------------------------------------------
        // Tuning
        // -------------------------------------------------------
        private const float SentryRange = 700f;
        private const float MeleeRange = 120f;
        private const int ShotDamage = 55;
        private const int MeleeDamage = 40;
        private const float ShotSpeed = 16f;
        private const int ShotCooldown = 30;   // ticks between ranged shots
        private const int MeleeCooldown = 8;    // ticks between punches
        private const int Lifetime = 1800; // 30 s max

        // -------------------------------------------------------
        // Local state
        // -------------------------------------------------------
        private int shotTimer = 0;
        private int meleeTimer = 0;

        // -------------------------------------------------------
        // Setup
        // -------------------------------------------------------

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
            // No damage from the sentry body itself
            Projectile.damage = 0;
        }

        // -------------------------------------------------------
        // AI
        // -------------------------------------------------------

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            // Consumed by warp — brief flash then die
            if (Projectile.ai[0] == 1f)
            {
                SpawnWarpParticles();
                if (Projectile.timeLeft > 10)
                    Projectile.timeLeft = 10;
                return;
            }

            // Keep velocity at zero — we are a sentry, not a flying projectile
            Projectile.velocity = Vector2.Zero;

            // Tick cooldowns
            if (shotTimer > 0) shotTimer--;
            if (meleeTimer > 0) meleeTimer--;

            // Face toward nearest enemy (or owner if none)
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
                // No enemy — face the owner
                Projectile.spriteDirection = owner.Center.X > Projectile.Center.X ? 1 : -1;
            }

            // Ambient water particle breath
            if (Main.rand.NextBool(12))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Water, Main.rand.NextFloat(-0.5f, 0.5f), -1f, 100, Color.CornflowerBlue, 0.8f);
                Main.dust[d].noGravity = true;
            }
        }

        // -------------------------------------------------------
        // Target finding
        // -------------------------------------------------------

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

        // -------------------------------------------------------
        // Ranged attack
        // -------------------------------------------------------

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
            Projectile.netUpdate = true;
        }

        // -------------------------------------------------------
        // Melee attack — spawns a short-lived hitbox projectile
        // -------------------------------------------------------

        private void DoPunch(NPC target)
        {
            // Direction toward the target for the punch hitbox
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
            Projectile.netUpdate = true;
        }

        // -------------------------------------------------------
        // Warp consumption
        // -------------------------------------------------------

        /// <summary>Called by the stand when the owner wants to warp to this Replicant.</summary>
        public void ConsumeAsWarp()
        {
            Player owner = Main.player[Projectile.owner];

            // Teleport the player here
            owner.Teleport(Projectile.Center - new Vector2(owner.width / 2f, owner.height / 2f),
                           TeleportationStyleID.DebugTeleport);

            // Enter warp-death phase
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

        // -------------------------------------------------------
        // Networking
        // -------------------------------------------------------

        public override void SendExtraAI(System.IO.BinaryWriter writer)
        {
            writer.Write(shotTimer);
            writer.Write(meleeTimer);
        }

        public override void ReceiveExtraAI(System.IO.BinaryReader reader)
        {
            shotTimer = reader.ReadInt32();
            meleeTimer = reader.ReadInt32();
        }

        // -------------------------------------------------------
        // Drawing  — mirrors the stand sprite with a blue tint
        // -------------------------------------------------------

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(
                "JoJoFanStands/Projectiles/PlayerStands/HolyDiver/HolyDiver_Idle").Value;

            // Full sheet frame count assumed 4; pick current idle frame
            int frameHeight = tex.Height / 4;
            int frame = (int)(Main.GameUpdateCount / 8 % 4);
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