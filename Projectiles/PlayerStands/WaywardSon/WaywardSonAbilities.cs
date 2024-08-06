using JoJoStands.Buffs.EffectBuff;
using JoJoStands;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria;
using JoJoStands.Projectiles.PlayerStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles;
using Microsoft.Xna.Framework;
using JoJoStands.Projectiles.PlayerStands.SilverChariot;
using Terraria.ID;
using JoJoStands.Buffs.ItemBuff;
using JoJoStands.Projectiles.Minions;
using JoJoStands.DataStructures;
using JoJoStands.NPCs;
using Terraria.Localization;
using JoJoStands.Projectiles.PlayerStands.CrazyDiamond;

namespace JoJoFanStands.Projectiles.PlayerStands.WaywardSon
{
    public class WaywardSonAbilities
    {
        public int standTier;
        public float standCompletionProgress;
        public int owner;
        public StandClass standInstance;

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

        public WaywardSonAbilities(int standTier)
        {
            this.standTier = standTier;
            standCompletionProgress = standTier / 4f;
        }

        private int secondRingTimer = 0;
        private int generalAbilityTimer = 0;

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

        public static readonly SoundStyle ScrapeSoundEffect = new SoundStyle("JoJoStands/Sounds/GameSounds/BRRR")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };

        public void ManageAbilities(int targetStandType, Player player, Projectile projectile)
        {
            standInstance = projectile.ModProjectile as StandClass;
            int newProjectileDamage = standInstance.newPunchDamage;
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            if (targetStandType == Aerosmith)
            {

            }
            else if (targetStandType == BadCompany)
            {

            }
            else if (targetStandType == CenturyBoy)
            {

            }
            else if (targetStandType == CrazyDiamond)
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
            else if (targetStandType == Cream)
            {
                if (standInstance.SpecialKeyPressed() && player.ownedProjectileCounts[ModContent.ProjectileType<WaywardSonVoid>()] <= 0)
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), player.Top, player.velocity, ModContent.ProjectileType<WaywardSonVoid>(), (int)((standInstance.PunchDamage * 0.5f) * mPlayer.standDamageBoosts), 6f, player.whoAmI);
            }
            else if (targetStandType == DollyDagger)
            {

            }
            else if (targetStandType == Echoes)
            {

            }
            else if (targetStandType == GoldExperience)
            {

            }
            else if (targetStandType == GratefulDead)
            {

            }
            else if (targetStandType == HermitPurple)
            {
                if (standInstance.SpecialKeyPressed() && player.ownedProjectileCounts[ModContent.ProjectileType<HermitPurpleHook>()] <= 0)
                {
                    Vector2 shootVelocity = Main.MouseWorld - player.Center;
                    shootVelocity.Normalize();
                    shootVelocity *= 12f;
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, shootVelocity, ModContent.ProjectileType<HermitPurpleHook>(), 0, 0f, player.whoAmI);
                }
            }
            else if (targetStandType == HierophantGreen)
            {
                if (standInstance.SpecialKeyPressed() && player.ownedProjectileCounts[ModContent.ProjectileType<EmeraldStringPointConnector>()] <= 0 && !spawningField)
                {
                    spawningField = true;
                    formPosition = projectile.position;
                }
                if (spawningField && owner == Main.myPlayer)
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
            else if (targetStandType == KillerQueen)
            {
                if (standInstance.SpecialKeyPressed() && player.ownedProjectileCounts[ModContent.ProjectileType<SheerHeartAttack>()] == 0)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.position + new Vector2(10f * projectile.direction, 0f), projectile.velocity, ModContent.ProjectileType<SheerHeartAttack>(), 1, 0f, projectile.owner, 1f);
                }
            }
            else if (targetStandType == KillerQueenBTD)
            {

            }
            else if (targetStandType == KingCrimson)
            {

            }
            else if (targetStandType == Lock)
            {
                player.AddBuff(ModContent.BuffType<LockActiveBuff>(), 10);
            }
            else if (targetStandType == MagiciansRed)
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
            else if (targetStandType == SexPistols)
            {

            }
            else if (targetStandType == SilverChariot)
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
            else if (targetStandType == SoftAndWet)
            {

            }
            else if (targetStandType == StarPlatinum)
            {
                if (standInstance.SpecialKeyPressed())
                    standInstance.Timestop((int)(4 * standCompletionProgress));
            }
            else if (targetStandType == StickyFingers)
            {

            }
            else if (targetStandType == StoneFree)
            {

            }
            else if (targetStandType == TheHand)
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
            else if (targetStandType == TheWorld)
            {
                if (standInstance.SpecialKeyPressed())
                    standInstance.Timestop((int)(9 * standCompletionProgress));
            }
            else if (targetStandType == TowerOfGray)
            {
                if (standInstance.SpecialKeyPressed())
                {
                    mimicTowerOfGraySize = !mimicTowerOfGraySize;
                }
                if (mimicTowerOfGraySize)
                {
                    projectile.scale = 0.4f;
                }
            }
            else if (targetStandType == Tusk)
            {

            }
            else if (targetStandType == Whitesnake)
            {

            }
            else if (targetStandType == BackInBlack)
            {

            }
            else if (targetStandType == Banks)
            {

            }
            else if (targetStandType == Blur)
            {

            }
            else if (targetStandType == CoolOut)
            {

            }
            else if (targetStandType == Expanses)
            {

            }
            else if (targetStandType == FollowMe)
            {

            }
            else if (targetStandType == LucyInTheSky)
            {

            }
            else if (targetStandType == Megalovania)
            {

            }
            else if (targetStandType == MortalReminder)
            {

            }
            else if (targetStandType == RoseColoredBoy)
            {

            }
            else if (targetStandType == SlavesOfFear)
            {

            }
            else if (targetStandType == TheFates)
            {

            }
            else if (targetStandType == TheWorldOverHeaven)
            {
                if (standInstance.SpecialKeyPressed())
                    standInstance.Timestop((int)(18 * standCompletionProgress));
            }
            else if (targetStandType == WaywardSon)
            {

            }
        }
    }
}