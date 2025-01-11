using JoJoFanStands.Buffs;
using JoJoFanStands.Projectiles.PlayerStands.Metempsychosis;
using JoJoStands;
using JoJoStands.Buffs.AccessoryBuff;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Buffs.EffectBuff;
using JoJoStands.Buffs.ItemBuff;
using JoJoStands.DataStructures;
using JoJoStands.Networking;
using JoJoStands.NPCs;
using JoJoStands.Projectiles;
using JoJoStands.Projectiles.Minions;
using JoJoStands.Projectiles.PlayerStands;
using JoJoStands.Projectiles.PlayerStands.CrazyDiamond;
using JoJoStands.Projectiles.PlayerStands.KillerQueen;
using JoJoStands.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static JoJoStands.Projectiles.PlayerStands.KillerQueenBTD.KillerQueenBTDStand;
using Intangible = JoJoFanStands.Buffs.Intangible;

namespace JoJoFanStands.Projectiles.PlayerStands.WaywardSon
{
    public class WaywardSonAbilities
    {
        public int standTier;
        public float standCompletionProgress;
        public int owner;
        public int currentStandType;
        public StandClass standInstance;
        public Player standOwner;
        public bool canAttack = true;
        public bool canDraw = true;
        public bool limitDistance = true;

        private const byte Aerosmith = 0;
        private const byte BadCompany = 1;
        private const byte CenturyBoy = 2;
        private const byte CrazyDiamond = 3;
        private const byte Cream = 4;
        private const byte DollyDagger = 5;
        private const byte Echoes = 6;
        private const byte GoldExperience = 7;
        private const byte GratefulDead = 8;
        private const byte HermitPurple = 9;
        private const byte HierophantGreen = 10;
        private const byte KillerQueen = 11;
        private const byte KillerQueenBTD = 12;
        private const byte KingCrimson = 13;
        private const byte Lock = 14;
        private const byte MagiciansRed = 15;
        private const byte SexPistols = 16;
        private const byte SilverChariot = 17;
        private const byte SoftAndWet = 18;
        private const byte StarPlatinum = 19;
        private const byte StickyFingers = 20;
        private const byte StoneFree = 21;
        private const byte TheHand = 22;
        private const byte TheWorld = 23;
        private const byte TowerOfGray = 24;
        private const byte Tusk = 25;
        private const byte Whitesnake = 26;
        private const byte BackInBlack = 27;
        private const byte Banks = 28;
        private const byte Blur = 29;
        private const byte CoolOut = 30;
        private const byte Expanses = 31;
        private const byte FollowMe = 32;
        private const byte LucyInTheSky = 33;
        private const byte Megalovania = 34;
        private const byte MortalReminder = 35;
        private const byte RoseColoredBoy = 36;
        private const byte SlavesOfFear = 37;
        private const byte TheFates = 38;
        private const byte TheWorldOverHeaven = 39;
        private const byte WaywardSon = 40;
        private const byte GoldExperienceRequiem = 41;
        private const byte Metempsychosis = 42;

        public WaywardSonAbilities(int standTier)
        {
            this.standTier = standTier;
            standCompletionProgress = standTier / 4f;
        }

        private int secondRingTimer = 0;
        private int generalAbilityTimer = 0;
        private int helpTipCooldown;

        private bool shirtless = false;
        private const int AfterImagesLimit = 5;

        private bool spawningField = false;
        private int amountOfLinksCreated = 0;
        private bool pointShot = false;
        private bool linkShotForSpecial = false;
        private Vector2 formPosition = Vector2.Zero;
        private const float EmeraldSplashRadius = 24f * 16f;
        private const float AmountOfEmeraldSplashLinks = 50;

        private bool mimicTowerOfGraySize = false;

        public bool scrapeMode = false;

        private bool healingFrames = false;
        private bool healingFramesRepeatTimerOnlyOnce = false;
        private bool returnToOwner = false;
        private bool playerStandDirectionMismatch = false;
        private bool restorationMode = false;
        private bool restoringObjects = false;
        private bool restoredEnemies = false;
        private bool restorationTargetSelected = false;

        private int healingFramesRepeatTimer = 0;
        private int healingTargetNPC = -1;
        private int healingTargetPlayer = -1;
        private int restorationEffectStartTimer = 0;
        private int tileRestorationTimer = 0;
        private Rectangle mouseClickRect;

        private int currentAct = 0;
        private int rightClickHoldTimer = 0;
        private int echoesTailTipType;

        private readonly string[] EffectNames = new string[4] { "BOING", "KABOOOM", "WOOOSH", "SIZZLE" };
        private readonly Color[] EffectColors = new Color[4] { Color.HotPink, Color.Magenta, Color.LightSkyBlue, Color.IndianRed };

        private bool threeFreeze = false;
        private bool playedThreeFreezeThudSound = false;

        private bool gasActive = false;
        private const float GasDetectionDist = 30 * 16f;

        private int btdStartDelay = 0;

        private int btdPositionSaveTimer = 0;
        private int btdPositionIndex = 0;
        private int btdRevertTimer = 0;
        private int btdRevertTime = 0;
        private int amountOfSavedData = 0;
        private int currentRewindTime = 0;
        private int totalRewindTime = 0;
        private List<Vector2> btdPlayerPositions;       //Positions of KQ:BTD's owner.
        private List<Vector2> btdPlayerVelocities;
        private bool bitesTheDustActive;
        private bool bitesTheDustActivated;
        private PlayerData[] savedPlayerDatas;
        private WorldData savedWorldData;
        private bool saveDataCreated = false;       //For use with all clients that aren't 
        private float btdStartTime;

        private int timeskipStartDelay = 0;
        private bool preparingTimeskip = false;

        private int nailShootCooldown = 0;

        private bool stealingDisc = false;
        private bool waitingForStealEnemy = false;

        private int shotgunChargeTimer;

        private bool IsCrystallized;

        private int attacksLeftBeforeProjectile;

        public static readonly SoundStyle ScrapeSoundEffect = new SoundStyle("JoJoStands/Sounds/GameSounds/BRRR")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };

        private static readonly SoundStyle BtdWarpSoundEffect = new SoundStyle("JoJoStands/Sounds/GameSounds/BiteTheDustEffect")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };
        private static readonly SoundStyle BtdSound = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/BiteTheDust")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };
        public static readonly SoundStyle TimeskipSound = new SoundStyle("JoJoStands/Sounds/GameSounds/TimeSkip");
        public static readonly SoundStyle KingCrimsonSound = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/KingCrimson");

        public void UpdateInformation(int currentStandType, Player owner)
        {
            this.currentStandType = currentStandType;
            standOwner = owner;
            canAttack = true;
            canDraw = true;
            limitDistance = true;
            StandEffects();
        }

        /// <summary>
        /// Manages WS's special abilities.
        /// </summary>
        /// <param name="projectile">Wayward Son's projectile instance.</param>
        public void ManageAbilities(Projectile projectile)
        {
            standInstance = projectile.ModProjectile as StandClass;
            int newProjectileDamage = standInstance.newPunchDamage;
            Player player = standOwner;
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            if (helpTipCooldown > 0)
                helpTipCooldown--;
            if (nailShootCooldown > 0)
                nailShootCooldown--;
            if (generalAbilityTimer > 0)
                generalAbilityTimer--;

            if ((currentStandType == Tusk || currentStandType == Echoes) && standInstance.SecondSpecialKeyPressed(false))
            {
                currentAct++;
                if (currentAct >= standTier)
                    currentAct = 0;
                Main.NewText("Current Act: " + currentAct);
            }

            if (currentStandType == Aerosmith)
            {
                AerosmithRadar.Visible = true;
            }
            else if (currentStandType == BadCompany)
            {
                int amountOfSoldiers = player.ownedProjectileCounts[ModContent.ProjectileType<WaywardSonTiny>()];
                Vector2 randomOffset = new Vector2(Main.rand.Next(-4 * 16, (4 * 16) + 1), Main.rand.Next(-4 * 16, 0));
                if (amountOfSoldiers < 8 * standTier)     //Adding troops
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + randomOffset, player.velocity, ModContent.ProjectileType<WaywardSonTiny>(), 0, 0f, player.whoAmI, standTier);
            }
            else if (currentStandType == CenturyBoy)
            {
                if (standInstance.SpecialKeyPressed())
                    player.AddBuff(ModContent.BuffType<CenturyBoyBuff>(), 2, true);
            }
            else if (currentStandType == CrazyDiamond)
            {
                if (standInstance.SpecialKeyPressed(false) && !healingFrames && !restorationTargetSelected && projectile.owner == Main.myPlayer)
                {
                    restorationMode = !restorationMode;
                    if (restorationMode)
                        Main.NewText("Restoration Mode: Active");
                    else
                        Main.NewText("Restoration Mode: Disabled");
                }
                if (restorationMode)
                {
                    int amountOfDusts = Main.rand.Next(1, 4 + 1);
                    for (int i = 0; i < amountOfDusts; i++)
                    {
                        int index = Dust.NewDust(projectile.position - new Vector2(0f, standInstance.HalfStandHeight), projectile.width, standInstance.HalfStandHeight * 2, DustID.IchorTorch, Scale: Main.rand.Next(8, 12) / 10f);
                        Main.dust[index].noGravity = true;
                        Main.dust[index].velocity = new Vector2(Main.rand.Next(-2, 2 + 1) / 10f, Main.rand.Next(-5, -2 + 1) / 10f);
                    }
                    Lighting.AddLight(projectile.position, TorchID.Ichor);

                    if (Main.mouseRight && restorationEffectStartTimer <= 0 && !standInstance.playerHasAbilityCooldown && projectile.owner == Main.myPlayer)
                    {
                        if (projectile.owner == player.whoAmI)
                            mouseClickRect = new Rectangle((int)(Main.MouseWorld.X - 10), (int)(Main.MouseWorld.Y - 10), 20, 20);

                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.active && !npc.hide && !npc.immortal && npc.Hitbox.Intersects(mouseClickRect))
                            {
                                if (Vector2.Distance(projectile.Center, npc.Center) > 200f)
                                {
                                    Main.NewText(Language.GetText("Mods.JoJoStands.MiscText.CrazyDiamondTargetOOR").Value);
                                    break;
                                }

                                if (Vector2.Distance(projectile.Center, npc.Center) <= 200f && !healingFrames && !restorationTargetSelected)
                                {
                                    restorationTargetSelected = true;
                                    healingTargetNPC = npc.whoAmI;
                                    break;
                                }
                            }
                        }
                        if (!restorationTargetSelected)
                        {
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                if (mPlayer.overHeaven && player.active && player.Hitbox.Intersects(mouseClickRect))
                                {
                                    if (!restorationTargetSelected && !healingFrames)
                                    {
                                        restorationTargetSelected = true;
                                        healingTargetPlayer = player.whoAmI;
                                    }
                                }
                            }
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                for (int p = 0; p < Main.maxPlayers; p++)
                                {
                                    Player otherPlayer = Main.player[p];
                                    if (otherPlayer.active && otherPlayer.Hitbox.Intersects(mouseClickRect))
                                    {
                                        if (Vector2.Distance(projectile.Center, otherPlayer.Center) > 200f)
                                        {
                                            Main.NewText(Language.GetText("Mods.JoJoStands.MiscText.CrazyDiamondTargetOOR").Value);
                                            break;
                                        }

                                        if (Vector2.Distance(projectile.Center, otherPlayer.Center) <= 200f && !restorationTargetSelected && !healingFrames)
                                        {
                                            if (!mPlayer.overHeaven && otherPlayer.whoAmI != player.whoAmI || mPlayer.overHeaven)
                                            {
                                                restorationTargetSelected = true;
                                                healingTargetPlayer = otherPlayer.whoAmI;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!restorationTargetSelected)
                        {
                            SoundEngine.PlaySound(CrazyDiamondStandFinal.RestorationSound, projectile.Center);
                            restorationEffectStartTimer += 180;
                            restoringObjects = true;
                            restoredEnemies = false;
                        }
                    }
                    if (restoringObjects)
                    {
                        if (restorationEffectStartTimer > 0)
                        {
                            for (int n = 0; n < Main.maxNPCs; n++)
                            {
                                NPC npc = Main.npc[n];
                                if (npc.active && !npc.hide && !npc.immortal && npc.GetGlobalNPC<JoJoGlobalNPC>().taggedByCrazyDiamondRestoration && npc.GetGlobalNPC<JoJoGlobalNPC>().crazyDiamondPunchCount >= 7)
                                {
                                    int amountOfNPCDusts = Main.rand.Next(1, 4 + 1);
                                    for (int i = 0; i < amountOfNPCDusts; i++)
                                    {
                                        int index = Dust.NewDust(npc.position, npc.width, npc.height, DustID.IchorTorch, Scale: Main.rand.Next(8, 12) / 10f);
                                        Main.dust[index].noGravity = true;
                                        Main.dust[index].velocity = new Vector2(Main.rand.Next(-2, 2 + 1) / 10f, Main.rand.Next(-5, -2 + 1) / 10f);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!restoredEnemies)
                            {
                                restoredEnemies = true;
                                for (int n = 0; n < Main.maxNPCs; n++)
                                {
                                    NPC npc = Main.npc[n];
                                    if (npc.active && !npc.hide && !npc.immortal && npc.GetGlobalNPC<JoJoGlobalNPC>().taggedByCrazyDiamondRestoration && npc.GetGlobalNPC<JoJoGlobalNPC>().crazyDiamondPunchCount >= 7)
                                    {
                                        npc.defense = (int)(npc.defense * 0.9f);
                                        if (!npc.boss)
                                            npc.lifeMax = (int)(npc.lifeMax * 0.8f);
                                        else
                                            npc.lifeMax = (int)(npc.lifeMax * 0.95f);
                                        npc.life = npc.lifeMax;
                                    }
                                }
                                player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(20));
                            }

                            if (tileRestorationTimer <= 0)
                            {
                                if (mPlayer.crazyDiamondDestroyedTileData.Count <= 0)
                                {
                                    restoringObjects = false;
                                    player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(10));
                                }
                                else
                                {
                                    tileRestorationTimer += 1;
                                    mPlayer.crazyDiamondMessageCooldown = 0;
                                    DestroyedTileData.Restore(mPlayer.crazyDiamondDestroyedTileData[mPlayer.crazyDiamondDestroyedTileData.Count - 1]);
                                    mPlayer.crazyDiamondDestroyedTileData.RemoveAt(mPlayer.crazyDiamondDestroyedTileData.Count - 1);
                                }
                            }

                            int startingIndex = (int)MathHelper.Clamp(mPlayer.crazyDiamondDestroyedTileData.Count - 20, 0, mPlayer.crazyDiamondDestroyedTileData.Count);
                            for (int i = startingIndex; i < mPlayer.crazyDiamondDestroyedTileData.Count; i++)
                            {
                                int index = Dust.NewDust(mPlayer.crazyDiamondDestroyedTileData[i].TilePosition * 16f, 16, 16, DustID.IchorTorch, Scale: Main.rand.Next(8, 12) / 10f);
                                Main.dust[index].noGravity = true;
                            }
                        }
                    }
                }
                if (restorationTargetSelected)
                {
                    float offset = 0f;
                    float offset2 = 0f;
                    if (playerStandDirectionMismatch)
                        offset = -60f * projectile.spriteDirection;
                    if (projectile.spriteDirection == -1)
                        offset2 = 24f;

                    if (healingTargetNPC != -1)
                    {
                        NPC npc = Main.npc[healingTargetNPC];
                        if (!returnToOwner)
                        {
                            if (!healingFrames)
                            {
                                if (npc.Center.X > projectile.Center.X)
                                    projectile.spriteDirection = 1;
                                if (npc.Center.X < projectile.Center.X)
                                    projectile.spriteDirection = -1;
                                projectile.velocity = npc.Center - projectile.Center;
                                projectile.velocity.Normalize();
                                projectile.velocity *= 6f;
                                if (Vector2.Distance(projectile.Center, npc.Center) <= 20f)
                                {
                                    if (projectile.spriteDirection != player.direction)
                                        playerStandDirectionMismatch = true;
                                    projectile.frame = 0;
                                    healingFrames = true;
                                }
                                projectile.netUpdate = true;
                            }
                            else
                            {
                                projectile.position = new Vector2(npc.Center.X - 10f - offset - offset2, npc.Center.Y - 20f);
                                if (projectile.frame == 0 && !healingFramesRepeatTimerOnlyOnce)
                                {
                                    healingFramesRepeatTimer += 1;
                                    healingFramesRepeatTimerOnlyOnce = true;
                                }
                                if (projectile.frame != 0)
                                    healingFramesRepeatTimerOnlyOnce = false;
                                if (healingFramesRepeatTimer >= 4)
                                {
                                    playerStandDirectionMismatch = false;
                                    healingFrames = false;
                                    healingFramesRepeatTimerOnlyOnce = false;
                                    restorationTargetSelected = false;
                                    healingFramesRepeatTimer = 0;
                                    int heal = npc.lifeMax - npc.life;
                                    if (npc.HasBuff(ModContent.BuffType<MissingOrgans>()))
                                        heal = 0;

                                    npc.AddBuff(ModContent.BuffType<Restoration>(), 360);
                                    if (npc.townNPC && heal > 0)
                                        npc.GetGlobalNPC<JoJoGlobalNPC>().crazyDiamondFullHealth = true;
                                    else
                                        npc.lifeMax = npc.life;
                                    player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(20));
                                }
                                projectile.netUpdate = true;
                            }
                        }
                        if (Vector2.Distance(npc.Center, player.Center) > 200f)
                        {
                            returnToOwner = true;
                            healingFrames = false;
                        }
                    }
                    if (healingTargetPlayer != -1)
                    {
                        Player otherPlayer = Main.player[healingTargetPlayer];
                        if (!returnToOwner)
                        {
                            if (!healingFrames)
                            {
                                if (otherPlayer.Center.X > projectile.Center.X)
                                    projectile.spriteDirection = 1;
                                else
                                    projectile.spriteDirection = -1;
                                projectile.velocity = otherPlayer.Center - projectile.Center;
                                projectile.velocity.Normalize();
                                projectile.velocity *= 6f;
                                if (Vector2.Distance(projectile.Center, otherPlayer.Center) <= 20f)
                                {
                                    if (projectile.spriteDirection != player.direction)
                                        playerStandDirectionMismatch = true;
                                    projectile.frame = 0;
                                    healingFrames = true;
                                }
                                projectile.netUpdate = true;
                            }
                            else
                            {
                                projectile.position = new Vector2(otherPlayer.Center.X - 10f - offset - offset2, otherPlayer.Center.Y - 20f);
                                if (projectile.frame == 0)
                                {
                                    if (healingFramesRepeatTimerOnlyOnce)
                                    {
                                        healingFramesRepeatTimer += 1;
                                        healingFramesRepeatTimerOnlyOnce = true;
                                    }
                                }
                                else
                                    healingFramesRepeatTimerOnlyOnce = false;

                                if (healingFramesRepeatTimer >= 4)
                                {
                                    playerStandDirectionMismatch = false;
                                    healingFrames = false;
                                    healingFramesRepeatTimerOnlyOnce = false;
                                    restorationTargetSelected = false;
                                    healingFramesRepeatTimer = 0;
                                    int healthValue = otherPlayer.statLifeMax - otherPlayer.statLife;
                                    if (otherPlayer.HasBuff(ModContent.BuffType<MissingOrgans>()))
                                        healthValue = 0;

                                    player.AddBuff(ModContent.BuffType<ImproperRestoration>(), 10 * 60);

                                    player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(30));
                                }
                                projectile.netUpdate = true;
                            }
                        }
                        if (Vector2.Distance(otherPlayer.Center, player.Center) > 200f)
                        {
                            returnToOwner = true;
                            healingFrames = false;
                        }
                    }
                    if (returnToOwner)
                    {
                        if (projectile.velocity.X < 0)
                            projectile.spriteDirection = -1;
                        else
                            projectile.spriteDirection = 1;
                        projectile.velocity = player.Center - projectile.Center;
                        projectile.velocity.Normalize();
                        projectile.velocity *= 6f + player.moveSpeed;
                        if (Vector2.Distance(projectile.Center, player.Center) <= 20f)
                        {
                            returnToOwner = false;
                            playerStandDirectionMismatch = false;
                            healingFrames = false;
                            healingFramesRepeatTimerOnlyOnce = false;
                            restorationTargetSelected = false;
                            healingFramesRepeatTimer = 0;
                        }
                        projectile.netUpdate = true;
                    }
                    if (!restorationTargetSelected)
                    {
                        healingTargetNPC = -1;
                        healingTargetPlayer = -1;
                    }
                }
            }
            else if (currentStandType == Cream)
            {
                if (standInstance.SpecialKeyPressed() && player.ownedProjectileCounts[ModContent.ProjectileType<WaywardSonVoid>()] <= 0)
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), player.Top, player.velocity, ModContent.ProjectileType<WaywardSonVoid>(), (int)((standInstance.PunchDamage * 0.5f) * mPlayer.standDamageBoosts), 6f, player.whoAmI);
            }
            else if (currentStandType == DollyDagger)
            {
                if (standInstance.SpecialKeyPressed())
                {
                    int stabDamage = Main.rand.Next(50, 81);
                    player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " couldn't reflect enough damage back."), stabDamage, player.direction);
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<DollyDaggerBeam>(), stabDamage, 2f, player.whoAmI);
                }
            }
            else if (currentStandType == Echoes)
            {
                if (currentAct == 0)
                {
                    Rectangle mouseRect = Rectangle.Empty;
                    if (projectile.owner == player.whoAmI)
                        mouseRect = new Rectangle((int)(Main.MouseWorld.X - 10), (int)(Main.MouseWorld.Y - 10), 20, 20);

                    bool targetFound = false;
                    if (standInstance.SpecialKeyPressed()) //right-click ability activation
                    {
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.active && !npc.hide && !npc.immortal)
                            {
                                if (npc.Hitbox.Intersects(mouseRect))
                                {
                                    if (Vector2.Distance(projectile.Center, npc.Center) <= 200f)
                                    {
                                        if (!npc.townNPC)
                                        {
                                            npc.AddBuff(ModContent.BuffType<SMACK>(), 900);
                                            npc.GetGlobalNPC<JoJoGlobalNPC>().echoesDebuffOwner = player.whoAmI;
                                        }
                                        else
                                            npc.AddBuff(ModContent.BuffType<BelieveInMe>(), 1800);
                                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(30));
                                        SoundEngine.PlaySound(SoundID.Item4, npc.Center);
                                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(30));
                                    }
                                    else
                                    {
                                        helpTipCooldown += 90;
                                        Main.NewText(Language.GetText("Mods.JoJoStands.MiscText.CrazyDiamondTargetOOR").Value);
                                    }
                                }
                                else
                                {
                                    if (helpTipCooldown <= 0)
                                    {
                                        helpTipCooldown += 90;
                                        Main.NewText(Language.GetText("Mods.JoJoStands.MiscText.EchoesMouseHint").Value);
                                    }
                                }
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            for (int p = 0; p < Main.maxPlayers; p++)
                            {
                                Player otherPlayer = Main.player[p];
                                if (otherPlayer.active)
                                {
                                    if (otherPlayer.Hitbox.Intersects(mouseRect))
                                    {
                                        if (Vector2.Distance(projectile.Center, otherPlayer.Center) <= 200f)
                                        {
                                            if (!targetFound && otherPlayer.whoAmI != player.whoAmI)
                                            {
                                                if (otherPlayer.hostile && player.hostile && player.InOpposingTeam(otherPlayer))
                                                {
                                                    otherPlayer.AddBuff(ModContent.BuffType<SMACK>(), 360);
                                                    SyncCall.SyncOtherPlayerDebuff(player.whoAmI, otherPlayer.whoAmI, ModContent.BuffType<SMACK>(), 360);
                                                }
                                                else if (!otherPlayer.hostile || !player.hostile || otherPlayer.hostile && player.hostile && !player.InOpposingTeam(otherPlayer))
                                                {
                                                    otherPlayer.AddBuff(ModContent.BuffType<BelieveInMe>(), 720);
                                                    SyncCall.SyncOtherPlayerDebuff(player.whoAmI, otherPlayer.whoAmI, ModContent.BuffType<BelieveInMe>(), 720);
                                                }
                                                SoundEngine.PlaySound(SoundID.Item4, otherPlayer.Center);
                                                player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(30));
                                            }
                                        }
                                        else
                                        {
                                            helpTipCooldown += 90;
                                            Main.NewText(Language.GetText("Mods.JoJoStands.MiscText.CrazyDiamondTargetOOR").Value);
                                        }
                                    }
                                    else
                                    {
                                        if (helpTipCooldown <= 0)
                                        {
                                            helpTipCooldown += 90;
                                            Main.NewText(Language.GetText("Mods.JoJoStands.MiscText.EchoesMouseHint").Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (currentAct == 1)
                {
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<EchoesTailTip>()] == 0 && standInstance.shootCount <= 0)
                    {
                        if (Main.mouseRight)        //right-click ability 
                            rightClickHoldTimer++;
                        else if (rightClickHoldTimer > 0 && rightClickHoldTimer < 60)
                        {
                            rightClickHoldTimer = 0;
                            standInstance.shootCount = 30;
                            echoesTailTipType++;
                            if (echoesTailTipType >= 5)
                                echoesTailTipType = 1;

                            Main.NewText(EffectNames[echoesTailTipType - 1], EffectColors[echoesTailTipType - 1]);
                        }
                    }
                    if (rightClickHoldTimer >= 60)
                    {
                        rightClickHoldTimer = 0;
                        projectile.frame = 0;
                        Vector2 shootVel = Main.MouseWorld - projectile.Center;
                        if (shootVel == Vector2.Zero)
                            shootVel = new Vector2(0f, 1f);
                        shootVel.Normalize();
                        shootVel *= 8f;
                        int projIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, shootVel, ModContent.ProjectileType<EchoesTailTip>(), (int)(newProjectileDamage / 4 * mPlayer.standDamageBoosts), 6f, projectile.owner, projectile.whoAmI);
                        Main.projectile[projIndex].GetGlobalProjectile<JoJoGlobalProjectile>().echoesTailTipTier = mPlayer.echoesTier;
                        Main.projectile[projIndex].GetGlobalProjectile<JoJoGlobalProjectile>().echoesTailTipType = echoesTailTipType;
                        Main.projectile[projIndex].netUpdate = true;
                        projectile.netUpdate = true;
                    }
                    if (mPlayer.echoesTailTip != -1)
                    {
                        if (Main.mouseRight && projectile.owner == Main.myPlayer)
                        {
                            int echoesTailTipStage = Main.projectile[mPlayer.echoesTailTip].GetGlobalProjectile<JoJoGlobalProjectile>().echoesTailTipStage;
                            if (echoesTailTipStage != 2)
                            {
                                standInstance.shootCount = 30;
                                Main.projectile[mPlayer.echoesTailTip].GetGlobalProjectile<JoJoGlobalProjectile>().echoesTailTipStage = 2;
                                echoesTailTipStage = 2;
                            }
                            if (echoesTailTipStage == 2 && standInstance.shootCount == 0)
                            {
                                if (Vector2.Distance(projectile.Center, Main.projectile[mPlayer.echoesTailTip].Center) <= 75f * 16f)
                                {
                                    standInstance.shootCount = 30;
                                }
                                else
                                {
                                    standInstance.shootCount = 30;
                                    Main.NewText(Language.GetText("Mods.JoJoStands.MiscText.EchoesTipOOR").Value);
                                }
                            }
                        }
                    }
                }
                else if (currentAct == 2)
                {
                    Rectangle rectangle = Rectangle.Empty;
                    if (projectile.owner == player.whoAmI)
                        rectangle = new Rectangle((int)(Main.MouseWorld.X - 10), (int)(Main.MouseWorld.Y - 10), 20, 20);

                    if (Main.mouseRight && projectile.owner == Main.myPlayer && !mPlayer.posing && !standInstance.attacking)      //3freeze activation
                    {
                        threeFreeze = true;
                        projectile.frame = 0;
                        bool enemyAffectedByThreeFreeze = false;
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.active && !npc.hide && !npc.immortal)
                            {
                                if (npc.Hitbox.Intersects(rectangle) && Vector2.Distance(projectile.Center, npc.Center) <= 250f)
                                {
                                    enemyAffectedByThreeFreeze = true;
                                    if (npc.GetGlobalNPC<JoJoGlobalNPC>().echoesThreeFreezeTimer <= 15)
                                    {
                                        npc.GetGlobalNPC<JoJoGlobalNPC>().echoesFreezeTarget = true;
                                        npc.GetGlobalNPC<JoJoGlobalNPC>().echoesCrit = mPlayer.standCritChangeBoosts;
                                        npc.GetGlobalNPC<JoJoGlobalNPC>().echoesDamageBoost = mPlayer.standDamageBoosts;
                                        if (npc.GetGlobalNPC<JoJoGlobalNPC>().echoesThreeFreezeTimer <= 15)
                                            npc.GetGlobalNPC<JoJoGlobalNPC>().echoesThreeFreezeTimer += 30;
                                        SyncCall.SyncStandEffectInfo(player.whoAmI, npc.whoAmI, 15, 3, 0, 0, mPlayer.standCritChangeBoosts, mPlayer.standDamageBoosts);
                                    }
                                    break;
                                }
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient && !enemyAffectedByThreeFreeze)
                        {
                            for (int p = 0; p < Main.maxPlayers; p++)
                            {
                                Player otherPlayer = Main.player[p];
                                if (otherPlayer.active && otherPlayer.hostile && player.hostile && player.InOpposingTeam(Main.player[otherPlayer.whoAmI]))
                                {
                                    if (otherPlayer.Hitbox.Intersects(rectangle) && Vector2.Distance(projectile.Center, otherPlayer.Center) <= 250f && otherPlayer.whoAmI != player.whoAmI)
                                    {
                                        MyPlayer mOtherPlayer = otherPlayer.GetModPlayer<MyPlayer>();
                                        if (mOtherPlayer.echoesFreeze <= 15)
                                        {
                                            mOtherPlayer.echoesDamageBoost = mPlayer.standDamageBoosts;
                                            mOtherPlayer.echoesFreeze += 30;
                                            SyncCall.SyncOtherPlayerExtraEffect(player.whoAmI, otherPlayer.whoAmI, 1, 0, 0, mPlayer.standDamageBoosts, 0f);
                                        }
                                        enemyAffectedByThreeFreeze = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (JoJoStands.JoJoStands.SoundsLoaded && !playedThreeFreezeThudSound && enemyAffectedByThreeFreeze)
                            SoundEngine.PlaySound(new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/EchoesActThreeFreeze_Thud").WithVolumeScale(JoJoStands.JoJoStands.ModSoundsVolume), projectile.Center);
                        playedThreeFreezeThudSound = enemyAffectedByThreeFreeze;
                    }
                }
            }
            else if (currentStandType == GoldExperience)
            {
                if (Main.mouseRight && !player.HasBuff(ModContent.BuffType<AbilityCooldown>()))
                {
                    float mouseDistance = Vector2.Distance(Main.MouseWorld, player.Center);
                    bool mouseOnPlatform = TileID.Sets.Platforms[Main.tile[(int)(Main.MouseWorld.X / 16f), (int)(Main.MouseWorld.Y / 16f)].TileType];
                    if (mouseDistance < standInstance.newMaxDistance && (Collision.SolidCollision(Main.MouseWorld, 1, 1) || mouseOnPlatform) && !Collision.SolidCollision(Main.MouseWorld - new Vector2(0f, 16f), 1, 1))
                    {
                        int yPos = (((int)Main.MouseWorld.Y / 16) - 3) * 16;
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), Main.MouseWorld.X, yPos, 0f, 0f, ModContent.ProjectileType<GETree>(), 1, 0f, projectile.owner, standTier);
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime((int)(4 / standCompletionProgress)));
                    }
                }
            }
            else if (currentStandType == GoldExperienceRequiem)
            {
                if (standInstance.SpecialKeyPressed() && !player.HasBuff(ModContent.BuffType<BacktoZero>()))
                {
                    mPlayer.backToZeroActive = true;
                    SyncCall.SyncBackToZero(player.whoAmI, true);
                    player.AddBuff(ModContent.BuffType<BacktoZero>(), 20 * 60);
                }
            }
            else if (currentStandType == GratefulDead)
            {
                if (standInstance.SpecialKeyPressed() && standInstance.shootCount <= 0)
                {
                    standInstance.shootCount += 30;
                    gasActive = !gasActive;
                    if (gasActive)
                        Main.NewText("Gas Spread: On");
                    else
                        Main.NewText("Gas Spread: Off");
                    projectile.netUpdate = true;
                }

                mPlayer.gratefulDeadGasActive = gasActive;
                if (gasActive)
                {
                    float gasRange = GasDetectionDist + mPlayer.standRangeBoosts;
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (npc.active)
                        {
                            float distance = Vector2.Distance(player.Center, npc.Center);
                            if (distance <= gasRange && !npc.immortal && !npc.hide)
                            {
                                npc.GetGlobalNPC<JoJoGlobalNPC>().standDebuffEffectOwner = player.whoAmI;
                                npc.AddBuff(ModContent.BuffType<Aging>(), 2);
                            }
                        }
                    }
                    if (JoJoStands.JoJoStands.StandPvPMode && Main.netMode != NetmodeID.SinglePlayer)
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            Player otherPlayer = Main.player[i];
                            if (otherPlayer.active && otherPlayer.InOpposingTeam(player) && otherPlayer.whoAmI != player.whoAmI)
                            {
                                float distance = Vector2.Distance(player.Center, otherPlayer.Center);
                                if (distance <= gasRange)
                                    otherPlayer.AddBuff(ModContent.BuffType<Aging>(), 2);
                            }
                        }
                    }
                    if (Main.rand.Next(0, 12 + 1) == 0)
                        Dust.NewDust(projectile.Center - new Vector2(gasRange / 2f, 0f), (int)gasRange, projectile.height, ModContent.DustType<JoJoStands.Dusts.GratefulDeadCloud>());

                    player.AddBuff(ModContent.BuffType<Aging>(), 2);
                }
            }
            else if (currentStandType == HermitPurple)
            {
                if (standInstance.SpecialKeyPressed() && player.ownedProjectileCounts[ModContent.ProjectileType<HermitPurpleHook>()] <= 0)
                {
                    Vector2 shootVelocity = Main.MouseWorld - player.Center;
                    shootVelocity.Normalize();
                    shootVelocity *= 12f;
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, shootVelocity, ModContent.ProjectileType<HermitPurpleHook>(), 0, 0f, player.whoAmI);
                }
            }
            else if (currentStandType == HierophantGreen)
            {
                if (standInstance.SpecialKeyPressed() && player.ownedProjectileCounts[ModContent.ProjectileType<EmeraldStringPointConnector>()] <= 0 && !spawningField)
                {
                    spawningField = true;
                    formPosition = projectile.position;
                }
                if (spawningField)
                {
                    float randomAngle = MathHelper.ToRadians(Main.rand.Next(0, 360 + 1));
                    Vector2 pointPosition = formPosition + (randomAngle.ToRotationVector2() * EmeraldSplashRadius);

                    if (amountOfLinksCreated < AmountOfEmeraldSplashLinks * standCompletionProgress && generalAbilityTimer <= 0)
                    {
                        if (!linkShotForSpecial)        //50 tendrils, the number spawned limit /2 is the wanted amount
                        {
                            generalAbilityTimer += 2;
                            linkShotForSpecial = true;
                            int emeraldIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), pointPosition, Vector2.Zero, ModContent.ProjectileType<EmeraldStringPoint>(), 0, 2f, player.whoAmI);
                            Main.projectile[emeraldIndex].netUpdate = true;
                            Main.projectile[emeraldIndex].tileCollide = false;
                            int amountOfDusts = Main._rand.Next(1, 4 + 1);
                            for (int i = 0; i < amountOfDusts; i++)
                            {
                                int dustIndex = Dust.NewDust(pointPosition, 18, 18, DustID.GreenTorch);
                                Main.dust[dustIndex].noGravity = true;
                                Main.dust[dustIndex].velocity = Vector2.Zero;
                            }
                        }
                        else
                        {
                            generalAbilityTimer += 2;
                            amountOfLinksCreated += 1;
                            linkShotForSpecial = false;
                            int emeraldIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), pointPosition, Vector2.Zero, ModContent.ProjectileType<EmeraldStringPointConnector>(), 0, 2f, player.whoAmI, 24);
                            Main.projectile[emeraldIndex].netUpdate = true;
                            Main.projectile[emeraldIndex].tileCollide = false;
                            int amountOfDusts = Main._rand.Next(1, 4 + 1);
                            for (int i = 0; i < amountOfDusts; i++)
                            {
                                int dustIndex = Dust.NewDust(pointPosition, 18, 18, DustID.GreenTorch);
                                Main.dust[dustIndex].noGravity = true;
                                Main.dust[dustIndex].velocity = Vector2.Zero;
                            }
                        }
                    }
                    if (amountOfLinksCreated >= AmountOfEmeraldSplashLinks * standCompletionProgress)
                    {
                        spawningField = false;
                        amountOfLinksCreated = 0;
                        formPosition = Vector2.Zero;
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(30));
                    }
                }
            }
            else if (currentStandType == KillerQueen)
            {
                if (standInstance.SpecialKeyPressed() && player.ownedProjectileCounts[ModContent.ProjectileType<SheerHeartAttack>()] == 0)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.position + new Vector2(10f * projectile.direction, 0f), projectile.velocity, ModContent.ProjectileType<SheerHeartAttack>(), 1, 0f, projectile.owner, 1f);
                }
            }
            else if (currentStandType == KillerQueenBTD)
            {
                bitesTheDustActive = player.HasBuff(ModContent.BuffType<BitesTheDust>());
                if (!bitesTheDustActive && saveDataCreated)
                    saveDataCreated = false;
                if (Main.netMode != NetmodeID.SinglePlayer && Main.myPlayer != projectile.owner)
                {
                    Player otherPlayer = Main.player[Main.myPlayer];
                    if (otherPlayer.GetModPlayer<MyPlayer>().bitesTheDustActive && !bitesTheDustActivated)
                    {
                        bitesTheDustActivated = true;
                        totalRewindTime = CalculateRewindTime();
                        if (JoJoStands.JoJoStands.SoundsLoaded)
                            SoundEngine.PlaySound(BtdSound, projectile.Center);
                    }
                }

                if ((standInstance.SpecialKeyPressed() && !bitesTheDustActive) || (bitesTheDustActive && !saveDataCreated && !standInstance.playerHasAbilityCooldown))
                {
                    btdPositionIndex = 0;
                    amountOfSavedData = 0;
                    btdPlayerPositions = new List<Vector2>() { player.position };
                    btdPlayerVelocities = new List<Vector2>() { player.velocity };
                    savedPlayerDatas = new PlayerData[Main.maxPlayers];
                    currentRewindTime = 0;
                    totalRewindTime = 0;
                    btdRevertTime = 35;

                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        savedPlayerDatas[0] = new PlayerData();
                        savedPlayerDatas[0].playerBTDHealth = player.statLife;
                        savedPlayerDatas[0].playerBTDInventory = player.inventory.Clone() as Item[];
                        savedPlayerDatas[0].playerBTDPos = player.position;
                        savedPlayerDatas[0].playerDirection = player.direction;

                        int amountOfBuffs = player.CountBuffs();
                        savedPlayerDatas[0].buffTypes = new int[amountOfBuffs];
                        savedPlayerDatas[0].buffTimes = new int[amountOfBuffs];
                        for (int i = 0; i < amountOfBuffs; i++)
                        {
                            savedPlayerDatas[0].buffTypes[i] = player.buffType[i];
                            savedPlayerDatas[0].buffTimes[i] = player.buffTime[i];
                        }
                    }
                    else
                    {
                        int activeIndex = 0;
                        for (int p = 0; p < Main.maxPlayers; p++)
                        {
                            Player otherPlayer = Main.player[p];
                            if (otherPlayer.active)
                            {
                                savedPlayerDatas[activeIndex].active = true;
                                savedPlayerDatas[activeIndex] = new PlayerData();
                                savedPlayerDatas[activeIndex].playerBTDHealth = otherPlayer.statLife;
                                if (p == Main.myPlayer)
                                    savedPlayerDatas[activeIndex].playerBTDInventory = otherPlayer.inventory.Clone() as Item[];
                                savedPlayerDatas[activeIndex].playerBTDPos = otherPlayer.position;
                                savedPlayerDatas[activeIndex].playerDirection = otherPlayer.direction;

                                int amountOfBuffs = otherPlayer.CountBuffs();
                                savedPlayerDatas[activeIndex].buffTypes = new int[amountOfBuffs];
                                savedPlayerDatas[activeIndex].buffTimes = new int[amountOfBuffs];
                                for (int i = 0; i < amountOfBuffs; i++)
                                {
                                    savedPlayerDatas[activeIndex].buffTypes[i] = otherPlayer.buffType[i];
                                    savedPlayerDatas[activeIndex].buffTimes[i] = otherPlayer.buffTime[i];
                                }
                                savedPlayerDatas[activeIndex].whoAmI = (byte)p;
                                activeIndex++;
                            }
                        }
                    }

                    player.AddBuff(ModContent.BuffType<BitesTheDust>(), 5 * 60 * 60);      //So it doesn't save
                    mPlayer.standChangingLocked = true;

                    savedWorldData = new WorldData();
                    savedWorldData.worldTime = Utils.GetDayTimeAs24FloatStartingFromMidnight();
                    savedWorldData.npcData = new NPCData[Main.maxNPCs];
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        savedWorldData.npcData[i] = new NPCData();
                        if (npc.active)
                        {
                            savedWorldData.npcData[i].type = npc.type;
                            savedWorldData.npcData[i].position = npc.position;
                            savedWorldData.npcData[i].velocity = npc.velocity;
                            savedWorldData.npcData[i].health = npc.life;
                            savedWorldData.npcData[i].direction = npc.direction;
                            savedWorldData.npcData[i].ai = npc.ai;
                            savedWorldData.npcData[i].active = true;
                        }
                    }
                    projectile.netUpdate = true;
                    saveDataCreated = true;
                }

                if (standInstance.SpecialKeyPressed() && bitesTheDustActive && btdStartDelay <= 0)
                {
                    if (!JoJoStands.JoJoStands.SoundsLoaded || !JoJoStands.JoJoStands.SoundsModAbilityVoicelines)
                    {
                        bitesTheDustActivated = true;
                        totalRewindTime = CalculateRewindTime();
                        SoundEngine.PlaySound(BtdWarpSoundEffect);
                        SyncCall.SyncBitesTheDust(player.whoAmI, true);
                    }
                    else
                        btdStartDelay = 205;
                    projectile.netUpdate = true;
                }
                if (JoJoStands.JoJoStands.SoundsLoaded && !bitesTheDustActivated && btdStartDelay > 0)
                {
                    btdStartDelay--;
                    if (btdStartDelay <= 0)
                    {
                        bitesTheDustActivated = true;
                        totalRewindTime = CalculateRewindTime();
                        SoundEngine.PlaySound(BtdSound, projectile.Center);
                        SyncCall.SyncBitesTheDust(player.whoAmI, true);
                        projectile.netUpdate = true;
                    }
                }
                if (bitesTheDustActive && !bitesTheDustActivated && saveDataCreated)       //Records
                {
                    btdPositionSaveTimer++;
                    if (btdPositionSaveTimer >= 30)
                    {
                        btdPositionSaveTimer = 0;
                        btdPositionIndex++;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            btdPlayerPositions.Add(player.position);
                            btdPlayerVelocities.Add(-player.velocity);
                        }
                        else
                        {
                            btdPlayerPositions.Add(Main.player[Main.myPlayer].position);
                            btdPlayerVelocities.Add(-Main.player[Main.myPlayer].velocity);
                        }
                        amountOfSavedData++;
                    }
                }
                if (bitesTheDustActivated)      //Actual activation
                {
                    if (totalRewindTime == 0 && Main.myPlayer != projectile.owner)
                    {
                        totalRewindTime = CalculateRewindTime();
                        if (JoJoStands.JoJoStands.SoundsLoaded)
                            SoundEngine.PlaySound(BtdSound, projectile.Center);
                    }

                    btdRevertTimer++;
                    currentRewindTime++;
                    mPlayer.bitesTheDustActive = true;
                    mPlayer.biteTheDustEffectProgress = (float)currentRewindTime / (float)totalRewindTime;
                    if (Main.netMode == NetmodeID.MultiplayerClient && projectile.owner != Main.myPlayer)
                        Main.player[Main.myPlayer].GetModPlayer<MyPlayer>().biteTheDustEffectProgress = (float)currentRewindTime / (float)totalRewindTime;

                    mPlayer.bitesTheDustNewTime = (MathHelper.Lerp(btdStartTime, savedWorldData.worldTime, mPlayer.biteTheDustEffectProgress)) % 24f;        //range from 0 - 24                    if (btdRevertTimer >= btdRevertTime)
                    {
                        btdRevertTime = (int)(btdRevertTime * 0.8f);
                        if (btdRevertTime < 2)
                            btdRevertTime = 2;

                        btdRevertTimer = 0;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            player.position = btdPlayerPositions[btdPositionIndex];
                            player.velocity = btdPlayerVelocities[btdPositionIndex];
                        }
                        else
                        {
                            if (btdPlayerPositions != null && btdPositionIndex < btdPlayerPositions.Count)
                            {
                                Main.player[Main.myPlayer].position = btdPlayerPositions[btdPositionIndex];
                                Main.player[Main.myPlayer].velocity = btdPlayerVelocities[btdPositionIndex];
                            }
                        }
                        btdPositionIndex--;
                        if (btdPositionIndex <= 0)
                        {
                            bitesTheDustActivated = false;
                            mPlayer.bitesTheDustActive = false;
                            mPlayer.biteTheDustEffectProgress = 0f;
                            mPlayer.standChangingLocked = false;
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                player.statLife = savedPlayerDatas[0].playerBTDHealth;
                                player.position = savedPlayerDatas[0].playerBTDPos;
                                player.velocity = Vector2.Zero;
                                player.inventory = savedPlayerDatas[0].playerBTDInventory;
                                player.ChangeDir(savedPlayerDatas[0].playerDirection);
                                for (int i = 0; i < savedPlayerDatas[0].buffTypes.Length; i++)
                                {
                                    player.buffType[i] = savedPlayerDatas[0].buffTypes[i];
                                    player.buffTime[i] = savedPlayerDatas[0].buffTimes[i];
                                }
                            }
                            else
                            {
                                for (int i = 0; i < savedPlayerDatas.Length; i++)
                                {
                                    if (!savedPlayerDatas[i].active)
                                        continue;

                                    Player otherPlayer = Main.player[savedPlayerDatas[i].whoAmI];
                                    if (otherPlayer.active)
                                    {
                                        otherPlayer.statLife = savedPlayerDatas[i].playerBTDHealth;
                                        otherPlayer.position = savedPlayerDatas[i].playerBTDPos;
                                        otherPlayer.velocity = Vector2.Zero;
                                        if (savedPlayerDatas[i].whoAmI == Main.myPlayer)
                                            otherPlayer.inventory = savedPlayerDatas[i].playerBTDInventory;
                                        otherPlayer.ChangeDir(savedPlayerDatas[i].playerDirection);
                                        otherPlayer.ClearBuff(ModContent.BuffType<BitesTheDust>());

                                        for (int b = 0; b < savedPlayerDatas[i].buffTypes.Length; b++)
                                        {
                                            otherPlayer.buffType[b] = savedPlayerDatas[i].buffTypes[b];
                                            otherPlayer.buffTime[b] = savedPlayerDatas[i].buffTimes[b];
                                        }
                                    }
                                }
                            }

                            player.ClearBuff(ModContent.BuffType<BitesTheDust>());
                            SoundEngine.PlaySound(KillerQueenStandFinal.KillerQueenClickSound, projectile.Center);
                            if (projectile.owner == Main.myPlayer)
                            {
                                SyncCall.SyncBitesTheDust(player.whoAmI, false);
                                player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(120));
                            }
                            if (Main.netMode != NetmodeID.SinglePlayer && projectile.owner != Main.myPlayer)
                            {
                                Main.player[Main.myPlayer].GetModPlayer<MyPlayer>().bitesTheDustActive = false;
                                Main.player[Main.myPlayer].GetModPlayer<MyPlayer>().biteTheDustEffectProgress = 0f;
                                JoJoStandsShaders.ChangeShaderActiveState(JoJoStandsShaders.BiteTheDustEffect, false);
                                JoJoStandsShaders.ChangeShaderUseProgress(JoJoStandsShaders.BiteTheDustEffect, 0f);
                            }
                            saveDataCreated = false;
                        }
                    }
                }
            }
            else if (currentStandType == KingCrimson)
            {
                if (standInstance.SpecialKeyPressed() && !player.HasBuff(ModContent.BuffType<SkippingTime>()) && timeskipStartDelay <= 0 && mPlayer.kingCrimsonBuffIndex == -1)
                {
                    if (!JoJoStands.JoJoStands.SoundsLoaded || !JoJoStands.JoJoStands.SoundsModAbilityVoicelines)
                        timeskipStartDelay = 80;
                    else
                    {
                        SoundStyle kingCrimsonSound = KingCrimsonSound;
                        kingCrimsonSound.Volume = JoJoStands.JoJoStands.ModSoundsVolume;
                        SoundEngine.PlaySound(kingCrimsonSound, projectile.position);
                        timeskipStartDelay = 0;
                    }
                    preparingTimeskip = true;
                }
                if (standInstance.SpecialKeyPressed(false) && mPlayer.kingCrimsonBuffIndex != -1)
                {
                    if (player.buffTime[mPlayer.kingCrimsonBuffIndex] > 10)
                    {
                        player.buffTime[mPlayer.kingCrimsonBuffIndex] = 10;
                        mPlayer.kingCrimsonBuffIndex = -1;
                    }
                }
                if (preparingTimeskip)
                {
                    timeskipStartDelay++;
                    if (timeskipStartDelay >= 80)
                    {
                        standInstance.shootCount += 15;
                        mPlayer.timeskipActive = true;
                        player.AddBuff(ModContent.BuffType<SkippingTime>(), 10 * 60);
                        SoundEngine.PlaySound(TimeskipSound);
                        SyncCall.SyncTimeskip(player.whoAmI, true);
                        timeskipStartDelay = 0;
                        preparingTimeskip = false;
                        mPlayer.kingCrimsonAbilityCooldownTime = 30;
                    }
                }
            }
            else if (currentStandType == Lock)
            {
                player.AddBuff(ModContent.BuffType<LockActiveBuff>(), 10);
            }
            else if (currentStandType == MagiciansRed)
            {
                if (standInstance.SpecialKeyPressed())
                {
                    int amountOfAnkhs = (int)(12 * standCompletionProgress);
                    for (int p = 0; p < amountOfAnkhs; p++)
                    {
                        float angle = MathHelper.ToRadians((360f / amountOfAnkhs) * p);
                        Vector2 position = player.Center + (angle.ToRotationVector2() * 48f);
                        int projIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), position - new Vector2(26f, 25f), Vector2.Zero, ModContent.ProjectileType<CrossfireHurricaneAnkh>(), newProjectileDamage * 2, 4f, owner, 24f * 16f, angle);
                        Main.projectile[projIndex].netUpdate = true;
                        Main.projectile[projIndex].timeLeft += 10 * p;
                        projectile.netUpdate = true;
                    }
                    player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(45));
                    secondRingTimer = 1;
                }
                if (secondRingTimer != 0)
                {
                    secondRingTimer++;
                    if (secondRingTimer >= 40)
                    {
                        int amountOfAnkhs = (int)(25 * standCompletionProgress);
                        for (int p = 0; p < amountOfAnkhs; p++)
                        {
                            float angle = MathHelper.ToRadians((360 / amountOfAnkhs) * p);
                            Vector2 position = player.Center + (angle.ToRotationVector2() * 48f);
                            int projIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), position - new Vector2(26f, 25f), Vector2.Zero, ModContent.ProjectileType<CrossfireHurricaneAnkh>(), newProjectileDamage, 4f, owner, 16f * 16f, -angle);
                            Main.projectile[projIndex].netUpdate = true;
                            Main.projectile[projIndex].timeLeft += 180 + (5 * p);
                            projectile.netUpdate = true;
                        }
                        secondRingTimer = 0;
                    }
                }
            }
            else if (currentStandType == SilverChariot)
            {
                if (standInstance.SpecialKeyPressed())
                {
                    shirtless = !shirtless;
                    if (shirtless)
                    {
                        if (!player.HasBuff(ModContent.BuffType<AbilityCooldown>()) && player.ownedProjectileCounts[ModContent.ProjectileType<WaywardSonAfterImage>()] == 0)
                        {
                            for (int i = 0; i < AfterImagesLimit; i++)
                            {
                                int projIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.position, Vector2.Zero, ModContent.ProjectileType<WaywardSonAfterImage>(), 0, 0f, owner, i, AfterImagesLimit);
                                Main.projectile[projIndex].netUpdate = true;
                                player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(5));
                            }
                        }
                    }
                }
            }
            else if (currentStandType == StarPlatinum)
            {
                if (standInstance.SpecialKeyPressed())
                    standInstance.Timestop((int)(4 * standCompletionProgress));
            }
            else if (currentStandType == StickyFingers)
            {
                if (standInstance.SpecialKeyPressed() && standInstance.shootCount <= 0 && !standInstance.secondaryAbility && player.ownedProjectileCounts[ModContent.ProjectileType<StickyFingersTraversalZipper>()] == 0)
                {
                    standInstance.shootCount += 20;
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = Vector2.One;

                    shootVel.Normalize();
                    shootVel *= 32f;
                    int projIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, shootVel, ModContent.ProjectileType<StickyFingersTraversalZipper>(), 0, 0f, projectile.owner, 1f);
                    Main.projectile[projIndex].netUpdate = true;
                    projectile.netUpdate = true;
                }
            }
            else if (currentStandType == StoneFree)
            {
                if (standInstance.SpecialKeyPressed())
                {
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    shootVel.Normalize();
                    shootVel *= 12f;
                    int projectileIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, shootVel, ModContent.ProjectileType<StoneFreeBindString>(), 4, 0f, player.whoAmI, projectile.whoAmI, 18);
                    Main.projectile[projectileIndex].netUpdate = true;
                    player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(5));
                }
            }
            else if (currentStandType == TheHand)
            {
                if (standInstance.SpecialKeyPressed(false))
                {
                    scrapeMode = !scrapeMode;
                    if (scrapeMode)
                        Main.NewText("Scrape Mode: Active");
                    else
                        Main.NewText("Scrape Mode: Disabled");
                }
            }
            else if (currentStandType == TheWorld)
            {
                if (standInstance.SpecialKeyPressed())
                    standInstance.Timestop((int)(9 * standCompletionProgress));
            }
            else if (currentStandType == TowerOfGray)
            {
                if (standInstance.SpecialKeyPressed())
                    mimicTowerOfGraySize = !mimicTowerOfGraySize;
                if (mimicTowerOfGraySize)
                    projectile.scale = 0.4f;
            }
            else if (currentStandType == Tusk)
            {
                if (standInstance.SpecialKeyPressed() && standOwner.ownedProjectileCounts[ModContent.ProjectileType<WaywardSonTuskWormhole>()] <= 0 && !standOwner.HasBuff(ModContent.BuffType<AbilityCooldown>()))
                {
                    SoundEngine.PlaySound(SoundID.Item78, player.Center);
                    Vector2 shootVelocity = Main.MouseWorld - standOwner.Center;
                    shootVelocity.Normalize();
                    shootVelocity *= 5f;
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), standOwner.Center, shootVelocity, ModContent.ProjectileType<WaywardSonTuskWormhole>(), 124, 8f, projectile.whoAmI);
                }
            }
            else if (currentStandType == Whitesnake)
            {
                if (standInstance.SpecialKeyCurrent() && standInstance.shootCount <= 0 && !stealingDisc)
                {
                    limitDistance = false;
                    projectile.velocity = Main.MouseWorld - projectile.position;
                    projectile.velocity.Normalize();
                    projectile.velocity *= 5f;
                    projectile.netUpdate = true;
                    float mouseDistance = Vector2.Distance(Main.MouseWorld, projectile.Center);
                    if (mouseDistance > 40f)
                        projectile.velocity = player.velocity + projectile.velocity;
                    else
                        projectile.velocity = Vector2.Zero;

                    waitingForStealEnemy = true;
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (npc.active)
                        {
                            if (projectile.Distance(npc.Center) <= 30f && !npc.immortal && !npc.hide)
                            {
                                projectile.ai[0] = npc.whoAmI;
                                stealingDisc = true;
                            }
                        }
                    }
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        for (int p = 0; p < Main.maxPlayers; p++)
                        {
                            Player otherPlayer = Main.player[p];
                            if (otherPlayer.active)
                            {
                                if (projectile.Distance(otherPlayer.Center) <= 30f & otherPlayer.team != player.team && otherPlayer.whoAmI != player.whoAmI)
                                {
                                    projectile.ai[1] = otherPlayer.whoAmI;
                                    stealingDisc = true;
                                }
                            }
                        }
                    }
                }
                if (stealingDisc && projectile.ai[0] != -1f)
                {
                    limitDistance = false;
                    projectile.velocity = Vector2.Zero;
                    NPC npc = Main.npc[(int)projectile.ai[0]];
                    npc.direction = -projectile.direction;
                    npc.position = projectile.position + new Vector2(-6f * projectile.direction, -2f - npc.height / 3f);
                    npc.velocity = Vector2.Zero;
                    npc.AddBuff(ModContent.BuffType<Stolen>(), 30 * 60);
                    npc.GetGlobalNPC<JoJoGlobalNPC>().whitesnakeDISCImmune += 1;
                    SyncCall.SyncStandEffectInfo(player.whoAmI, npc.whoAmI, 9);
                    stealingDisc = false;
                    projectile.ai[0] = -1f;
                    standInstance.shootCount += 60;
                    if (!npc.active)
                    {
                        stealingDisc = false;
                        projectile.ai[0] = -1f;
                        standInstance.shootCount += 30;
                    }
                }
                if (stealingDisc && projectile.ai[1] != -1f)
                {
                    limitDistance = false;
                    projectile.velocity = Vector2.Zero;
                    Player otherPlayer = Main.player[(int)projectile.ai[1]];
                    otherPlayer.direction = -projectile.direction;
                    otherPlayer.position = projectile.position + new Vector2(-6f * projectile.direction, -2f - otherPlayer.height / 3f);
                    otherPlayer.velocity = Vector2.Zero;
                    otherPlayer.AddBuff(ModContent.BuffType<Stolen>(), 30 * 60);
                    SyncCall.SyncOtherPlayerDebuff(player.whoAmI, otherPlayer.whoAmI, ModContent.BuffType<Stolen>(), 30 * 60);
                    stealingDisc = false;
                    projectile.ai[1] = -1f;
                    standInstance.shootCount += 60;
                    if (!otherPlayer.active)
                    {
                        stealingDisc = false;
                        projectile.ai[1] = -1f;
                        standInstance.shootCount += 30;
                    }
                }
                if (!standInstance.SpecialKeyCurrent() && stealingDisc || !standInstance.SpecialKeyCurrent() && waitingForStealEnemy || projectile.ai[1] != -1f)
                {
                    stealingDisc = false;
                    waitingForStealEnemy = false;
                    projectile.ai[0] = -1f;
                    projectile.ai[1] = -1f;
                    standInstance.shootCount += 30;
                }
            }
            else if (currentStandType == Blur)
            {
                if (standInstance.SpecialKeyCurrent())
                    player.AddBuff(ModContent.BuffType<LightningFastReflex>(), (int)(8 * 60 * (standTier / 4f)));
            }
            else if (currentStandType == CoolOut)
            {
                if (standInstance.SpecialKeyPressed() && standInstance.shootCount <= 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item28, projectile.position);
                    int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center.X, projectile.Center.Y + 16f, 0f, 0f, ModContent.ProjectileType<IceSpike>(), standInstance.newPunchDamage / 4, 2f, Main.myPlayer, projectile.whoAmI, projectile.direction);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                    player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(10));
                }
            }
            else if (currentStandType == Expanses)
            {
                IsCrystallized = !IsCrystallized;
                if (!IsCrystallized) { player.ClearBuff(ModContent.BuffType<SelfCrystallize>()); }
                else { player.AddBuff(ModContent.BuffType<SelfCrystallize>(), 16000, true, false); }
            }
            else if (currentStandType == FollowMe)
            {
                if (standInstance.SpecialKeyPressed() && !player.HasBuff(ModContent.BuffType<Intangible>()))
                    player.AddBuff(ModContent.BuffType<Intangible>(), 7200);
            }
            else if (currentStandType == TheWorldOverHeaven)
            {
                if (standInstance.SpecialKeyPressed())
                    standInstance.Timestop((int)(18 * standCompletionProgress));
            }
            else if (currentStandType == Metempsychosis)
            {
                player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(30 / standTier));
                if (Main.myPlayer == projectile.owner)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (npc.CanBeChasedBy(this) && projectile.Distance(npc.Center) <= standInstance.newMaxDistance * 4)
                        {
                            int newHealth = (int)(npc.life * (1f - (0.05f * standTier))) - npc.defense;
                            if (npc.boss || newHealth > 0.25f * npc.lifeMax)
                            {
                                int damage = npc.life - newHealth;
                                NPC.HitInfo hitInfo = new NPC.HitInfo()
                                {
                                    Damage = damage,
                                    Knockback = 0f,
                                    HitDirection = -projectile.direction,
                                };
                                npc.StrikeNPC(hitInfo);
                                NetMessage.SendStrikeNPC(npc, hitInfo, projectile.owner);
                            }
                            else
                            {
                                npc.StrikeInstantKill();
                                Projectile.NewProjectile(projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<SoulEffect>(), 0, 0f, projectile.owner, player.statLifeMax2 * 0.05f);
                            }
                        }
                    }
                }
            }
        }

        public void StandEffects()
        {
            if (currentStandType == BadCompany)
                canAttack = canDraw = false;
            else if (currentStandType == Whitesnake)
            {
                if (standInstance != null && standInstance.SpecialKeyCurrent() && standInstance.shootCount <= 0 && !stealingDisc)
                    limitDistance = false;
            }
            else if (currentStandType == LucyInTheSky)
            {
                Vector3 lightLevel = Lighting.GetColor((int)standOwner.Center.X / 16, (int)standOwner.Center.Y / 16).ToVector3();       //1.703 is max light
                if (lightLevel.Length() > 1.3f)
                    standOwner.statDefense += (int)(standOwner.statDefense * 0.5f);
            }
        }

        public bool OverrideMainAttack(ref int currentAnimationState)
        {
            Projectile projectile = standInstance.Projectile;
            if (currentStandType == HierophantGreen)
            {
                if (standInstance.shootCount <= 0)
                {
                    if (attacksLeftBeforeProjectile <= 0)
                    {
                        attacksLeftBeforeProjectile = (int)(2 / standCompletionProgress);
                        int direction = Main.MouseWorld.X > standOwner.Center.X ? 1 : -1;
                        Vector2 shootVel = Main.MouseWorld - standInstance.Projectile.Center;
                        if (shootVel == Vector2.Zero)
                            shootVel = new Vector2(0f, 1f);

                        shootVel.Normalize();
                        shootVel *= 16f;
                        float numberProjectiles = standTier + 1;
                        float rotation = MathHelper.ToRadians(30);
                        float randomSpeedOffset = (100f + Main.rand.NextFloat(-6f, 6f)) / 100f;
                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            Vector2 perturbedSpeed = shootVel.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                            perturbedSpeed *= randomSpeedOffset;
                            int projIndex = Projectile.NewProjectile(standInstance.Projectile.GetSource_FromThis(), standInstance.Projectile.Center, perturbedSpeed, ModContent.ProjectileType<Emerald>(), (int)(standInstance.newPunchDamage / 4 * standCompletionProgress), 3f, standOwner.whoAmI);
                            Main.projectile[projIndex].netUpdate = true;
                        }
                        SoundEngine.PlaySound(SoundID.Item21, standInstance.Projectile.position);
                        standInstance.Projectile.netUpdate = true;
                        if (standOwner.velocity.X == 0f)
                            standOwner.ChangeDir(direction);
                    }
                    else
                        attacksLeftBeforeProjectile--;
                }
            }
            else if (currentStandType == KillerQueenBTD)
            {
                if (attacksLeftBeforeProjectile <= 0)
                {
                    attacksLeftBeforeProjectile = (int)(4 / standCompletionProgress);
                    standInstance.attacking = true;
                    currentAnimationState = 1;
                    standInstance.Projectile.netUpdate = true;
                    if (standOwner.GetModPlayer<MyPlayer>().standControlStyle == MyPlayer.StandControlStyle.Manual)
                    {
                        if (standInstance.shootCount <= 0)
                        {
                            standInstance.shootCount += standInstance.newShootTime * 3;
                            Vector2 shootVel = Main.MouseWorld - standInstance.Projectile.Center;
                            if (shootVel == Vector2.Zero)
                                shootVel = new Vector2(0f, 1f);

                            shootVel.Normalize();
                            shootVel *= 4f;
                            int projIndex = Projectile.NewProjectile(standInstance.Projectile.GetSource_FromThis(), standInstance.Projectile.Center, shootVel, ModContent.ProjectileType<ExplosiveBubble>(), standInstance.newPunchDamage * 2, 6f, standInstance.Projectile.owner, 1f, standOwner.whoAmI);
                            Main.projectile[projIndex].netUpdate = true;
                            standInstance.Projectile.netUpdate = true;
                        }
                    }
                    return true;
                }
                else
                    attacksLeftBeforeProjectile--;
            }
            else if (currentStandType == SexPistols)
            {
                if (standInstance.shootCount <= 0)
                {
                    int direction = Main.MouseWorld.X > standOwner.Center.X ? 1 : -1;
                    Vector2 shootVel = Main.MouseWorld - standInstance.Projectile.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = new Vector2(0f, 1f);

                    shootVel.Normalize();
                    shootVel *= 16f;
                    float numberProjectiles = 6;
                    float rotation = MathHelper.ToRadians(30);
                    float randomSpeedOffset = (100f + Main.rand.NextFloat(-6f, 6f)) / 100f;
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = shootVel.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                        perturbedSpeed *= randomSpeedOffset;
                        int projIndex = Projectile.NewProjectile(standInstance.Projectile.GetSource_FromThis(), standInstance.Projectile.Center, perturbedSpeed, ProjectileID.Bullet, standInstance.newProjectileDamage, 3f, standOwner.whoAmI);
                        Main.projectile[projIndex].netUpdate = true;
                        Main.projectile[projIndex].GetGlobalProjectile<JoJoGlobalProjectile>().autoModeSexPistols = true;
                    }
                    SoundEngine.PlaySound(SoundID.Item21, standInstance.Projectile.position);
                    standInstance.Projectile.netUpdate = true;
                    if (standOwner.velocity.X == 0f)
                        standOwner.ChangeDir(direction);
                }
                return false;
            }
            else if (currentStandType == SoftAndWet)
            {
                if (Main.rand.NextBool((int)(18 / standCompletionProgress)))
                {
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = new Vector2(0f, 1f);

                    shootVel.Normalize();
                    shootVel *= 3f;
                    Vector2 bubbleSpawnPosition = projectile.Center + new Vector2(Main.rand.Next(0, 18 + 1) * projectile.direction, -Main.rand.Next(0, standInstance.HalfStandHeight - 2 + 1));
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), bubbleSpawnPosition, shootVel, ModContent.ProjectileType<TinyBubble>(), 8 * standTier, 2f, projectile.owner, projectile.whoAmI);
                    SoundEngine.PlaySound(SoundID.Drip, projectile.Center);
                }
                if (Main.rand.NextBool((int)(12 / standCompletionProgress)))
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        int BubbleSpawnRadius = 8 * 16;
                        Vector2 bubbleSpawnPosition = projectile.Center + new Vector2(Main.rand.Next(-(int)BubbleSpawnRadius, (int)BubbleSpawnRadius), Main.rand.Next(-(int)BubbleSpawnRadius, (int)BubbleSpawnRadius));
                        if (Vector2.Distance(bubbleSpawnPosition, standOwner.Center) > BubbleSpawnRadius)
                            bubbleSpawnPosition = projectile.Center + new Vector2(Main.rand.Next(-(int)BubbleSpawnRadius, (int)BubbleSpawnRadius) / 2, Main.rand.Next(-(int)BubbleSpawnRadius, (int)BubbleSpawnRadius) / 2);

                        Point bubbleSpawnPoint = (bubbleSpawnPosition / 16f).ToPoint();
                        bubbleSpawnPoint.X = Math.Clamp(bubbleSpawnPoint.X, 0, Main.maxTilesX - 1);
                        bubbleSpawnPoint.Y = Math.Clamp(bubbleSpawnPoint.Y, 0, Main.maxTilesY - 1);
                        if (Main.tile[bubbleSpawnPoint.X, bubbleSpawnPoint.Y].HasTile)
                        {
                            int attempts = 0;
                            while (Main.tile[bubbleSpawnPoint.X, bubbleSpawnPoint.Y].HasTile && attempts < 5)
                            {
                                attempts++;
                                bubbleSpawnPoint.Y -= 2;
                                bubbleSpawnPosition.Y -= 2 * 16;
                            }
                        }

                        Vector2 bubbleVelocity = new Vector2(0f, -Main.rand.Next(12, 24) / 10f);
                        int projIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), bubbleSpawnPosition, bubbleVelocity, ModContent.ProjectileType<ControllableBubble>(), (int)(standInstance.newPunchDamage / 4 * standOwner.GetModPlayer<MyPlayer>().standDamageBoosts * 0.9f), 2f, projectile.owner);
                        Main.projectile[projIndex].netUpdate = true;
                    }
                }
            }
            else if (currentStandType == Tusk)
            {
                if (nailShootCooldown <= 0)
                {
                    nailShootCooldown += 5 * 60 / standTier;
                    if (Main.MouseWorld.X > projectile.position.X)
                        projectile.direction = 1;
                    else
                        projectile.direction = -1;
                    SoundStyle shootSound = SoundID.Item67;
                    shootSound.Volume = 0.33f;
                    SoundEngine.PlaySound(shootSound, projectile.Center);
                    Vector2 shootVelocity = Main.MouseWorld - projectile.Center;
                    shootVelocity.Normalize();
                    shootVelocity *= 4f;
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, shootVelocity, ModContent.ProjectileType<WaywardSonControllableNail>(), (int)(49 * (standTier / 4f)), 5f, standOwner.whoAmI);
                }
            }
            else if (currentStandType == BackInBlack)
            {
                if (Main.rand.NextBool((int)(8 + (2 * standCompletionProgress))))
                {
                    SoundEngine.PlaySound(SoundID.Item78, projectile.position);
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = new Vector2(0f, 1f);

                    shootVel.Normalize();
                    shootVel *= 1.5f;
                    Vector2 perturbedSpeed = new Vector2(shootVel.X, shootVel.Y);
                    int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center + new Vector2(4f), perturbedSpeed, ModContent.ProjectileType<BackInBlackOrb>(), standInstance.newPunchDamage / 4, 2f, standOwner.whoAmI);
                    Main.projectile[proj].netUpdate = true;
                }
            }
            else if (currentStandType == Banks)
            {
                shotgunChargeTimer++;
                if (shotgunChargeTimer >= 60)
                {
                    shotgunChargeTimer = 0;
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    shootVel.Normalize();
                    shootVel *= 16f;

                    float numberProjectiles = 6;
                    float rotation = MathHelper.ToRadians(30f);
                    float random = Main.rand.NextFloat(-6f, 6f + 1f);
                    for (int i = 0; i < numberProjectiles; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(shootVel.X + random, shootVel.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f;
                        int proj = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, perturbedSpeed, ProjectileID.Bullet, standInstance.newPunchDamage / 8, 1f, standOwner.whoAmI);
                        Main.projectile[proj].netUpdate = true;
                    }
                    SoundEngine.PlaySound(SoundID.Item36, projectile.position);
                }
            }
            else if (currentStandType == CoolOut)
            {
                if (attacksLeftBeforeProjectile <= 0)
                {
                    attacksLeftBeforeProjectile = (int)(6 / standCompletionProgress);
                    int projIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center.X, projectile.Center.Y - 10f, 0f, 0f, ModContent.ProjectileType<IceSpear>(), standInstance.newPunchDamage / 2, 10f, Main.myPlayer, projectile.whoAmI);
                    Main.projectile[projIndex].netUpdate = true;
                }
                else
                    attacksLeftBeforeProjectile--;
            }
            else if (currentStandType == RoseColoredBoy)
            {
                if (Main.rand.NextBool(6 * (2 - (standTier / 2))))
                {
                    Vector2 shootVel = Main.MouseWorld - projectile.position;
                    shootVel.Normalize();
                    shootVel *= 12f;
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center + new Vector2(28f * projectile.direction, -8f), shootVel, ModContent.ProjectileType<RosePetal>(), standInstance.newPunchDamage / 4, 6f, standOwner.whoAmI);
                }
                Dust.NewDust(projectile.Center + new Vector2(28f * projectile.direction, -8f), 2, 2, DustID.Torch);
            }
            return true;
        }


        /// <summary>
        /// Gets called at the end of Scimitar Slash.
        /// </summary>
        /// <param name="projectile">Wayward son's projectile instance.</param>
        public void OnSecondaryUse(Projectile projectile)
        {
            if (currentStandType == Aerosmith && standTier > 3)
            {
                standInstance.shootCount += standInstance.newShootTime;
                projectile.frame = 2;
                int projIndex = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity, ModContent.ProjectileType<AerosmithBomb>(), 0, 3f, projectile.owner, 568 * (float)standOwner.GetModPlayer<MyPlayer>().standDamageBoosts);
                Main.projectile[projIndex].netUpdate = true;
                standOwner.AddBuff(ModContent.BuffType<AbilityCooldown>(), standOwner.GetModPlayer<MyPlayer>().AbilityCooldownTime(5));
            }
        }

        /// <summary>
        /// Modifies the attack made by Scimitar Slash. Called when ModifyHitNPC is called.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="modifiers"></param>
        public void AttackModifiers(NPC target, ref NPC.HitModifiers modifiers)
        {
            CloneDamageChanges(currentStandType, standCompletionProgress, ref modifiers);
            //MyPlayer mPlayer = Main.player[standInstance.Projectile.owner].GetModPlayer<MyPlayer>();
            //FanPlayer fPlayer = Main.player[standInstance.Projectile.owner].GetModPlayer<FanPlayer>();
            if (currentStandType == MagiciansRed && Main.rand.Next(1, 100 + 1) <= 5)
            {
                target.AddBuff(BuffID.OnFire, 5 * 60);
            }
            else if (currentStandType == GoldExperience)
            {
                if (standTier == 3)
                    target.AddBuff(ModContent.BuffType<LifePunch>(), 4 * 60);
                else if (standTier == 4)
                    target.AddBuff(ModContent.BuffType<LifePunch>(), 6 * 60);
            }
            else if (currentStandType == GratefulDead)
            {
                target.GetGlobalNPC<JoJoGlobalNPC>().standDebuffEffectOwner = standOwner.whoAmI;
                SyncCall.SyncStandEffectInfo(standOwner.whoAmI, target.whoAmI, 8, standOwner.whoAmI);
                target.AddBuff(ModContent.BuffType<Aging>(), (7 + ((int)standTier * 2)) * 60);
            }
            else if (currentStandType == StickyFingers)
            {
                target.GetGlobalNPC<JoJoGlobalNPC>().standDebuffEffectOwner = standOwner.whoAmI;
                target.AddBuff(ModContent.BuffType<Zipped>(), (2 * (int)standTier) * 60);
            }
            else if (currentStandType == TheWorldOverHeaven)
            {
                if (Main.rand.NextBool(42))
                {
                    target.AddBuff(ModContent.BuffType<RealityRewriteBuff>(), 30 * 60);
                    int amountOfDusts = Main.rand.Next(32, 48);
                    for (int i = 0; i < amountOfDusts; i++)
                    {
                        float angle = (i / (float)amountOfDusts) * (float)Math.PI * 2f;
                        Vector2 dustVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 3.8f;
                        int dustIndex = Dust.NewDust(target.Center, 1, 1, DustID.WhiteTorch, Scale: 1.6f);
                        Main.dust[dustIndex].velocity = dustVelocity;
                        Main.dust[dustIndex].noGravity = true;
                        Main.dust[dustIndex].fadeIn = 2f;
                        if (Main.rand.Next(0, 7 + 1) != 0)
                            Main.dust[dustIndex].noLight = true;
                    }
                }
            }
        }

        public void CloneDamageChanges(int firstStandType, float standCompletion, ref NPC.HitModifiers modifiers)
        {
            if (firstStandType == -1)
                return;

            Player player = Main.player[standInstance.Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            float newDamage = standInstance.newPunchDamage;
            if (firstStandType == StarPlatinum)
                modifiers.FinalDamage *= 2 * (standCompletion + 1f);
            else if (firstStandType == TheWorld)
                modifiers.FinalDamage *= 2 * (standCompletion + 1f);
            else if (firstStandType == CrazyDiamond)
                modifiers.FinalDamage *= 1.5f * (standCompletion + 1f);
            else if (firstStandType == Echoes && standCompletion == 0.25f)
                modifiers.FinalDamage *= 0.5f;
            else if (firstStandType == KingCrimson)
                modifiers.FinalDamage *= 2f * (standCompletion + 1f);
            else if (firstStandType == StoneFree)
                modifiers.FinalDamage *= 1.5f * (standCompletion + 1f);
        }

        public int CloneSpeedChanges(int firstStandType, float standCompletion)
        {
            if (firstStandType == -1)
                return standInstance.newPunchTime;

            Player player = Main.player[standInstance.Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            float punchTime = standInstance.newPunchTime;
            if (firstStandType == SilverChariot)
                punchTime *= 0.5f * (standCompletion + 1f);
            else if (firstStandType == MortalReminder)
                punchTime *= 0.5f * (standCompletion + 1f);
            return (int)punchTime;
        }

        /// <summary>
        /// Effects of the attack.
        /// </summary>
        /// <param name="projectile">The first projectile</param>
        public void AttackEffects(Projectile projectile)
        { }

        /// <summary>
        /// Effects of the attack.
        /// </summary>
        /// <param name="projectile">The first projectile</param>
        public void WhirlwindAttackEffects(Projectile projectile, float vacuumRange)
        {
            int amountOfDusts = Main.rand.Next(1, 2 + 1);
            for (int i = 0; i < amountOfDusts; i++)
            {
                float xOffset = Main.rand.Next(0, (int)vacuumRange + 1) * projectile.direction;
                float yOffset = (Main.rand.Next(-100, 100 + 1) / 100f) * (xOffset / vacuumRange) * (vacuumRange * 3f / 4f);       //Cone-like shape
                Vector2 dustPosition = projectile.Center + new Vector2(xOffset, yOffset);
                Vector2 dustVelocity = -new Vector2(xOffset, yOffset);
                dustVelocity.Normalize();
                dustVelocity *= 3f * (Main.rand.Next(80, 120 + 1) / 100f);
                int dustType = DustID.Cloud;
                if (currentStandType == MagiciansRed || currentStandType == SlavesOfFear)
                    dustType = DustID.Torch;
                int dustIndex = Dust.NewDust(dustPosition, 2, 2, dustType);
                Main.dust[dustIndex].velocity = dustVelocity;
                Main.dust[dustIndex].noGravity = true;
            }
        }

        private int CalculateRewindTime()
        {
            int rewindTime = 0;
            int currentTimeAmount = 35;
            for (int i = 0; i < amountOfSavedData; i++)
            {
                rewindTime += currentTimeAmount;
                currentTimeAmount = (int)(currentTimeAmount * 0.8);
            }
            return rewindTime;
        }
    }
}