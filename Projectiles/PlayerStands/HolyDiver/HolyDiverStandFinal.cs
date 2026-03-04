using JoJoFanStands.Buffs;
using JoJoFanStands.UI.AbilityWheel.HolyDiver;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles.PlayerStands;
using JoJoStands.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    public class HolyDiverStandFinal : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 60;
        public override int AltDamage => 90;
        public override int PunchTime => 5;
        public override int TierNumber => 4;
        public override Vector2 StandOffset => new Vector2(-2 * 2, 0f);
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;

        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        // -------------------------------------------------------
        // Tuning constants
        // -------------------------------------------------------
        private const int WaterCannonDamage = 75;
        private const int WaterMissileDamage = 55;
        private const int MineDamage = 120; // base; multiplied x3 by ModifyHitNPC in the mine itself
        private const int MinePlaceCooldown = 90;
        private const float CannonSpeed = 18f;
        private const float MissileSpeed = 9f;
        /// <summary>Ticks M2 must be held to trigger missiles instead of beam (Water Cannon mode).</summary>
        private const int MissileChargeThreshold = 30;
        private const int WaterCannonCooldown = 45;
        /// <summary>How long (ticks) the beam fires after charge completes.</summary>
        private const int BeamDuration = 90;
        /// <summary>How often (ticks) the beam spawns a new projectile while active.</summary>
        private const int BeamFireRate = 5;
        /// <summary>Total missiles fired per salvo.</summary>
        private const int MissileSalvoCount = 5;
        /// <summary>Ticks between each missile in a salvo (~0.1 s at 60 FPS = 6 ticks).</summary>
        private const int MissileSalvoInterval = 6;
        /// <summary>Ticks the beam must charge before firing (1 second = 60 ticks).</summary>
        private const int BeamChargeTime = 60;

        // -------------------------------------------------------
        // Timers
        // -------------------------------------------------------
        private int mineCooldownTimer = 0;
        private int waterCannonCooldownTimer = 0;
        private int m2HoldTimer = 0;       // counts up while M2 held in Water Cannon mode
        private int beamTimer = 0;
        private int beamFireRateTimer = 0;
        private int punchAnimationTimer = 0;
        private int beamChargeTimer = 0;   // counts up while charging beam
        private bool beamCharged = false;  // true once charge threshold is met
        /// <summary>Prevents Special toggling every frame — must release between presses.</summary>
        private bool specialKeyWasHeld = false;
        private int salvoRemaining = 0;   // missiles left to fire in current salvo
        private int salvoTimer = 0;       // countdown between salvo shots
        private int[] salvoTargets = null; // cached targets for the ongoing salvo
        private Vector2 salvoDirection;    // cached aim direction

        private int MaxMissileTargets => TierNumber >= 4 ? 3 : TierNumber >= 3 ? 2 : 1;

        // -------------------------------------------------------
        // M2 mode
        // -------------------------------------------------------

        public enum M2Mode { Mine, WaterCannon }
        public M2Mode currentM2Mode = M2Mode.Mine;

        // -------------------------------------------------------
        // Animation
        // -------------------------------------------------------

        public new enum AnimationState
        {
            Idle,
            Attack,         // M1 - Scorching Torrent Barrage
            KnifeThrow,     // Water Cannon charge / fire animation
            Secondary,      // M2 ability animation
            HydroSymbiosis, // Special install form
            Pose
        }


        // -------------------------------------------------------
        // Spawn / Kill
        // -------------------------------------------------------

        public override void ExtraSpawnEffects()
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            // TODO: Show Liquid Gauge UI
        }

        public override void StandKillEffects()
        {
            // TODO: Hide Liquid Gauge UI
            HolyDiverAbilityWheel.CloseAbilityWheel();
        }


        // -------------------------------------------------------
        // Main AI
        // -------------------------------------------------------

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            SelectAnimation();
            UpdateStandInfo();

            TickTimers();

            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    HandleM1();
                    HandleM2(player);
                    HandleSpecialToggle();
                    HandleBeamTick(player);
                    TickSalvo(player);
                    SpawnBeamChargeParticles(player);
                }

                HandleIdleState();
            }
            else
            {
                BasicPunchAI();
            }

            LimitDistance();

            if (mPlayer.posing)
                currentAnimationState = AnimationState.Pose;
        }


        // -------------------------------------------------------
        // Timer helpers
        // -------------------------------------------------------

        private void TickTimers()
        {
            if (shootCount > 0) shootCount--;
            if (waterCannonCooldownTimer > 0) waterCannonCooldownTimer--;
            if (mineCooldownTimer > 0) mineCooldownTimer--;
            if (beamFireRateTimer > 0) beamFireRateTimer--;
        }

        private bool CanFireCannon => waterCannonCooldownTimer <= 0;
        private bool CanPlaceMine => mineCooldownTimer <= 0;
        private bool BeamIsActive => beamTimer > 0;
        private bool IsChargingBeam => beamChargeTimer > 0 && !beamCharged;


        // -------------------------------------------------------
        // Input handlers
        // -------------------------------------------------------

        /// <summary>M1 - Scorching Torrent Barrage punch.</summary>
        private void HandleM1()
        {
            if (Main.mouseLeft)
            {
                attacking = true;
                Punch();
                currentAnimationState = AnimationState.Idle;
                Projectile.netUpdate = true;
            }
            else
            {
                attacking = false;
            }
        }

        /// <summary>
        /// M2 - behaviour depends on the active mode:
        ///   Mine mode       : tap/hold places a mine (on cooldown).
        ///   Water Cannon    : tap triggers a 1-second Kamehameha charge then fires beam;
        ///                     hold past MissileChargeThreshold = fire missiles instead.
        /// </summary>
        private void HandleM2(Player player)
        {
            if (!Main.mouseRight)
            {
                secondaryAbility = false;

                // Key released while we have been holding (missile charge range) — cancel
                if (currentM2Mode == M2Mode.WaterCannon)
                {
                    if (m2HoldTimer > 0 && m2HoldTimer < MissileChargeThreshold)
                    {
                        // Tap released — begin beam charge if not already charging/active
                        if (!beamCharged && !BeamIsActive && CanFireCannon)
                        {
                            beamChargeTimer = 1; // kick off charge (increments next frame)
                        }
                        m2HoldTimer = 0;
                    }

                    // Complete a charged beam when the charge timer finishes naturally
                    if (beamCharged && !BeamIsActive && CanFireCannon)
                    {
                        StartBeam();
                        beamCharged = false;
                    }
                }

                return;
            }

            secondaryAbility = true;
            StayBehindWithAbility();

            switch (currentM2Mode)
            {
                case M2Mode.Mine:
                    currentAnimationState = AnimationState.Secondary;
                    if (CanPlaceMine)
                        DoMine(player);
                    break;

                case M2Mode.WaterCannon:
                    // If already charging beam or beam is active, don't interrupt
                    if (beamCharged || BeamIsActive)
                        break;

                    if (!CanFireCannon)
                        break;

                    m2HoldTimer++;

                    if (m2HoldTimer >= MissileChargeThreshold)
                    {
                        // Cancel any pending beam charge and fire missiles instead
                        beamChargeTimer = 0;
                        beamCharged = false;
                        DoMissiles(player);
                    }
                    else
                    {
                        currentAnimationState = AnimationState.Idle;
                    }
                    break;
            }
        }

        /// <summary>
        /// Advances the beam charge timer while M2 is not held.
        /// Once BeamChargeTime ticks have elapsed, marks the beam as ready to fire.
        /// Called every frame when the owner is local.
        /// </summary>
        private void SpawnBeamChargeParticles(Player player)
        {
            if (beamChargeTimer <= 0 || beamCharged)
                return;

            beamChargeTimer++;
            currentAnimationState = AnimationState.Idle;

            // Particle burst — orbiting water / energy rings around the stand
            float chargeProgress = (float)beamChargeTimer / BeamChargeTime;
            int particleCount = (int)MathHelper.Lerp(1f, 4f, chargeProgress);

            for (int i = 0; i < particleCount; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float radius = MathHelper.Lerp(20f, 60f, chargeProgress);
                Vector2 offset = new Vector2(radius, 0f).RotatedBy(angle);
                Vector2 vel = new Vector2(-offset.Y, offset.X) * 0.05f; // tangential orbit

                // Blue water orbs
                int d = Dust.NewDust(Projectile.Center + offset, 1, 1, DustID.Water,
                    vel.X, vel.Y, 0, Color.DeepSkyBlue, MathHelper.Lerp(1f, 2.5f, chargeProgress));
                Main.dust[d].noGravity = true;

                // Occasional bright spark
                if (Main.rand.NextBool(3))
                {
                    int spark = Dust.NewDust(Projectile.Center + offset * 0.5f, 1, 1, DustID.Electric,
                        vel.X * 2f, vel.Y * 2f, 0, Color.White, 1.2f);
                    Main.dust[spark].noGravity = true;
                }
            }

            // Sound cue at the halfway point
            if (beamChargeTimer == BeamChargeTime / 2)
                SoundEngine.PlaySound(SoundID.Item20, player.Center);

            // Charge complete
            if (beamChargeTimer >= BeamChargeTime)
            {
                beamCharged = true;
                beamChargeTimer = 0;
                SoundEngine.PlaySound(SoundID.Item29, player.Center); // "ready" sound
                Projectile.netUpdate = true;
            }
        }

        /// <summary>Special - toggles M2 mode between Mine and Water Cannon.</summary>
        private void HandleSpecialToggle()
        {
            if (SpecialKeyCurrent())
            {
                if (!specialKeyWasHeld)
                {
                    currentM2Mode = currentM2Mode == M2Mode.Mine ? M2Mode.WaterCannon : M2Mode.Mine;
                    specialKeyWasHeld = true;

                    // Reset any in-progress charge when switching modes
                    beamChargeTimer = 0;
                    beamCharged = false;
                    m2HoldTimer = 0;

                    Projectile.netUpdate = true;
                }
            }
            else
            {
                specialKeyWasHeld = false;
            }
        }

        /// <summary>Resets to idle when no inputs are active.</summary>
        private void HandleIdleState()
        {
            if (!attacking && !BeamIsActive && m2HoldTimer == 0 && !IsChargingBeam && !beamCharged)
            {
                StayBehind();
                if (!secondaryAbility)
                    currentAnimationState = AnimationState.Idle;
                punchAnimationTimer = 0;
            }
        }


        // -------------------------------------------------------
        // Ability: Mine
        // -------------------------------------------------------

        private void DoMine(Player player)
        {
            PlaceMine(player);
            mineCooldownTimer = MinePlaceCooldown;
            Projectile.netUpdate = true;
        }

        private void PlaceMine(Player player)
        {
            int proj = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<HolyDiverMine>(),
                MineDamage,
                2f,
                Projectile.owner);
            Main.projectile[proj].netUpdate = true;
            SoundEngine.PlaySound(SoundID.Item4, player.Center);
        }


        // -------------------------------------------------------
        // Ability: Beam
        // -------------------------------------------------------

        private void StartBeam()
        {
            beamTimer = BeamDuration;
            beamFireRateTimer = 0;
            m2HoldTimer = 0;
            waterCannonCooldownTimer = WaterCannonCooldown + BeamDuration;
            Projectile.netUpdate = true;
        }

        private void HandleBeamTick(Player player)
        {
            if (!BeamIsActive)
                return;

            beamTimer--;
            currentAnimationState = AnimationState.Idle;

            if (beamFireRateTimer <= 0)
            {
                FireBeamShot(player);
                beamFireRateTimer = BeamFireRate;
            }

            if (!BeamIsActive) // just finished this tick
                currentAnimationState = AnimationState.Idle;

            Projectile.netUpdate = true;
        }

        private void FireBeamShot(Player player)
        {
            Vector2 toMouse = Main.MouseWorld - Projectile.Center;
            if (toMouse == Vector2.Zero)
                toMouse = Vector2.UnitX;
            toMouse.Normalize();
            toMouse *= CannonSpeed;

            int proj = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                toMouse,
                ModContent.ProjectileType<HolyDiverWaterCannon>(),
                WaterCannonDamage,
                4f,
                Projectile.owner);
            Main.projectile[proj].netUpdate = true;

            SoundEngine.PlaySound(SoundID.Splash, player.Center);
        }


        // -------------------------------------------------------
        // Ability: Missiles
        // -------------------------------------------------------

        private void DoMissiles(Player player)
        {
            salvoTargets = FindMissileTargets(player);
            salvoDirection = Main.MouseWorld - Projectile.Center;
            salvoRemaining = MissileSalvoCount;
            salvoTimer = 0;
            waterCannonCooldownTimer = WaterCannonCooldown;
            m2HoldTimer = 0;
            Projectile.netUpdate = true;
        }

        private void TickSalvo(Player player)
        {
            if (salvoRemaining <= 0)
                return;

            if (salvoTimer > 0)
            {
                salvoTimer--;
                return;
            }
            int targetIndex = (MissileSalvoCount - salvoRemaining) % (salvoTargets?.Length > 0 ? salvoTargets.Length : 1);
            int targetId = salvoTargets != null && salvoTargets.Length > 0 ? salvoTargets[targetIndex] : -1;

            int shotIndex = MissileSalvoCount - salvoRemaining;
            float spreadAngle = MathHelper.ToRadians((shotIndex - (MissileSalvoCount - 1) / 2f) * 12f);
            Vector2 launchDir = Vector2.UnitX.RotatedBy(salvoDirection.ToRotation() + spreadAngle) * MissileSpeed;

            int proj = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                launchDir,
                ModContent.ProjectileType<HolyDiverWaterMissile>(),
                WaterMissileDamage,
                3f,
                Projectile.owner,
                ai0: targetId);
            Main.projectile[proj].netUpdate = true;

            SoundEngine.PlaySound(SoundID.Item17, player.Center);

            salvoRemaining--;
            salvoTimer = MissileSalvoInterval;
            Projectile.netUpdate = true;
        }

        private void FireWaterMissiles(Player player)
        {
            int[] targets = FindMissileTargets(player);

            for (int i = 0; i < targets.Length; i++)
            {
                float spreadAngle = MathHelper.ToRadians((i - (targets.Length - 1) / 2f) * 12f);
                Vector2 launchDir = Vector2.UnitX.RotatedBy(
                    (Main.MouseWorld - Projectile.Center).ToRotation() + spreadAngle) * MissileSpeed;

                int proj = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    launchDir,
                    ModContent.ProjectileType<HolyDiverWaterMissile>(),
                    WaterMissileDamage,
                    3f,
                    Projectile.owner,
                    ai0: targets[i]);
                Main.projectile[proj].netUpdate = true;
            }

            SoundEngine.PlaySound(SoundID.Item17, player.Center);
        }

        private int[] FindMissileTargets(Player player)
        {
            const float ScanRange = 1200f;
            int cap = MaxMissileTargets;

            int[] ids = new int[cap];
            float[] dists = new float[cap];
            int found = 0;

            for (int i = 0; i < Main.maxNPCs && found < cap; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5 || npc.dontTakeDamage)
                    continue;
                float d = Vector2.Distance(player.Center, npc.Center);
                if (d > ScanRange)
                    continue;

                ids[found] = npc.whoAmI;
                dists[found] = d;
                found++;
            }

            int[] result = new int[found];
            for (int i = 0; i < found; i++)
                result[i] = ids[i];

            // Sort by distance (insertion sort - tiny array)
            for (int i = 1; i < result.Length; i++)
            {
                int k = result[i];
                float kd = dists[i];
                int j = i - 1;
                while (j >= 0 && dists[j] > kd)
                {
                    result[j + 1] = result[j];
                    dists[j + 1] = dists[j];
                    j--;
                }
                result[j + 1] = k;
                dists[j + 1] = kd;
            }

            // No targets found - fire one self-homing missile
            if (result.Length == 0)
                return new int[] { -1 };

            return result;
        }


        // -------------------------------------------------------
        // Misc overrides
        // -------------------------------------------------------

        public override bool PreKill(int timeLeft)
        {
            HolyDiverAbilityWheel.CloseAbilityWheel();
            return true;
        }

        public override byte SendAnimationState() => (byte)currentAnimationState;
        public override void ReceiveAnimationState(byte state) => currentAnimationState = (AnimationState)state;

        public override void SelectAnimation()
        {
            if (oldAnimationState != currentAnimationState)
            {
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
                oldAnimationState = currentAnimationState;
                Projectile.netUpdate = true;
            }

            if (currentAnimationState == AnimationState.Idle)
                PlayAnimation("Idle");
            else if (currentAnimationState == AnimationState.Attack)
                PlayAnimation("Attack");
            else if (currentAnimationState == AnimationState.Secondary)
                PlayAnimation("Secondary");
            else if (currentAnimationState == AnimationState.HydroSymbiosis)
                PlayAnimation("HydroSymbiosis");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/HolyDiver/HolyDiver_" + animationName).Value;

            if (animationName == "Idle")
                AnimateStand(animationName, 4, 8, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, PunchTime / 2, true);
            else if (animationName == "Secondary")
                AnimateStand(animationName, 2, 10, true);
            else if (animationName == "WaterCannon")
                AnimateStand(animationName, 3, 6, true);
            else if (animationName == "HydroSymbiosis")
                AnimateStand(animationName, 4, 8, true);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 600, true);
        }


        // -------------------------------------------------------
        // Networking
        // -------------------------------------------------------

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(secondaryAbility);
            writer.Write((byte)currentM2Mode);
            writer.Write(waterCannonCooldownTimer);
            writer.Write(beamTimer);
            writer.Write(m2HoldTimer);
            writer.Write(mineCooldownTimer);
            writer.Write(salvoRemaining);
            writer.Write(salvoTimer);
            writer.Write(beamChargeTimer);
            writer.Write(beamCharged);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            secondaryAbility = reader.ReadBoolean();
            currentM2Mode = (M2Mode)reader.ReadByte();
            waterCannonCooldownTimer = reader.ReadInt32();
            beamTimer = reader.ReadInt32();
            m2HoldTimer = reader.ReadInt32();
            mineCooldownTimer = reader.ReadInt32();
            salvoRemaining = reader.ReadInt32();
            salvoTimer = reader.ReadInt32();
            beamChargeTimer = reader.ReadInt32();
            beamCharged = reader.ReadBoolean();
        }


        // -------------------------------------------------------
        // Draw
        // -------------------------------------------------------

        public override bool PreDrawExtras()
        {
            return false;
        }

        public override void PostDrawExtras()
        {
            // TODO: Draw front-layer effects (e.g. current M2 mode indicator)
        }
    }
}