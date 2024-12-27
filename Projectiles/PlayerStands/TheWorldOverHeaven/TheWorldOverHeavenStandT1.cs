using JoJoFanStands.Buffs;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Buffs.EffectBuff;
using JoJoStands.Items;
using JoJoStands.Projectiles;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.TheWorldOverHeaven
{
    public class TheWorldOverHeavenStandT1 : StandClass
    {
        public override int PunchDamage => 173;
        public override int AltDamage => 397;
        public override int PunchTime => 7;
        public override int HalfStandHeight => 44;
        public override int FistID => 1;
        public override int TierNumber => 4;
        public override string PunchSoundName => "Muda";
        public override string PoseSoundName => "ComeAsCloseAsYouLike";
        public override string SpawnSoundName => "The World";
        public override int AmountOfPunchVariants => 3;
        public override string PunchTexturePath => "JoJoFanStands/Projectiles/PlayerStands/TheWorldOverHeaven/TWOH_Punch_";
        public override Vector2 PunchSize => new Vector2(28, 12);
        public override PunchSpawnData PunchData => new PunchSpawnData()
        {
            standardPunchOffset = new Vector2(10f, 0f),
            minimumLifeTime = 6,
            maximumLifeTime = 12,
            minimumTravelDistance = 18,
            maximumTravelDistance = 36,
            bonusAfterimageAmount = 0
        };

        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;
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
        private bool activatedTimestop = false;
        private NPC snapTarget;
        private bool snapping = false;

        public new enum AnimationState
        {
            Idle,
            Attack,
            SecondaryAbility,
            Special,
            RealityOverwritePunch,
            Pose
        }

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

            int highestDamage = PunchDamage;
            for (int i = 0; i < 50; i++)        //At 50 is where the actual inventory stops, the rest is coins and ammo.
            {
                if (player.inventory[i] != null)
                {
                    if (player.inventory[i].damage > highestDamage)
                        highestDamage = player.inventory[i].damage;
                }
            }
            newPunchDamage = (int)(highestDamage * player.GetModPlayer<MyPlayer>().standDamageBoosts);
            int newAltDamage = (int)(AltDamage + (newPunchDamage - PunchDamage) * player.GetModPlayer<MyPlayer>().standDamageBoosts);
            if (Projectile.owner == Main.myPlayer)
                mPlayer.StandSlot.SlotItem.damage = newPunchDamage;

            if (SpecialKeyPressed() && !player.HasBuff(ModContent.BuffType<TheWorldBuff>()) && timestopStartDelay <= 0)
            {
                activatedTimestop = false;
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
                    timestopPoseTimer = 60;
                    timestopStartDelay = 0;
                }
            }
            if (timestopPoseTimer > 0)
            {
                abilityPose = true;
                Main.mouseLeft = false;
                Main.mouseRight = false;
                timestopPoseTimer--;
                if (timestopPoseTimer <= 0)
                    abilityPose = false;
            }
            if (mPlayer.timestopActive && !mPlayer.timestopOwner)
                return;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft && !secondaryAbility)
                    {
                        if (!fPlayer.realityRewriteActive)
                        {
                            Punch();
                            currentAnimationState = AnimationState.Attack;
                        }
                    }
                    else
                    {
                        attacking = false;
                        currentAnimationState = AnimationState.Idle;
                    }

                    if (Main.mouseRight && !playerHasAbilityCooldown && !snapping)
                    {
                        secondaryAbility = true;
                        Projectile.netUpdate = true;
                        currentAnimationState = AnimationState.SecondaryAbility;
                        Projectile.frame = 0;
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.active && npc.Hitbox.Contains(Main.MouseWorld.ToPoint()))
                            {
                                snapping = true;
                                snapTarget = npc;
                                break;
                            }
                        }

                        Vector2 cloudSpawnPosition = Main.MouseWorld - new Vector2(0f, (Main.screenHeight / 2f) + 48);
                        int dustIndex = Dust.NewDust(cloudSpawnPosition, 32, 32, ModContent.DustType<JoJoStands.Dusts.GratefulDeadCloud>());
                        Main.dust[dustIndex].color = Color.Lerp(Color.Gray, Color.DarkGray, Main.rand.Next(20 - 15, 45 + 1 - 15) / 100f);
                        if (Main.rand.Next(1, 100 + 1) <= 5)
                        {
                            dustIndex = Dust.NewDust(cloudSpawnPosition, 32 * 4, 16 * 4, DustID.IchorTorch);
                            Main.dust[dustIndex].noGravity = true;
                        }
                    }
                    if (!Main.mouseRight && !snapping)
                        secondaryAbility = false;
                }
                if (snapping)
                {
                    secondaryAbility = true;
                    currentAnimationState = AnimationState.SecondaryAbility;
                    if (shootCount <= 0 && Projectile.frame == 4)
                    {
                        shootCount += 15;
                        CreateLightningStrike(snapTarget.Center, snapTarget.Center + new Vector2(Main.rand.Next(-6, 6 + 1), -Main.screenHeight / 2f), 36, 12, Main.rand.Next(4, 9));
                        if (Projectile.owner == Main.myPlayer)
                        {
                            NPC.HitInfo snapHitInfo = new NPC.HitInfo()
                            {
                                Damage = newAltDamage,
                                Knockback = 6f,
                                HitDirection = -snapTarget.direction
                            };
                            snapTarget.StrikeNPC(snapHitInfo);
                            NetMessage.SendStrikeNPC(snapTarget, snapHitInfo, Projectile.owner);

                            for (int n = 0; n < Main.maxNPCs; n++)
                            {
                                if (n == snapTarget.whoAmI)
                                    continue;

                                NPC npc = Main.npc[n];
                                if (npc.active && Vector2.Distance(snapTarget.Center, npc.Center) < 16f)
                                {
                                    NPC.HitInfo hitInfo = new NPC.HitInfo()
                                    {
                                        Damage = newAltDamage / 2,
                                        Knockback = 6f,
                                        HitDirection = -npc.direction
                                    };
                                    npc.StrikeNPC(hitInfo);
                                    NetMessage.SendStrikeNPC(snapTarget, hitInfo, Projectile.owner);
                                }
                            }
                        }
                        SoundEngine.PlaySound(SoundID.Item14.WithPitchOffset(-1f), Projectile.Center);
                        SoundEngine.PlaySound(SoundID.Item41.WithPitchOffset(-1f).WithVolumeScale(1.2f), Projectile.Center);
                        snapTarget = null;
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(10));
                    }

                    if (snapTarget != null)
                    {
                        Vector2 cloudSpawnPosition = snapTarget.Center - new Vector2(0f, (Main.screenHeight / 2f) + 48);
                        int dustIndex = Dust.NewDust(cloudSpawnPosition, 32, 32, ModContent.DustType<JoJoStands.Dusts.GratefulDeadCloud>());
                        Main.dust[dustIndex].color = Color.Lerp(Color.Gray, Color.DarkGray, Main.rand.Next(20 - 15, 45 + 1 - 15) / 100f);
                        if (Main.rand.Next(1, 100 + 1) <= 5)
                        {
                            dustIndex = Dust.NewDust(cloudSpawnPosition, 32 * 4, 16 * 4, DustID.IchorTorch);
                            Main.dust[dustIndex].noGravity = true;
                        }
                    }
                }

                if (!attacking)
                {
                    if (!secondaryAbility)
                    {
                        StayBehind();
                        currentAnimationState = AnimationState.Idle;
                        Projectile.direction = Projectile.spriteDirection = player.direction;
                    }
                    else
                    {
                        GoInFront(player.direction);
                        currentAnimationState = AnimationState.SecondaryAbility;
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
                        int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<RoadRoller>(), (int)(newPunchDamage * 3 * mPlayer.standDamageBoosts), 12f, player.whoAmI);
                        Main.projectile[projIndex].netUpdate = true;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                PunchAndShootAI(ModContent.ProjectileType<KnifeProjectile>(), ModContent.ItemType<Knife>(), true, 30);
            }
            if (abilityPose)
                currentAnimationState = AnimationState.Special;
            if (mPlayer.posing)
                currentAnimationState = AnimationState.Pose;
            if (Main.myPlayer != Projectile.owner && currentAnimationState == AnimationState.SecondaryAbility && !snapping)
                Projectile.frame = 0;
        }

        public void CreateLightningStrike(Vector2 startPosition, Vector2 targetPosition, int positionOffset, int maxDustSpread, int pathBreaks)
        {
            Vector2 lastEndPosition = Vector2.Zero;
            float totalDistance = Vector2.Distance(startPosition, targetPosition);
            for (int i = 0; i < pathBreaks; i++)
            {
                float lightningProgress = (float)Math.Abs(i - pathBreaks) / pathBreaks;
                Vector2 lightningStartPosition = i == 0 ? startPosition : lastEndPosition;
                Vector2 lightningEndPosition = Vector2.Lerp(startPosition, targetPosition, (i + 1) / (float)pathBreaks) + (new Vector2(Main.rand.Next(-positionOffset, positionOffset + 1), Main.rand.Next(-positionOffset, positionOffset + 1)) / lightningProgress);
                if (i == pathBreaks - 1)
                    lightningEndPosition = targetPosition;

                lastEndPosition = lightningEndPosition;
                int amountOfDusts = (int)(Vector2.Distance(lightningStartPosition, lightningEndPosition) * 10f * (1f - (0.8f * (Vector2.Distance(lightningStartPosition, targetPosition) / totalDistance)) + 0.2f));
                for (int d = 0; d < amountOfDusts; d++)
                {
                    Vector2 dustPosition = Vector2.Lerp(lightningStartPosition, lightningEndPosition, d / (float)amountOfDusts);
                    dustPosition += new Vector2(Main.rand.Next(-maxDustSpread, maxDustSpread + 1), Main.rand.Next(-maxDustSpread, maxDustSpread + 1)) * (1f - (Vector2.Distance(dustPosition, targetPosition) / totalDistance));
                    Vector2 dustVelocity = lightningStartPosition - lightningEndPosition;
                    dustVelocity.Normalize();
                    dustVelocity *= Main.rand.Next(1, 100 + 1) * 0.02f;
                    int dustIndex = Dust.NewDust(dustPosition, 1, 1, DustID.IchorTorch, dustVelocity.X, dustVelocity.Y);
                    Main.dust[dustIndex].noGravity = true;
                }
            }
        }

        public override byte SendAnimationState() => (byte)currentAnimationState;
        public override void ReceiveAnimationState(byte state) => currentAnimationState = (AnimationState)state;

        public override void SendExtraStates(BinaryWriter writer)       //since this is overriden you have to sync the normal stuff
        {
            writer.Write(abilityPose);
            writer.Write(snapping);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            abilityPose = reader.ReadBoolean();
            snapping = reader.ReadBoolean();
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
                PlayAnimation("Snap");
            else if (currentAnimationState == AnimationState.Special)
                PlayAnimation("AbilityPose");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void AnimationCompleted(string animationName)
        {
            if (!activatedTimestop && currentAnimationState == AnimationState.Special && Main.myPlayer == Projectile.owner)
            {
                activatedTimestop = true;
                Timestop(11);
            }
            if (currentAnimationState == AnimationState.SecondaryAbility)
            {
                snapping = false;
                secondaryAbility = false;
            }
        }

        public override void PlayAnimation(string animationName)
        {
            if (Main.netMode != NetmodeID.Server)
                standTexture = GetStandTexture("JoJoFanStands/Projectiles/PlayerStands/TheWorldOverHeaven", "TWOH_" + animationName);

            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
            else if (animationName == "Snap")
                AnimateStand(animationName, 7, 8, false);
            else if (animationName == "RealityOverwrite")
                AnimateStand(animationName, 5, 6, false);
            else if (animationName == "AbilityPose")
                AnimateStand(animationName, 4, 5, false);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 10, true);
        }
    }
}