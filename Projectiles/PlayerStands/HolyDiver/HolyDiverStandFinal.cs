using JoJoFanStands.UI;
using JoJoFanStands.UI.AbilityWheel.HolyDiver;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    public class HolyDiverStandFinal : StandClass
    {
        public static readonly SoundStyle GlassShatter = new SoundStyle("JoJoFanStands/Sounds/StandLines/GlassShatterSoundEffect")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };
        public override int HalfStandHeight => 50;
        public override int PunchDamage => 56;
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
        private const int ReplicantCooldown = 180;

        private const int JCEDamage = 350;
        private const float JCERadius = 2400f;
        private const int JCESlashCount = 40;
        private const int JCESlashStagger = 1;
        private const int JCEFreezeTime = 40;
        private const int JCEImmunityTime = 90;

        private bool jcePending = false;
        private int jceFreezeTimer = 0;
        private int jceSlashSpawned = 0;
        private int jceSlashTimer = 0;

        // -------------------------------------------------------
        // Symbiosis sword constants
        // -------------------------------------------------------
        private const int SymbiosisSwordDamage = 110;
        private const float SymbiosisChargedDamageMultiplier = 2.2f;
        private const int SymbiosisSwordCooldown = 22;
        private const int SymbiosisChargeThreshold = 45;
        private const int SymbiosisChargedLifetime = 28;
        private const int SymbiosisSwingLifetime = 18;

        // -------------------------------------------------------
        // Symbiosis Iai Slash (M2) constants
        // -------------------------------------------------------
        private const int IaiSlashDamage = 200;
        private const float IaiSlashHalfWidth = 24f;
        private const float IaiSlashMaxRange = 1400f;
        private const int IaiSlashCooldown = 90;
        private const float IaiSlashOvershoot = 80f;

        // -------------------------------------------------------
        // Symbiosis sword state
        // -------------------------------------------------------
        private int symbiosisSwordCooldown = 0;
        private int symbiosisM1HoldTimer = 0;
        private bool symbiosisM1WasHeld = false;
        private bool symbiosisChargeReleased = false;
        private int iaiSlashCooldown = 0;
        private bool symbiosisM2WasHeld = false;

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

        private const int WaterCostBarrage = 2;
        private const int WaterCostBeamShot = 10;
        private const int WaterCostMissiles = 20;
        private const int WaterCostMine = 5;
        private const int WaterCostHolyWater = 1;
        private const int WaterRestoreAbsorb = 25;
        private const int WaterCostSymbiosis = 2;
        private const int WaterPerTileCluster = 1;
        private const int MaxTileClusters = 5;
        private const int WaterAbsorbFromEnemy = 3;
        private const int WaterAbsorbPassive = 1;
        private const int AbsorbEnemyDamage = 12;
        private const int MaxEnemyAbsorb = 3;
        private const int AbsorptionRadius = 30 * 16;
        private const int PassiveAccrualInterval = 90;
        private int passiveAccrualTimer = 0;

        private int holyWaterBeamTimer = 0;
        private int holyWaterFireRateTimer = 0;
        private const int HolyWaterBeamDuration = 90;
        private const int HolyWaterFireRate = 9;
        private const float HolyWaterProjectileSpeed = 9f;
        private const int HolyWaterHoldMax = 120;
        private const int HolyWaterBurnBonusMax = 600;
        private const int HolyWaterHealAmount = 20;
        private const int HolyWaterHealCooldown = 90;
        private const int HolyWaterCooldown = 45;
        private bool holyWaterActive => holyWaterBeamTimer > 0;
        private int holyWaterCooldownTimer = 0;

        // -------------------------------------------------------
        // Replicant tracking
        // -------------------------------------------------------
        private int replicantProjIndex = -1;
        private int replicantCooldown = 0;

        private bool replicantM2WasHeld = false;

        private bool HasActiveReplicant => replicantProjIndex >= 0
            && replicantProjIndex < Main.maxProjectiles
            && Main.projectile[replicantProjIndex].active
            && Main.projectile[replicantProjIndex].type == ModContent.ProjectileType<HolyDiverWaterReplicant>();

        private int MaxMissileTargets => TierNumber >= 4 ? 3 : TierNumber >= 3 ? 2 : 1;

        // -------------------------------------------------------
        // M2 mode
        // -------------------------------------------------------
        public enum M2Mode { WaterReplicant, WaterCannon, Mine, WaterAbsorption, HolyWater }
        public M2Mode currentM2Mode = M2Mode.WaterReplicant;

        // -------------------------------------------------------
        // Animation
        // -------------------------------------------------------
        public new enum AnimationState
        {
            Idle,
            Attack,
            CannonShot,
            HydroSymbiosisIaiSlash,
            HydroSymbiosisIdle,
            HydroSymbiosisSwim,
            HydroSymbiosisSwordAttack,
            HydroSymbiosisSwordCharge,
            Pose
        }


        // -------------------------------------------------------
        // Spawn / Kill
        // -------------------------------------------------------

        public override void ExtraSpawnEffects()
        {
            if (Projectile.owner != Main.myPlayer) return;
            WaterGaugeBar.ShowWaterGaugeBar();
            WaterGaugePlayer.MaxWater = 200;

            Player player = Main.player[Projectile.owner];
            WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();
            if (wgp.CurrentWater > WaterGaugePlayer.MaxWater)
                wgp.SetWater(System.Math.Min(wgp.CurrentWater, WaterGaugePlayer.MaxWater));
            HolyDiverAbilityWheel.OpenAbilityWheel(player.GetModPlayer<MyPlayer>(), 5);
        }

        public override void StandKillEffects()
        {
            if (HasActiveReplicant)
            {
                Main.projectile[replicantProjIndex].Kill();
                replicantProjIndex = -1;
            }
            WaterGaugeBar.HideWaterGaugeBar();
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
            TickHolyWaterBeam(player);
            TickJCE(player);

            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    HandleHydroSymbiosis(player);
                    if (hydroSymbiosisActive)
                    {
                        HandleSymbiosisM1(player);
                        return;
                    }
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
            if (symbiosisSwordCooldown > 0) symbiosisSwordCooldown--;
            if (iaiSlashCooldown > 0) iaiSlashCooldown--;
        }

        private bool CanFireCannon => waterCannonCooldownTimer <= 0;
        private bool CanPlaceMine => mineCooldownTimer <= 0;
        private bool BeamIsActive => beamTimer > 0;
        private bool IsChargingBeam => beamChargeTimer > 0 && !beamCharged;
        private bool CanUseReplicant => replicantCooldown <= 0;

        private void HandleSymbiosisM1(Player player)
        {
            HandleIaiSlash(player);

            bool m1Held = Main.mouseLeft;

            if (m1Held)
            {
                symbiosisM1HoldTimer++;

                if (symbiosisM1HoldTimer >= SymbiosisChargeThreshold)
                    currentAnimationState = AnimationState.HydroSymbiosisSwordCharge;
                else
                    currentAnimationState = AnimationState.HydroSymbiosisSwordCharge;
            }
            else
            {
                if (symbiosisM1WasHeld)
                {
                    bool wasCharged = symbiosisM1HoldTimer >= SymbiosisChargeThreshold;

                    if (symbiosisSwordCooldown <= 0)
                    {
                        FireSymbiosisSword(player, wasCharged);
                        symbiosisSwordCooldown = wasCharged
                            ? SymbiosisSwordCooldown * 2
                            : SymbiosisSwordCooldown;
                    }

                    symbiosisM1HoldTimer = 0;
                    symbiosisM1WasHeld = false;
                    Projectile.netUpdate = true;
                    return;
                }

                symbiosisM1HoldTimer = 0;
            }

            symbiosisM1WasHeld = m1Held;
        }

        private void FireSymbiosisSword(Player player, bool charged)
        {
            Vector2 toMouse = Main.MouseWorld - Projectile.Center;
            if (toMouse == Vector2.Zero) toMouse = Vector2.UnitX;
            float baseAngle = toMouse.ToRotation();

            int damage = charged
                ? (int)(SymbiosisSwordDamage * SymbiosisChargedDamageMultiplier)
                : SymbiosisSwordDamage;

            int proj = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<HolyDiverSymbiosisSword>(),
                damage,
                charged ? 8f : 5f,
                Projectile.owner,
                ai0: charged ? 1f : 0f,
                ai1: baseAngle);

            Main.projectile[proj].timeLeft = charged ? SymbiosisChargedLifetime : SymbiosisSwingLifetime;
            Main.projectile[proj].netUpdate = true;

            if (charged)
                Main.projectile[proj].CritChance = 100;

            SoundEngine.PlaySound(charged ? SoundID.Item71 : SoundID.Item1, player.Center);

            if (charged)
            {
                for (int i = 0; i < 20; i++)
                {
                    float a = Main.rand.NextFloat(MathHelper.TwoPi);
                    Vector2 vel = new Vector2(Main.rand.NextFloat(3f, 7f), 0f).RotatedBy(a);
                    int d = Dust.NewDust(Projectile.Center, 4, 4, DustID.Water,
                        vel.X, vel.Y, 0, Color.OrangeRed, 2.0f);
                    Main.dust[d].noGravity = true;
                }

                for (int i = 0; i < 10; i++)
                {
                    float a = Main.rand.NextFloat(MathHelper.TwoPi);
                    Vector2 vel = new Vector2(Main.rand.NextFloat(2f, 5f), 0f).RotatedBy(a);
                    int sp = Dust.NewDust(Projectile.Center, 2, 2, DustID.Electric,
                        vel.X, vel.Y, 0, Color.White, 1.4f);
                    Main.dust[sp].noGravity = true;
                }
            }
            currentAnimationState = AnimationState.HydroSymbiosisSwordAttack;
            Projectile.netUpdate = true;
        }


        // -------------------------------------------------------
        // Ability: Iai Slash (Symbiosis M2)
        // -------------------------------------------------------
        private void HandleIaiSlash(Player player)
        {
            bool m2Held = Main.mouseRight;

            if (m2Held && !symbiosisM2WasHeld)
            {
                symbiosisM2WasHeld = true;

                if (iaiSlashCooldown <= 0)
                    PerformIaiSlash(player);
            }
            else if (!m2Held)
            {
                symbiosisM2WasHeld = false;
            }
        }

        private void PerformIaiSlash(Player player)
        {
            Vector2 origin = player.Center;
            Vector2 target = Main.MouseWorld;
            Vector2 rawDir = target - origin;
            float rawDist = rawDir.Length();

            if (rawDist < 1f) rawDir = Vector2.UnitX;
            else rawDir /= rawDist;

            float slashLength = System.Math.Min(rawDist, IaiSlashMaxRange);
            Vector2 slashEnd = origin + rawDir * slashLength;

            float wallLimit = FindWallLimit(origin, rawDir, slashLength);
            Vector2 safeEnd = origin + rawDir * wallLimit;

            float furthestHitDist = -1f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5 || npc.dontTakeDamage)
                    continue;

                float t = Vector2.Dot(npc.Center - origin, rawDir);
                if (t < 0f || t > wallLimit) continue;

                Vector2 closest = origin + rawDir * t;
                float perp = Vector2.Distance(npc.Center, closest);

                float hitRadius = IaiSlashHalfWidth + npc.width * 0.4f;
                if (perp > hitRadius) continue;

                if (Projectile.owner == Main.myPlayer)
                {
                    npc.SimpleStrikeNPC(
                        damage: IaiSlashDamage,
                        hitDirection: rawDir.X >= 0f ? 1 : -1,
                        crit: true,
                        noPlayerInteraction: false);

                    for (int k = 0; k < 10; k++)
                    {
                        float a = Main.rand.NextFloat(MathHelper.TwoPi);
                        Vector2 v = new Vector2(Main.rand.NextFloat(3f, 8f), 0f).RotatedBy(a);
                        int d = Dust.NewDust(npc.Center, 4, 4, DustID.Water,
                            v.X, v.Y, 0, Color.OrangeRed, 1.8f);
                        Main.dust[d].noGravity = true;
                    }
                    for (int k = 0; k < 5; k++)
                    {
                        float a = Main.rand.NextFloat(MathHelper.TwoPi);
                        Vector2 v = new Vector2(Main.rand.NextFloat(2f, 5f), 0f).RotatedBy(a);
                        int sp = Dust.NewDust(npc.Center, 2, 2, DustID.Electric,
                            v.X, v.Y, 0, Color.White, 1.2f);
                        Main.dust[sp].noGravity = true;
                    }
                }

                if (t > furthestHitDist)
                    furthestHitDist = t;
            }

            float tpDist;
            if (furthestHitDist >= 0f)
                tpDist = System.Math.Min(furthestHitDist + IaiSlashOvershoot, wallLimit);
            else
                tpDist = wallLimit;

            Vector2 tpPos = origin + rawDir * tpDist;

            if (Projectile.owner == Main.myPlayer)
            {
                player.Center = tpPos;
                Projectile.Center = tpPos;
                player.velocity = rawDir * 6f;
                player.immune = true;
                player.immuneTime = System.Math.Max(player.immuneTime, 25);
                SoundEngine.PlaySound(SoundID.Item71, tpPos);
            }

            int visual = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                safeEnd,
                Vector2.Zero,
                ModContent.ProjectileType<HolyDiverIaiSlashVisual>(),
                0, 0f,
                Projectile.owner,
                ai0: origin.X,
                ai1: origin.Y);
            Main.projectile[visual].netUpdate = true;

            iaiSlashCooldown = IaiSlashCooldown;
            currentAnimationState = AnimationState.HydroSymbiosisIaiSlash;
            Projectile.netUpdate = true;
        }

        private float FindWallLimit(Vector2 origin, Vector2 dir, float maxDist)
        {
            const float StepSize = 8f;
            int steps = (int)(maxDist / StepSize);

            for (int s = 1; s <= steps; s++)
            {
                Vector2 check = origin + dir * (s * StepSize);
                int tx = (int)(check.X / 16f);
                int ty = (int)(check.Y / 16f);

                if (tx < 0 || ty < 0 || tx >= Main.maxTilesX || ty >= Main.maxTilesY)
                    return s * StepSize;

                Tile tile = Main.tile[tx, ty];
                if (tile != null && tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                    return (s - 1) * StepSize;
            }

            return maxDist;
        }

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

        private void Punch()
        {
            Punch(Main.MouseWorld, afterImages: CanUseAfterImagePunches, playPunchSound: false);
            currentAnimationState = AnimationState.Idle;
            if (shootCount == newPunchTime)
            {
                Vector2 toMouse = Main.MouseWorld - Projectile.Center;
                if (toMouse == Vector2.Zero) toMouse = Vector2.UnitX;
                toMouse.Normalize();
                Vector2 burstOrigin = Projectile.Center + toMouse * 36f;
                for (int i = 0; i < 14; i++)
                {
                    float angle = toMouse.ToRotation() + MathHelper.ToRadians(Main.rand.NextFloat(-55f, 55f));
                    float speed = Main.rand.NextFloat(3.5f, 8f);
                    Vector2 vel = new Vector2(speed, 0f).RotatedBy(angle);
                    int d = Dust.NewDust(burstOrigin, 6, 6, DustID.Water,
                        vel.X, vel.Y, 80, Color.DeepSkyBlue, Main.rand.NextFloat(1.2f, 2.0f));
                    Main.dust[d].noGravity = true;
                }
                for (int i = 0; i < 6; i++)
                {
                    float angle = toMouse.ToRotation() + MathHelper.ToRadians(Main.rand.NextFloat(-40f, 40f));
                    float speed = Main.rand.NextFloat(2f, 5f);
                    Vector2 vel = new Vector2(speed, 0f).RotatedBy(angle);
                    int sp = Dust.NewDust(burstOrigin, 4, 4, DustID.Electric,
                        vel.X, vel.Y, 0, Color.White, 1.3f);
                    Main.dust[sp].noGravity = true;
                }
                int dropCount = 8;
                for (int i = 0; i < dropCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / dropCount;
                    Vector2 vel = new Vector2(Main.rand.NextFloat(2.5f, 5f), 0f).RotatedBy(angle);
                    int d = Dust.NewDust(burstOrigin, 2, 2, DustID.Water,
                        vel.X, vel.Y, 120, Color.CornflowerBlue, 1.1f);
                    Main.dust[d].noGravity = true;
                }
            }
        }

        private void HandleM2(Player player)
        {
            if (!Main.mouseRight)
            {
                secondaryAbility = false;

                if (currentM2Mode == M2Mode.WaterCannon)
                {
                    // Only start beam charge on release if we were genuinely charging
                    // (not if missiles just fired and reset m2HoldTimer to 0)
                    if (m2HoldTimer > 0 && m2HoldTimer < MissileChargeThreshold)
                    {
                        if (!beamCharged && !BeamIsActive && CanFireCannon)
                            beamChargeTimer = 1;
                    }

                    m2HoldTimer = 0;

                    if (beamCharged && !BeamIsActive && CanFireCannon)
                    {
                        StartBeam();
                        beamCharged = false;
                    }
                }
                else
                {
                    m2HoldTimer = 0;
                }

                replicantM2WasHeld = false;
                return;
            }

            secondaryAbility = true;

            switch (currentM2Mode)
            {
                case M2Mode.Mine:
                    currentAnimationState = AnimationState.Idle;
                    if (CanPlaceMine) DoMine(player);
                    break;

                case M2Mode.WaterCannon:
                    currentAnimationState = AnimationState.CannonShot;
                    if (beamCharged || BeamIsActive) break;
                    if (!CanFireCannon)
                    {
                        m2HoldTimer = 0;
                        StayBehind();
                        break;
                    }
                    StayBehind();
                    m2HoldTimer++;
                    if (m2HoldTimer >= MissileChargeThreshold)
                        DoMissiles(player);
                    else
                    {
                        currentAnimationState = AnimationState.Idle;
                    }
                    break;

                case M2Mode.WaterReplicant:
                    HandleReplicantM2(player);
                    break;

                case M2Mode.WaterAbsorption:
                    DoWaterAbsorption(player);
                    currentAnimationState = AnimationState.Idle;
                    break;

                case M2Mode.HolyWater:
                    HandleHolyWater(player);
                    break;
            }
        }


        private void HandleReplicantM2(Player player)
        {
            if (replicantM2WasHeld)
                return;

            replicantM2WasHeld = true;

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
                //HolyDiverAbilityWheel.OpenAbilityWheel(player.GetModPlayer<MyPlayer>(), 5);
                if (!specialKeyWasHeld)
                {
                    currentM2Mode = currentM2Mode switch
                    {
                        M2Mode.Mine => M2Mode.WaterCannon,
                        M2Mode.WaterCannon => M2Mode.WaterReplicant,
                        M2Mode.WaterReplicant => M2Mode.WaterAbsorption,
                        M2Mode.WaterAbsorption => M2Mode.HolyWater,
                        M2Mode.HolyWater => M2Mode.Mine,
                        _ => M2Mode.Mine
                    };

                    specialKeyWasHeld = true;

                    beamChargeTimer = 0;
                    beamCharged = false;
                    m2HoldTimer = 0;
                    replicantM2WasHeld = false;

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
            WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();
            if (!wgp.TrySpend(WaterCostMine)) return;
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
            currentAnimationState = AnimationState.CannonShot;

            if (beamFireRateTimer <= 0)
            {
                FireBeamShot(player);
                beamFireRateTimer = BeamFireRate;
            }

            if (!BeamIsActive)
                currentAnimationState = AnimationState.CannonShot;

            Projectile.netUpdate = true;
        }

        private void FireBeamShot(Player player)
        {
            WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();
            if (!wgp.TrySpend(WaterCostBeamShot))
            {
                beamTimer = 0;
                return;
            }

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

        private void DoMissiles(Player player)
        {
            WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();
            if (!wgp.TrySpend(WaterCostMissiles)) return;
            salvoTargets = FindMissileTargets(player);
            salvoDirection = Main.MouseWorld - Projectile.Center;
            salvoRemaining = MissileSalvoCount;
            salvoTimer = 0;
            waterCannonCooldownTimer = WaterCannonCooldown;
            m2HoldTimer = 0;
            beamChargeTimer = 0;
            beamCharged = false;
            Projectile.netUpdate = true;
        }

        private void DoWaterAbsorption(Player player)
        {
            WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();

            if (wgp.IsFull)
            {
                ShowAbsorbParticles(player.Center, Color.LightCyan, 3);
                return;
            }

            int totalRestored = 0;
            bool absorbedFromEnvironment = false;
            bool absorbedFromEnemies = false;

            List<Vector2> waterTilePositions = GetNearbyWaterTilePositions(player);
            if (waterTilePositions.Count > 0)
            {
                int clusters = System.Math.Min(waterTilePositions.Count, MaxTileClusters);
                totalRestored += clusters * WaterPerTileCluster;
                absorbedFromEnvironment = true;

                foreach (Vector2 tileWorldPos in waterTilePositions)
                    ShowAbsorbPullParticlesFrom(tileWorldPos, player.Center);
            }

            int enemiesAbsorbed = AbsorbFromEnemies(player, ref totalRestored);
            absorbedFromEnemies = enemiesAbsorbed > 0;

            if (!absorbedFromEnvironment && !absorbedFromEnemies)
            {
                passiveAccrualTimer++;
                if (passiveAccrualTimer >= PassiveAccrualInterval)
                {
                    passiveAccrualTimer = 0;
                    totalRestored += WaterAbsorbPassive;
                }
            }
            else
            {
                passiveAccrualTimer = 0;
            }

            if (totalRestored > 0)
            {
                wgp.Restore(totalRestored);
                SoundEngine.PlaySound(SoundID.Splash, player.Center);
            }

            currentAnimationState = AnimationState.Idle;
            Projectile.netUpdate = true;
        }

        private List<Vector2> GetNearbyWaterTilePositions(Player player)
        {
            int radiusTiles = AbsorptionRadius / 16;
            int playerTileX = (int)(player.Center.X / 16f);
            int playerTileY = (int)(player.Center.Y / 16f);

            var positions = new List<Vector2>();
            int step = 3;

            for (int tx = playerTileX - radiusTiles; tx <= playerTileX + radiusTiles; tx += step)
            {
                for (int ty = playerTileY - radiusTiles; ty <= playerTileY + radiusTiles; ty += step)
                {
                    if (tx < 0 || ty < 0 || tx >= Main.maxTilesX || ty >= Main.maxTilesY)
                        continue;

                    float distTiles = Vector2.Distance(new Vector2(tx, ty), new Vector2(playerTileX, playerTileY));
                    if (distTiles > radiusTiles) continue;

                    Tile tile = Main.tile[tx, ty];
                    if (tile != null && tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water)
                    {
                        positions.Add(new Vector2(tx * 16f + 8f, ty * 16f + 8f));
                        if (positions.Count >= MaxTileClusters)
                            return positions;
                    }
                }
            }

            return positions;
        }

        private int AbsorbFromEnemies(Player player, ref int totalRestored)
        {
            int hit = 0;

            for (int i = 0; i < Main.maxNPCs && hit < MaxEnemyAbsorb; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5 || npc.dontTakeDamage)
                    continue;

                float dist = Vector2.Distance(player.Center, npc.Center);
                if (dist > AbsorptionRadius) continue;

                if (Projectile.owner == Main.myPlayer)
                {
                    npc.SimpleStrikeNPC(
                        damage: AbsorbEnemyDamage,
                        hitDirection: npc.Center.X > player.Center.X ? 1 : -1,
                        noPlayerInteraction: false);

                    ShowAbsorbPullParticlesFrom(npc.Center, player.Center);
                }

                totalRestored += WaterAbsorbFromEnemy;
                hit++;
            }

            return hit;
        }

        private void HandleHolyWater(Player player)
        {
            if (!holyWaterActive && holyWaterCooldownTimer <= 0)
            {
                holyWaterBeamTimer = HolyWaterBeamDuration;
                holyWaterFireRateTimer = 0;
                Projectile.netUpdate = true;
            }
        }

        private void TickHolyWaterBeam(Player player)
        {
            if (holyWaterCooldownTimer > 0) holyWaterCooldownTimer--;
            if (!holyWaterActive) return;

            if (!Main.mouseRight || currentM2Mode != M2Mode.HolyWater)
            {
                holyWaterBeamTimer = 0;
                holyWaterCooldownTimer = HolyWaterCooldown;
                currentAnimationState = AnimationState.Idle;
                Projectile.netUpdate = true;
                return;
            }

            holyWaterBeamTimer--;
            holyWaterFireRateTimer--;
            currentAnimationState = AnimationState.CannonShot;

            if (holyWaterFireRateTimer <= 0)
            {
                FireHolyWaterShot(player);
                holyWaterFireRateTimer = HolyWaterFireRate;
            }

            if (holyWaterBeamTimer <= 0 && Main.mouseRight)
            {
                holyWaterBeamTimer = HolyWaterBeamDuration;
            }

            Projectile.netUpdate = true;
        }

        private void FireHolyWaterShot(Player player)
        {
            WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();
            if (!wgp.TrySpend(WaterCostHolyWater))
            {
                holyWaterBeamTimer = 0;
                return;
            }

            player.Heal(2);
            player.AddBuff(BuffID.Ironskin, 300);

            Vector2 toMouse = Main.MouseWorld - Projectile.Center;
            if (toMouse == Vector2.Zero) toMouse = Vector2.UnitX;
            toMouse.Normalize();
            toMouse *= HolyWaterProjectileSpeed;

            int proj = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                toMouse,
                ModContent.ProjectileType<HolyDiverHolyWater>(),
                PunchDamage / 2,
                2f,
                Projectile.owner);
            Main.projectile[proj].netUpdate = true;

            SoundEngine.PlaySound(SoundID.Splash, player.Center);
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
            else if (currentAnimationState == AnimationState.HydroSymbiosisIaiSlash) PlayAnimation("HydroSymbiosisIaiSlash");
            else if (currentAnimationState == AnimationState.HydroSymbiosisSwim) PlayAnimation("HydroSymbiosisSwim");
            else if (currentAnimationState == AnimationState.CannonShot) PlayAnimation("CannonShot");
            else if (currentAnimationState == AnimationState.HydroSymbiosisIdle) PlayAnimation("HydroSymbiosisIdle");
            else if (currentAnimationState == AnimationState.HydroSymbiosisSwordAttack) PlayAnimation("HydroSymbiosisSwordAttack");
            else if (currentAnimationState == AnimationState.HydroSymbiosisSwordCharge) PlayAnimation("HydroSymbiosisSwordCharge");
            else if (currentAnimationState == AnimationState.Pose) PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/HolyDiver/HolyDiver_" + animationName).Value;

            if (animationName == "Idle") AnimateStand(animationName, 7, 8, true);
            else if (animationName == "Attack") AnimateStand(animationName, 4, PunchTime / 2, true);
            else if (animationName == "HydroSymbiosisIaiSlash") AnimateStand(animationName, 8, 2, false);
            else if (animationName == "HydroSymbiosisSwim") AnimateStand(animationName, 8, 8, true);
            else if (animationName == "CannonShot") AnimateStand(animationName, 1, 6, true);
            else if (animationName == "HydroSymbiosisIdle") AnimateStand(animationName, 7, 8, true);
            else if (animationName == "HydroSymbiosisSwordAttack") AnimateStand(animationName, 7, 2, false);
            else if (animationName == "HydroSymbiosisSwordCharge") AnimateStand(animationName, 1, 2, false);
            else if (animationName == "Pose") AnimateStand(animationName, 1, 600, true);
        }

        public override void AnimationCompleted(string animationName)
        {
            if (animationName == "HydroSymbiosisIaiSlash" ||
                animationName == "HydroSymbiosisSwordAttack" ||
                animationName == "HydroSymbiosisSwordCharge")
            {
                currentAnimationState = hydroSymbiosisActive
                    ? AnimationState.HydroSymbiosisIdle
                    : AnimationState.Idle;
            }
        }

        private void ShowAbsorbParticles(Vector2 center, Color color, int count)
        {
            for (int i = 0; i < count; i++)
            {
                float a = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 vel = new Vector2(Main.rand.NextFloat(1.5f, 3.5f), 0f).RotatedBy(a);
                int d = Dust.NewDust(center, 4, 4, DustID.Water,
                    vel.X, vel.Y, 100, color, 1.4f);
                Main.dust[d].noGravity = true;
            }
        }

        private void ShowAbsorbPullParticles(Player player)
        {
            for (int i = 0; i < 12; i++)
            {
                float a = Main.rand.NextFloat(MathHelper.TwoPi);
                float radius = Main.rand.NextFloat(32f, 80f);
                Vector2 spawnPos = player.Center + new Vector2(radius, 0f).RotatedBy(a);
                Vector2 vel = (player.Center - spawnPos) * 0.12f;

                int d = Dust.NewDust(spawnPos, 2, 2, DustID.Water,
                    vel.X, vel.Y, 80, Color.CornflowerBlue, 1.2f);
                Main.dust[d].noGravity = true;
            }
        }

        private void ShowAbsorbPullParticlesFrom(Vector2 from, Vector2 to)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 t = (float)i / 6f * (to - from) + from;
                Vector2 vel = (to - from) * 0.08f;
                int d = Dust.NewDust(t, 2, 2, DustID.Water,
                    vel.X, vel.Y, 60, Color.Aquamarine, 1.0f);
                Main.dust[d].noGravity = true;
            }
        }

        #region Hydro Symbiosis

        private const int WaterDrainPerTick = 2;
        private const int HydroSymbiosisDrainInterval = 20;
        private const int HydroSymbiosisBuffTime = 2;

        private bool hydroSymbiosisActive = false;
        private bool secondSpecialWasHeld = false;
        private int hydroDrainTimer = 0;

        private void HandleHydroSymbiosis(Player player)
        {
            bool keyHeld = SecondSpecialKeyCurrent();

            if (keyHeld && !secondSpecialWasHeld)
            {
                secondSpecialWasHeld = true;

                if (!hydroSymbiosisActive)
                    EnterHydroSymbiosis(player);
                else
                    ExitHydroSymbiosis(player, manual: true);

                return;
            }

            if (!keyHeld)
                secondSpecialWasHeld = false;

            if (hydroSymbiosisActive)
                TickHydroSymbiosis(player);
        }

        private void EnterHydroSymbiosis(Player player)
        {
            WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();
            if (wgp.CurrentWater < WaterGaugePlayer.MaxWater * 0.2f) return;

            hydroSymbiosisActive = true;
            hydroDrainTimer = 0;
            symbiosisM1HoldTimer = 0;
            symbiosisM1WasHeld = false;
            symbiosisSwordCooldown = 0;
            symbiosisM2WasHeld = false;
            iaiSlashCooldown = 0;

            player.wingTimeMax = int.MaxValue / 2;

            player.AddBuff(BuffID.Ironskin, HydroSymbiosisBuffTime);
            player.AddBuff(BuffID.Regeneration, HydroSymbiosisBuffTime);
            player.AddBuff(BuffID.Swiftness, HydroSymbiosisBuffTime);
            player.AddBuff(BuffID.Endurance, HydroSymbiosisBuffTime);

            currentAnimationState = AnimationState.Idle;
            Projectile.netUpdate = true;

            SoundEngine.PlaySound(SoundID.Item29, player.Center);
            SpawnInstallParticles(player);
        }

        private void TickHydroSymbiosis(Player player)
        {
            WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            mPlayer.hideAllPlayerLayers = true;

            player.immune = true;
            player.immuneTime = System.Math.Max(player.immuneTime, 3);

            player.position = Projectile.Center - new Vector2(player.width * 0.5f, player.height * 0.5f);

            fPlayer.customCameraOverride = true;
            fPlayer.customCameraPosition = Projectile.Center - new Vector2(
                Main.screenWidth * 0.5f,
                Main.screenHeight * 0.5f);

            player.noFallDmg = true;
            player.gravity = 0f;

            const float FlyMaxSpeed = 14f;
            const float FlyAccel = 0.6f;
            const float FlyFriction = 0.85f;

            if (player.controlJump)
                player.velocity.Y = System.Math.Max(player.velocity.Y - FlyAccel, -FlyMaxSpeed);
            else if (player.controlDown)
                player.velocity.Y = System.Math.Min(player.velocity.Y + FlyAccel, FlyMaxSpeed);
            else
                player.velocity.Y *= FlyFriction;

            player.AddBuff(BuffID.Ironskin, HydroSymbiosisBuffTime);
            player.AddBuff(BuffID.Regeneration, HydroSymbiosisBuffTime);
            player.AddBuff(BuffID.Swiftness, HydroSymbiosisBuffTime);
            player.AddBuff(BuffID.Endurance, HydroSymbiosisBuffTime);

            ClearAllDebuffs(player);

            Projectile.Center = player.Center;
            Projectile.velocity = player.velocity;

            if (currentAnimationState != AnimationState.HydroSymbiosisSwordAttack &&
                currentAnimationState != AnimationState.HydroSymbiosisSwordCharge &&
                currentAnimationState != AnimationState.HydroSymbiosisIaiSlash)
            {
                bool isMoving = player.velocity.LengthSquared() > 0.5f;
                currentAnimationState = isMoving
                    ? AnimationState.HydroSymbiosisSwim
                    : AnimationState.HydroSymbiosisIdle;
            }

            hydroDrainTimer++;
            if (hydroDrainTimer >= HydroSymbiosisDrainInterval)
            {
                hydroDrainTimer = 0;

                if (!wgp.TrySpend(WaterDrainPerTick))
                {
                    ExitHydroSymbiosis(player, manual: false);
                    return;
                }
            }

            if (player.velocity.X != 0f)
                Projectile.spriteDirection = player.velocity.X > 0f ? 1 : -1;
            else
                Projectile.spriteDirection = Main.MouseWorld.X > Projectile.Center.X ? 1 : -1;

            Projectile.direction = Projectile.spriteDirection;

            Projectile.netUpdate = true;
        }

        private void ExitHydroSymbiosis(Player player, bool manual)
        {
            hydroSymbiosisActive = false;
            hydroDrainTimer = 0;
            symbiosisM1HoldTimer = 0;
            symbiosisM1WasHeld = false;
            symbiosisM2WasHeld = false;
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            fPlayer.customCameraOverride = false;
            player.Center = Projectile.Center;
            player.immuneTime = 0;
            player.wingTimeMax = 0;
            if (manual)
            {
                WaterGaugePlayer wgp = player.GetModPlayer<WaterGaugePlayer>();
                int penalty = (int)(WaterGaugePlayer.MaxWater * 0.1f);
                wgp.TrySpend(penalty);
            }
            currentAnimationState = AnimationState.Idle;
            Projectile.netUpdate = true;
            TriggerJCE(player);
        }

        private void TriggerJCE(Player player)
        {
            jcePending = true;
            jceFreezeTimer = JCEFreezeTime;
            jceSlashSpawned = 0;
            jceSlashTimer = 0;

            player.immune = true;
            player.immuneTime = JCEImmunityTime;

            SoundEngine.PlaySound(SoundID.Item71, player.Center);
            SoundEngine.PlaySound(GlassShatter, player.Center);

            Projectile.netUpdate = true;
        }

        private void TickJCE(Player player)
        {
            if (!jcePending) return;

            if (jceSlashSpawned < JCESlashCount)
            {
                jceSlashTimer++;
                if (jceSlashTimer >= JCESlashStagger)
                {
                    jceSlashTimer = 0;

                    float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                    float dist = Main.rand.NextFloat(0f, JCERadius * 0.9f);
                    Vector2 slashPos = player.Center + new Vector2(dist, 0f).RotatedBy(angle);

                    int vis = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        slashPos,
                        Vector2.Zero,
                        ModContent.ProjectileType<HolyDiverIaiSlashVisual>(),
                        0, 0f,
                        Projectile.owner,
                        ai0: slashPos.X + Main.rand.NextFloat(-120f, 120f),
                        ai1: slashPos.Y + Main.rand.NextFloat(-120f, 120f));
                    Main.projectile[vis].netUpdate = true;

                    jceSlashSpawned++;
                }
            }

            if (jceFreezeTimer > 0)
            {
                jceFreezeTimer--;

                if (Main.rand.NextBool(2))
                {
                    float a = Main.rand.NextFloat(MathHelper.TwoPi);
                    float r = Main.rand.NextFloat(60f, 260f);
                    Vector2 p = player.Center + new Vector2(r, 0f).RotatedBy(a);
                    Vector2 v = (player.Center - p) * 0.06f;
                    int dustType = Main.rand.NextBool(3) ? DustID.Electric : DustID.Water;
                    Color col = Main.rand.NextBool(3) ? Color.White : Color.DeepSkyBlue;
                    int d = Dust.NewDust(p, 2, 2, dustType, v.X, v.Y, 0, col, 1.8f);
                    Main.dust[d].noGravity = true;
                }

                if (jceFreezeTimer > 0) return;
            }

            if (Projectile.owner == Main.myPlayer)
                ApplyJCEDamage(player);

            for (int i = 0; i < 80; i++)
            {
                float a = Main.rand.NextFloat(MathHelper.TwoPi);
                float speed = Main.rand.NextFloat(4f, 14f);
                Vector2 vel = new Vector2(speed, 0f).RotatedBy(a);
                int dustType = (i % 4 == 0) ? DustID.Electric : DustID.Water;
                Color col = (i % 4 == 0) ? Color.White : Color.OrangeRed;
                int d = Dust.NewDust(player.Center, 6, 6, dustType, vel.X, vel.Y, 0, col, 2.2f);
                Main.dust[d].noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item62, player.Center);

            jcePending = false;
            Projectile.netUpdate = true;
        }

        private void ApplyJCEDamage(Player player)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active) continue;
                if (npc.friendly) continue;
                if (npc.lifeMax <= 5) continue;
                if (npc.dontTakeDamage) continue;

                float dist = Vector2.Distance(player.Center, npc.Center);
                if (dist > JCERadius) continue;

                npc.SimpleStrikeNPC(
                    damage: JCEDamage,
                    hitDirection: npc.Center.X >= player.Center.X ? 1 : -1,
                    crit: true,
                    noPlayerInteraction: false);

                for (int k = 0; k < 12; k++)
                {
                    float a = Main.rand.NextFloat(MathHelper.TwoPi);
                    Vector2 v = new Vector2(Main.rand.NextFloat(3f, 9f), 0f).RotatedBy(a);
                    int d = Dust.NewDust(npc.Center, 4, 4, DustID.Water,
                        v.X, v.Y, 0, Color.OrangeRed, 2.0f);
                    Main.dust[d].noGravity = true;
                }
            }
        }

        private void ClearAllDebuffs(Player player)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (player.buffTime[i] <= 0) continue;
                int type = player.buffType[i];
                if (type > 0 && Main.debuff[type])
                {
                    player.DelBuff(i);
                    i--;
                }
            }
        }

        private void SpawnInstallParticles(Player player)
        {
            for (int i = 0; i < 40; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float speed = Main.rand.NextFloat(2f, 7f);
                Vector2 vel = new Vector2(speed, 0f).RotatedBy(angle);

                int dustType = (i % 3 == 0) ? DustID.Electric : DustID.Water;
                Color color = (i % 3 == 0) ? Color.White : Color.DeepSkyBlue;

                int d = Dust.NewDust(player.Center, 4, 4, dustType,
                    vel.X, vel.Y, 0, color, Main.rand.NextFloat(1.2f, 2.2f));
                Main.dust[d].noGravity = true;
            }
        }

        #endregion


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
            writer.Write(holyWaterBeamTimer);
            writer.Write(holyWaterCooldownTimer);
            writer.Write(symbiosisSwordCooldown);
            writer.Write(symbiosisM1HoldTimer);
            writer.Write(iaiSlashCooldown);
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
            holyWaterBeamTimer = reader.ReadInt32();
            holyWaterCooldownTimer = reader.ReadInt32();
            symbiosisSwordCooldown = reader.ReadInt32();
            symbiosisM1HoldTimer = reader.ReadInt32();
            iaiSlashCooldown = reader.ReadInt32();
        }


        public override bool PreDrawExtras() => false;

        public override void PostDrawExtras()
        {
            // TODO: Draw current M2 mode indicator (Mine / Water Cannon / Water Replicant)
        }
    }
}