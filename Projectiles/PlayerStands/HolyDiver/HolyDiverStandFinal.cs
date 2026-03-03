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
        // Water Cannon tuning
        // -------------------------------------------------------
        /// <summary>Base damage for the Water Cannon beam.</summary>
        private const int WaterCannonDamage = 75;
        /// <summary>Base damage for each homing missile.</summary>
        private const int WaterMissileDamage = 55;
        /// <summary>Speed of the beam projectile (pixels per extraUpdate tick).</summary>
        private const float CannonSpeed = 18f;
        /// <summary>Speed at which missiles are launched before homing kicks in.</summary>
        private const float MissileSpeed = 9f;
        /// <summary>
        /// How long (ticks) right-click must be held before missiles fire instead of the beam.
        /// Acts as a short charge window so accidental holds don't trigger missiles.
        /// </summary>
        private const int MissileChargeThreshold = 12;
        /// <summary>Cooldown in ticks between Water Cannon uses.</summary>
        private const int WaterCannonCooldown = 45;

        // Max targets scale with tier: tier 4 = 3 targets cap
        private int MaxMissileTargets => TierNumber >= 4 ? 3 : TierNumber >= 3 ? 2 : 1;

        private int waterCannonCooldownTimer = 0;
        /// <summary>Tracks how many ticks right-click has been held during a Water Cannon window.</summary>
        private int rightClickHoldTimer = 0;
        /// <summary>True while the Water Cannon ability is in its active window (Special key has been pressed once).</summary>
        private bool waterCannonActive = false;

        private int punchAnimationTimer = 0;

        public new enum AnimationState
        {
            Idle,
            Attack,         // M1 - Scorching Torrent Barrage
            KnifeThrow,     // Water Cannon / Missile fire animation
            Secondary,      // Skill Wheel usage / general ability
            HydroSymbiosis, // Special 2 install form
            Pose
        }


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


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            SelectAnimation();
            UpdateStandInfo();

            if (shootCount > 0)
                shootCount--;

            if (waterCannonCooldownTimer > 0)
                waterCannonCooldownTimer--;

            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    // ---- M1: Scorching Torrent Barrage ----
                    if (Main.mouseLeft && !waterCannonActive)
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

                    // ---- M2 while Water Cannon is active: charge for missiles ----
                    if (waterCannonActive)
                    {
                        if (Main.mouseRight)
                            rightClickHoldTimer++;
                        else
                            rightClickHoldTimer = 0;
                    }

                    // ---- M2 (secondary, no ability window): original secondary ----
                    if (!waterCannonActive && Main.mouseRight)
                    {
                        secondaryAbility = true;
                        StayBehindWithAbility();
                        currentAnimationState = AnimationState.Secondary;
                    }
                    else if (!waterCannonActive)
                    {
                        secondaryAbility = false;
                    }
                }

                if (!attacking && !waterCannonActive)
                {
                    StayBehind();
                    currentAnimationState = AnimationState.Idle;
                    punchAnimationTimer = 0;
                }

                // ---- Special 1: DEBUG — directly fire Water Cannon ability ----
                if (SpecialKeyPressed() && waterCannonCooldownTimer <= 0)
                {
                    if (!waterCannonActive)
                    {
                        // Enter Water Cannon window — player now has MissileChargeThreshold ticks
                        // to hold right-click before we fire; otherwise fires beam on release.
                        waterCannonActive = true;
                        rightClickHoldTimer = 0;
                        currentAnimationState = AnimationState.KnifeThrow;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        // Second press cancels the window and fires beam immediately
                        if (Projectile.owner == Main.myPlayer)
                            FireWaterCannon(player, missile: false);
                        waterCannonActive = false;
                    }
                }

                // ---- Resolve Water Cannon window ----
                if (waterCannonActive && Projectile.owner == Main.myPlayer)
                {
                    // If right-click held long enough -> fire missiles
                    if (rightClickHoldTimer >= MissileChargeThreshold)
                    {
                        FireWaterCannon(player, missile: true);
                        waterCannonActive = false;
                        rightClickHoldTimer = 0;
                        waterCannonCooldownTimer = WaterCannonCooldown;
                        Projectile.netUpdate = true;
                    }
                    // Left-click while in window -> fire beam instantly
                    else if (Main.mouseLeft)
                    {
                        FireWaterCannon(player, missile: false);
                        waterCannonActive = false;
                        rightClickHoldTimer = 0;
                        waterCannonCooldownTimer = WaterCannonCooldown;
                        Projectile.netUpdate = true;
                    }
                }

                // ---- Special 2 - Hydro Symbiosis install form ----
                if (SecondSpecialKeyPressed(false))
                {
                    // TODO: Enter Hydro Symbiosis form
                    currentAnimationState = AnimationState.HydroSymbiosis;
                }
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
        // Scorching Hot Water Cannon
        // -------------------------------------------------------
        private void FireWaterCannon(Player player, bool missile)
        {
            currentAnimationState = AnimationState.KnifeThrow;
            Projectile.netUpdate = true;

            if (!missile)
            {
                // ---- Beam mode ----
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
            else
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

                if (found < cap)
                {
                    ids[found] = npc.whoAmI;
                    dists[found] = d;
                    found++;
                }
            }

            int[] result = new int[found];
            for (int i = 0; i < found; i++)
                result[i] = ids[i];

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

            // If no targets found at all, fall back to -1 (missile will self-home)
            if (result.Length == 0)
                return new int[] { -1 };

            return result;
        }


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
            else if (currentAnimationState == AnimationState.KnifeThrow)
                PlayAnimation("WaterCannon");
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
            writer.Write(waterCannonActive);
            writer.Write(waterCannonCooldownTimer);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            secondaryAbility = reader.ReadBoolean();
            waterCannonActive = reader.ReadBoolean();
            waterCannonCooldownTimer = reader.ReadInt32();
        }

        // -------------------------------------------------------
        // Draw
        // -------------------------------------------------------

        public override bool PreDrawExtras()
        {
            // TODO: Draw after-images for Hydro Symbiosis if needed
            return false;
        }

        public override void PostDrawExtras()
        {
            // TODO: Draw front-layer effects (e.g. water effects on M1)
        }
    }
}