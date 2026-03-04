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
        private const int WaterCannonDamage = 75;
        private const int WaterMissileDamage = 55;
        private const int MineDamage = 120; // base; multiplied x3 by ModifyHitNPC in the mine itself
        private const int MinePlaceCooldown = 90;
        private int mineCooldownTimer = 0;
        private const float CannonSpeed = 18f;
        private const float MissileSpeed = 9f;
        /// <summary>Ticks the Special key must be held to trigger missiles instead of beam.</summary>
        private const int MissileChargeThreshold = 30;
        private const int WaterCannonCooldown = 45;

        private int MaxMissileTargets => TierNumber >= 4 ? 3 : TierNumber >= 3 ? 2 : 1;

        private int waterCannonCooldownTimer = 0;
        private int specialHoldTimer = 0;
        /// <summary>Counts down while the beam is actively firing after a tap.</summary>
        private int beamTimer = 0;
        /// <summary>How long (ticks) the beam fires after a tap release.</summary>
        private const int BeamDuration = 90;
        /// <summary>How often (ticks) the beam spawns a new projectile while active.</summary>
        private const int BeamFireRate = 8;
        private int beamFireRateTimer = 0;

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
            if (mineCooldownTimer > 0)
                mineCooldownTimer--;

            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    // ---- M1: Scorching Torrent Barrage ----
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

                    // ---- M2: Secondary ability ----
                    if (Main.mouseRight)
                    {
                        secondaryAbility = true;
                        StayBehindWithAbility();
                        currentAnimationState = AnimationState.Secondary;
                    }
                    else
                    {
                        secondaryAbility = false;
                    }

                    // ---- Special 1: Tap = beam (fires for BeamDuration), Hold = missiles ----
                    if (SpecialKeyCurrent() && waterCannonCooldownTimer <= 0 && beamTimer <= 0)
                    {
                        // Key is held — count up toward missile threshold
                        specialHoldTimer++;
                        currentAnimationState = AnimationState.Idle;

                        if (specialHoldTimer >= MissileChargeThreshold)
                        {
                            FireWaterCannon(player, missile: true);
                            waterCannonCooldownTimer = WaterCannonCooldown;
                            specialHoldTimer = 0;
                            Projectile.netUpdate = true;
                        }
                    }
                    else if (!SpecialKeyCurrent() && specialHoldTimer > 0)
                    {
                        // Key was released — wait one extra frame (specialHoldTimer still > 0 here,
                        // we decrement it so next frame it hits 0 and we know the key is truly up)
                        specialHoldTimer--;
                        if (specialHoldTimer == 0 && waterCannonCooldownTimer <= 0)
                        {
                            // Confirmed tap: start beam
                            beamTimer = BeamDuration;
                            beamFireRateTimer = 0;
                            waterCannonCooldownTimer = WaterCannonCooldown + BeamDuration;
                            Projectile.netUpdate = true;
                        }
                    }

                    // ---- Beam active: keep firing projectiles each BeamFireRate ticks ----
                    if (beamTimer > 0)
                    {
                        beamTimer--;
                        beamFireRateTimer--;
                        currentAnimationState = AnimationState.Idle;

                        if (beamFireRateTimer <= 0)
                        {
                            FireWaterCannon(player, missile: false);
                            beamFireRateTimer = BeamFireRate;
                        }

                        if (beamTimer == 0)
                            currentAnimationState = AnimationState.Idle;

                        Projectile.netUpdate = true;
                    }
                }

                if (!attacking && specialHoldTimer == 0)
                {
                    StayBehind();
                    if (!secondaryAbility)
                        currentAnimationState = AnimationState.Idle;
                    punchAnimationTimer = 0;
                }

                // ---- Special 2 - Scorching Hot Mine ----
                if (SecondSpecialKeyPressed() && mineCooldownTimer <= 0)
                {
                    PlaceMine(player);
                    mineCooldownTimer = MinePlaceCooldown;
                    Projectile.netUpdate = true;
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

        private void FireWaterCannon(Player player, bool missile)
        {
            currentAnimationState = AnimationState.Idle;
            Projectile.netUpdate = true;

            if (!missile)
            {
                // Beam mode — fast piercing shot toward cursor
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
                // Missile mode — one missile per found target, fanned out
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

                ids[found] = npc.whoAmI;
                dists[found] = d;
                found++;
            }

            int[] result = new int[found];
            for (int i = 0; i < found; i++)
                result[i] = ids[i];

            // Sort by distance (insertion sort — tiny array)
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

            // No targets found — fire one self-homing missile
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
            writer.Write(specialHoldTimer);
            writer.Write(waterCannonCooldownTimer);
            writer.Write(beamTimer);
            writer.Write(mineCooldownTimer);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            secondaryAbility = reader.ReadBoolean();
            specialHoldTimer = reader.ReadInt32();
            waterCannonCooldownTimer = reader.ReadInt32();
            beamTimer = reader.ReadInt32();
            mineCooldownTimer = reader.ReadInt32();
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
            // TODO: Draw front-layer effects
        }
    }
}