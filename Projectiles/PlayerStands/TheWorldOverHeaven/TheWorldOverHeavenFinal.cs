using JoJoFanStands.Buffs;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Buffs.EffectBuff;
using JoJoStands.Items;
using JoJoStands.Projectiles;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.TheWorldOverHeaven
{
    public class TheWorldOverHeavenStandFinal : StandClass
    {
        public override int PunchDamage => 82;
        public override int AltDamage => 65;
        public override int PunchTime => 10;
        public override int HalfStandHeight => 44;
        public override int FistWhoAmI => 1;
        public override int TierNumber => 4;
        public override string PunchSoundName => "Muda";
        public override string PoseSoundName => "ComeAsCloseAsYouLike";
        public override string SpawnSoundName => "The World";
        public override int AmountOfPunchVariants => 3;
        public override string PunchTexturePath => "JoJoStands/Projectiles/PlayerStands/TheWorld/TheWorld_Punch_";
        public override Vector2 PunchSize => new Vector2(28, 12);
        public override PunchSpawnData PunchData => new PunchSpawnData()
        {
            standardPunchOffset = new Vector2(12f, 6f),
            minimumLifeTime = 5,
            maximumLifeTime = 12,
            minimumTravelDistance = 16,
            maximumTravelDistance = 32,
            bonusAfterimageAmount = 0
        };

        public override bool CanUseSaladDye => true;
        public override StandAttackType StandType => StandAttackType.Melee;
        public static readonly SoundStyle TheWorldTimestopSound = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/TheWorld")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };
        public static readonly SoundStyle RoadRollerSound = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/RoadRollerDa")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };
        private bool abilityPose = false;
        private int timestopPoseTimer = 0;
        private int timestopStartDelay = 0;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            UpdateStandSync();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            int highestDamage = 148;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i] != null)
                {
                    if (player.inventory[i].damage > highestDamage)
                        highestDamage = player.inventory[i].damage;
                }
            }
            newPunchDamage = (int)(highestDamage * player.GetModPlayer<MyPlayer>().standDamageBoosts);

            if (SpecialKeyPressed() && !player.HasBuff(ModContent.BuffType<TheWorldBuff>()) && timestopStartDelay <= 0)
            {
                if (!JoJoStands.JoJoStands.SoundsLoaded || !JoJoStands.JoJoStands.SoundsModAbilityVoicelines)
                    timestopStartDelay = 120;
                else
                {
                    SoundEngine.PlaySound(TheWorldTimestopSound, Projectile.position);
                    timestopStartDelay = 1;
                }
            }
            if (timestopStartDelay != 0)
            {
                timestopStartDelay++;
                if (timestopStartDelay >= 120)
                {
                    Timestop(12);
                    timestopPoseTimer = 60;
                    timestopStartDelay = 0;
                }
            }
            if (timestopPoseTimer > 0)
            {
                timestopPoseTimer--;
                abilityPose = true;
                Main.mouseLeft = false;
                Main.mouseRight = false;
                if (timestopPoseTimer <= 1)
                    abilityPose = false;
            }
            if (mPlayer.timestopActive && !mPlayer.timestopOwner)
                return;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft && !secondaryAbility)
                        Punch();
                    else
                    {
                        attacking = false;
                        currentAnimationState = AnimationState.Idle;
                    }

                    if (Main.mouseRight)
                    {
                        secondaryAbility = true;
                        currentAnimationState = AnimationState.SecondaryAbility;
                        Projectile.netUpdate = true;
                        if (shootCount <= 0 && Projectile.frame == 1)
                        {
                            shootCount += 16;       // has to be half if the framecounter + 1 (2 if shootCount goes to -1)
                            //snap ability
                        }
                    }
                    else
                        secondaryAbility = false;
                }

                if (!attacking)
                {
                    if (!secondaryAbility)
                    {
                        StayBehind();
                        Projectile.direction = Projectile.spriteDirection = player.direction;
                    }
                    else
                    {
                        GoInFront();
                        Projectile.direction = 1;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            if (Main.MouseWorld.X < Projectile.position.X)
                                Projectile.direction = -1;

                            Projectile.spriteDirection = Projectile.direction;
                        }
                    }
                }
                if (SpecialKeyPressed() && player.HasBuff(ModContent.BuffType<TheWorldBuff>()) && timestopPoseTimer <= 0)
                {
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<RoadRoller>()] == 0)
                    {
                        if (JoJoStands.JoJoStands.SoundsLoaded)
                            SoundEngine.PlaySound(RoadRollerSound, Projectile.Center);

                        shootCount += 12;
                        Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                        if (shootVel == Vector2.Zero)
                            shootVel = new Vector2(0f, 1f);

                        shootVel.Normalize();
                        shootVel *= ProjectileSpeed + 4f;
                        int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<RoadRoller>(), (int)(768 * mPlayer.standDamageBoosts), 12f, player.whoAmI);
                        Main.projectile[projIndex].netUpdate = true;
                        Projectile.netUpdate = true;
                    }
                }
                if (SecondSpecialKeyPressed())
                {
                    fPlayer.realityRewriteActive = !fPlayer.realityRewriteActive;
                    if (fPlayer.realityRewriteActive)
                    {
                        player.AddBuff(ModContent.BuffType<RealityRewrite>(), 2);
                        Main.NewText("Reality Rewrite Mode: Active", Color.White);
                    }
                    else
                        Main.NewText("Reality Rewrite Mode: Inactive", Color.White);
                }
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                PunchAndShootAI(ModContent.ProjectileType<KnifeProjectile>(), ModContent.ItemType<Knife>(), true, 15);
            }
            if (abilityPose)
                currentAnimationState = AnimationState.Special;
            if (mPlayer.posing)
                currentAnimationState = AnimationState.Pose;
        }

        public override void SendExtraStates(BinaryWriter writer)       //since this is overriden you have to sync the normal stuff
        {
            writer.Write(abilityPose);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            abilityPose = reader.ReadBoolean();
        }

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
            else if (currentAnimationState == AnimationState.SecondaryAbility)
                PlayAnimation("Secondary");
            else if (currentAnimationState == AnimationState.Special)
                PlayAnimation("AbilityPose");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            if (Main.netMode != NetmodeID.Server)
                standTexture = GetStandTexture("JoJoStands/Projectiles/PlayerStands/TheWorld", "TheWorld_" + animationName);

            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
            else if (animationName == "Secondary")
                AnimateStand(animationName, 2, 14, true);
            else if (animationName == "AbilityPose")
                AnimateStand(animationName, 1, 10, true);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 10, true);
        }
    }
}