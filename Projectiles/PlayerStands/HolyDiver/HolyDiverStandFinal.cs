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
        private const int MineDamage = 120;
        private const int MinePlaceCooldown = 90;
        private const float CannonSpeed = 18f;
        private const float MissileSpeed = 9f;
        private const int MissileChargeThreshold = 30;
        private const int WaterCannonCooldown = 45;
        private const int BeamDuration = 90;
        private const int BeamFireRate = 5;
        private const int MissileSalvoCount = 5;
        private const int MissileSalvoInterval = 6;
        private const int BeamChargeTime = 60;
        private const int ReplicantCooldown = 180; // 3 s after warp/place before a new one can be placed

        // -------------------------------------------------------
        // Timers
        // -------------------------------------------------------
        private int mineCooldownTimer = 0;
        private int waterCannonCooldownTimer = 0;
        private int m2HoldTimer = 0;
        private int beamTimer = 0;
        private int beamFireRateTimer = 0;
        private int punchAnimationTimer = 0;
        private int beamChargeTimer = 0;
        private bool beamCharged = false;
        private bool specialKeyWasHeld = false;
        private int salvoRemaining = 0;
        private int salvoTimer = 0;
        private int[] salvoTargets = null;
        private Vector2 salvoDirection;

        // -------------------------------------------------------
        // Replicant tracking
        // -------------------------------------------------------
        /// <summary>whoAmI of the active HolyDiverWaterReplicant projectile, or -1 if none.</summary>
        private int replicantProjIndex = -1;
        private int replicantCooldown = 0;

        private bool HasActiveReplicant => replicantProjIndex >= 0
            && replicantProjIndex < Main.maxProjectiles
            && Main.projectile[replicantProjIndex].active
            && Main.projectile[replicantProjIndex].type == ModContent.ProjectileType<HolyDiverWaterReplicant>();

        private int MaxMissileTargets => TierNumber >= 4 ? 3 : TierNumber >= 3 ? 2 : 1;

        // -------------------------------------------------------
        // M2 mode  (now three-way cycle)
        // -------------------------------------------------------

        public enum M2Mode { Mine, WaterCannon, WaterReplicant }
        public M2Mode currentM2Mode = M2Mode.Mine;

        // -------------------------------------------------------
        // Animation
        // -------------------------------------------------------

        public new enum AnimationState
        {
            Idle,
            Attack,
            KnifeThrow,
            Secondary,
            HydroSymbiosis,
            Pose
        }


        // -------------------------------------------------------
        // Spawn / Kill
        // -------------------------------------------------------

        public override void ExtraSpawnEffects()
        {
            if (Projectile.owner != Main.myPlayer) return;
        }

        public override void StandKillEffects()
        {
            if (HasActiveReplicant)
            {
                Main.projectile[replicantProjIndex].Kill();
                replicantProjIndex = -1;
            }
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
            if (replicantCooldown > 0) replicantCooldown--;
        }

        private bool CanFireCannon => waterCannonCooldownTimer <= 0;
        private bool CanPlaceMine => mineCooldownTimer <= 0;
        private bool BeamIsActive => beamTimer > 0;
        private bool IsChargingBeam => beamChargeTimer > 0 && !beamCharged;
        private bool CanUseReplicant => replicantCooldown <= 0;


        // -------------------------------------------------------
        // Input handlers
        // -------------------------------------------------------

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

        private void HandleM2(Player player)
        {
            if (!Main.mouseRight)
            {
                secondaryAbility = false;

                if (currentM2Mode == M2Mode.WaterCannon)
                {
                    if (m2HoldTimer > 0 && m2HoldTimer < MissileChargeThreshold)
                    {
                        if (!beamCharged && !BeamIsActive && CanFireCannon)
                            beamChargeTimer = 1;
                        m2HoldTimer = 0;
                    }

                    if (beamCharged && !BeamIsActive && CanFireCannon)
                    {
                        StartBeam();
                        beamCharged = false;
                    }
                }

                return;
            }

            secondaryAbility = true;

            switch (currentM2Mode)
            {
                // ---- Mine ----
                case M2Mode.Mine:
                    currentAnimationState = AnimationState.Secondary;
                    if (CanPlaceMine) DoMine(player);
                    break;

                // ---- Water Cannon ----
                case M2Mode.WaterCannon:
                    if (beamCharged || BeamIsActive) break;
                    if (!CanFireCannon) break;

                    m2HoldTimer++;
                    if (m2HoldTimer >= MissileChargeThreshold)
                    {
                        beamChargeTimer = 0;
                        beamCharged = false;
                        DoMissiles(player);
                    }
                    else
                    {
                        currentAnimationState = AnimationState.Idle;
                    }
                    break;

                // ---- Water Replicant ----
                case M2Mode.WaterReplicant:
                    HandleReplicantM2(player);
                    break;
            }
        }

        // -------------------------------------------------------
        // Water Replicant M2 logic
        // -------------------------------------------------------

        /// <summary>
        /// Single tap M2 in Replicant mode:
        ///   • If NO replicant exists → place one at the stand's current position.
        ///   • If a replicant EXISTS → warp the player to it and destroy it.
        /// </summary>
        private void HandleReplicantM2(Player player)
        {
            // Only respond on the frame the key is first pressed (tap detection via m2HoldTimer)
            m2HoldTimer++;
            if (m2HoldTimer != 1)
                return; // wait for a fresh tap

            if (!HasActiveReplicant)
            {
                if (!CanUseReplicant)
                    return;
                PlaceReplicant(player);
            }
            else
            {
                WarpToReplicant(player);
            }
        }

        private void PlaceReplicant(Player player)
        {
            int proj = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<HolyDiverWaterReplicant>(),
                0,
                0f,
                Projectile.owner);

            replicantProjIndex = proj;
            replicantCooldown = ReplicantCooldown;
            Main.projectile[proj].netUpdate = true;

            // Placement burst
            for (int i = 0; i < 20; i++)
            {
                float a = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 dustVel = new Vector2(3f, 0f).RotatedBy(a);
                int d = Dust.NewDust(Projectile.Center, 4, 4, DustID.Water,
                    dustVel.X, dustVel.Y, 100, Color.DeepSkyBlue, 1.4f);
                Main.dust[d].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item6, player.Center);
            currentAnimationState = AnimationState.Idle;
            Projectile.netUpdate = true;
        }

        private void WarpToReplicant(Player player)
        {
            HolyDiverWaterReplicant replicant =
                Main.projectile[replicantProjIndex].ModProjectile as HolyDiverWaterReplicant;

            if (replicant != null)
                replicant.ConsumeAsWarp();

            replicantProjIndex = -1;
            replicantCooldown = ReplicantCooldown;

            // Warp flash particles on the PLAYER side
            for (int i = 0; i < 20; i++)
            {
                float a = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 dustVel = new Vector2(3f, 0f).RotatedBy(a);
                int d = Dust.NewDust(Projectile.Center, 4, 4, DustID.Water,
                    dustVel.X, dustVel.Y, 0, Color.White, 1.6f);
                Main.dust[d].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item6, player.Center);
            Projectile.netUpdate = true;
        }


        // -------------------------------------------------------
        // Beam charge particles
        // -------------------------------------------------------

        private void SpawnBeamChargeParticles(Player player)
        {
            if (beamChargeTimer <= 0 || beamCharged) return;

            beamChargeTimer++;
            currentAnimationState = AnimationState.Idle;

            float chargeProgress = (float)beamChargeTimer / BeamChargeTime;
            int particleCount = (int)MathHelper.Lerp(1f, 4f, chargeProgress);

            for (int i = 0; i < particleCount; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float radius = MathHelper.Lerp(20f, 60f, chargeProgress);
                Vector2 offset = new Vector2(radius, 0f).RotatedBy(angle);
                Vector2 vel = new Vector2(-offset.Y, offset.X) * 0.05f;

                int d = Dust.NewDust(Projectile.Center + offset, 1, 1, DustID.Water,
                    vel.X, vel.Y, 0, Color.DeepSkyBlue, MathHelper.Lerp(1f, 2.5f, chargeProgress));
                Main.dust[d].noGravity = true;

                if (Main.rand.NextBool(3))
                {
                    int spark = Dust.NewDust(Projectile.Center + offset * 0.5f, 1, 1, DustID.Electric,
                        vel.X * 2f, vel.Y * 2f, 0, Color.White, 1.2f);
                    Main.dust[spark].noGravity = true;
                }
            }

            if (beamChargeTimer == BeamChargeTime / 2)
                SoundEngine.PlaySound(SoundID.Item20, player.Center);

            if (beamChargeTimer >= BeamChargeTime)
            {
                beamCharged = true;
                beamChargeTimer = 0;
                SoundEngine.PlaySound(SoundID.Item29, player.Center);
                Projectile.netUpdate = true;
            }
        }

        // -------------------------------------------------------
        // Special key — cycle through all three modes
        // -------------------------------------------------------

        private void HandleSpecialToggle()
        {
            if (SpecialKeyCurrent())
            {
                if (!specialKeyWasHeld)
                {
                    // Cycle: Mine → WaterCannon → WaterReplicant → Mine …
                    currentM2Mode = currentM2Mode switch
                    {
                        M2Mode.Mine => M2Mode.WaterCannon,
                        M2Mode.WaterCannon => M2Mode.WaterReplicant,
                        M2Mode.WaterReplicant => M2Mode.Mine,
                        _ => M2Mode.Mine
                    };

                    specialKeyWasHeld = true;

                    // Reset in-progress states from other modes
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
            if (!BeamIsActive) return;

            beamTimer--;
            currentAnimationState = AnimationState.Idle;

            if (beamFireRateTimer <= 0)
            {
                FireBeamShot(player);
                beamFireRateTimer = BeamFireRate;
            }

            if (!BeamIsActive)
                currentAnimationState = AnimationState.Idle;

            Projectile.netUpdate = true;
        }

        private void FireBeamShot(Player player)
        {
            Vector2 toMouse = Main.MouseWorld - Projectile.Center;
            if (toMouse == Vector2.Zero) toMouse = Vector2.UnitX;
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
            if (salvoRemaining <= 0) return;

            if (salvoTimer > 0) { salvoTimer--; return; }

            int targetIndex = (MissileSalvoCount - salvoRemaining) % (salvoTargets?.Length > 0 ? salvoTargets.Length : 1);
            int targetId = salvoTargets != null && salvoTargets.Length > 0 ? salvoTargets[targetIndex] : -1;

            int shotIndex = MissileSalvoCount - salvoRemaining;
            float spread = MathHelper.ToRadians((shotIndex - (MissileSalvoCount - 1) / 2f) * 12f);
            Vector2 launchDir = Vector2.UnitX.RotatedBy(salvoDirection.ToRotation() + spread) * MissileSpeed;

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
                if (!npc.active || npc.friendly || npc.lifeMax <= 5 || npc.dontTakeDamage) continue;
                float d = Vector2.Distance(player.Center, npc.Center);
                if (d > ScanRange) continue;
                ids[found] = npc.whoAmI;
                dists[found] = d;
                found++;
            }

            int[] result = new int[found];
            for (int i = 0; i < found; i++) result[i] = ids[i];

            for (int i = 1; i < result.Length; i++)
            {
                int k = result[i]; float kd = dists[i]; int j = i - 1;
                while (j >= 0 && dists[j] > kd) { result[j + 1] = result[j]; dists[j + 1] = dists[j]; j--; }
                result[j + 1] = k; dists[j + 1] = kd;
            }

            return result.Length == 0 ? new int[] { -1 } : result;
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

            if (currentAnimationState == AnimationState.Idle) PlayAnimation("Idle");
            else if (currentAnimationState == AnimationState.Attack) PlayAnimation("Attack");
            else if (currentAnimationState == AnimationState.Secondary) PlayAnimation("Secondary");
            else if (currentAnimationState == AnimationState.HydroSymbiosis) PlayAnimation("HydroSymbiosis");
            else if (currentAnimationState == AnimationState.Pose) PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/HolyDiver/HolyDiver_" + animationName).Value;

            if (animationName == "Idle") AnimateStand(animationName, 4, 8, true);
            else if (animationName == "Attack") AnimateStand(animationName, 4, PunchTime / 2, true);
            else if (animationName == "Secondary") AnimateStand(animationName, 2, 10, true);
            else if (animationName == "WaterCannon") AnimateStand(animationName, 3, 6, true);
            else if (animationName == "HydroSymbiosis") AnimateStand(animationName, 4, 8, true);
            else if (animationName == "Pose") AnimateStand(animationName, 1, 600, true);
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
            writer.Write(replicantProjIndex);
            writer.Write(replicantCooldown);
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
            replicantProjIndex = reader.ReadInt32();
            replicantCooldown = reader.ReadInt32();
        }


        // -------------------------------------------------------
        // Draw
        // -------------------------------------------------------

        public override bool PreDrawExtras() => false;

        public override void PostDrawExtras()
        {
            // TODO: Draw current M2 mode indicator (Mine / Water Cannon / Water Replicant)
        }
    }
}