using JoJoFanStands.Buffs;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
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
        public override float MaxDistance => 62.5f * 16f;
        public override Vector2 StandOffset => new Vector2(-2 * 2, 0f);
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;

        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        private int punchAnimationTimer = 0;

        public new enum AnimationState
        {
            Idle,
            Attack,         // M1 - Scorching Torrent Barrage
            KnifeThrow,     // Placeholder slot (e.g. Water Cannon)
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
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            // FanPlayer fPlayer = player.GetModPlayer<FanPlayer>(); // Uncomment when FanPlayer liquid fields are added

            SelectAnimation();
            UpdateStandInfo();

            if (shootCount > 0)
                shootCount--;

            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft)
                    {
                        Punch(7f, afterImages: false);
                        currentAnimationState = AnimationState.Idle;
                        punchAnimationTimer++;
                        Projectile.netUpdate = true;
                    }
                    else if (Main.mouseRight)
                    {
                        // M2 - Ability usage (Skill Wheel abilities will go here)
                        secondaryAbility = true;
                        StayBehind();
                        currentAnimationState = AnimationState.Secondary;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        secondaryAbility = false;
                    }
                }

                if (!attacking)
                {
                    StayBehind();
                    currentAnimationState = AnimationState.Idle;
                    punchAnimationTimer = 0;
                }

                // Special 1 - Skill Selection Wheel
                if (SpecialKeyPressed())
                {
                    // TODO: Open Skill Selection Wheel
                }

                // Special 2 - Hydro Symbiosis install form
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
            // TODO: Write liquid gauge and other state fields
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            secondaryAbility = reader.ReadBoolean();
            // TODO: Read liquid gauge and other state fields
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