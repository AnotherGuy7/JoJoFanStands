using JoJoFanStands.Items.Stands;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Items;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.WaywardSon
{
    public class WaywardSonStandFinal : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 85;
        public override int AltDamage => 99;
        public override int PunchTime => 10;
        public override int TierNumber => 4;
        public override bool CanUseAfterImagePunches => false;
        public override int FistID => FanStandFists.WaywardSonFists;
        public override Vector2 StandOffset => new Vector2(-12, 0f);
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;
        public WaywardSonAbilities standAbilities;

        private const float RemoteControlMaxDistance = 60f * 16f;
        private const float AttackVacuumRange = 14f * 16f;
        private const float AttackVacuumForce = 0.3f;

        public bool canAttack = true;
        public bool canDraw = true;
        public bool limitDistance = true;

        //private readonly Point HeadParticlePoint = new Point(37 - 6, 11 - 4);
        //private readonly Point ArmParticlePoint = new Point(18, 42);
        //private readonly Point LegParticlePoint = new Point(40, 67);

        public new enum AnimationState
        {
            Idle,
            Attack,
            SecondaryAbility,
            SecondaryAbilityStab,
            Special,
            Pose
        }

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

        private Dictionary<string, byte> StandTypes = new Dictionary<string, byte>()
        {
            { "Aerosmith", Aerosmith },
            { "BadCompany", BadCompany },
            { "CenturyBoy", CenturyBoy },
            { "CrazyDiamond", CrazyDiamond },
            { "Cream", Cream },
            { "DollyDagger", DollyDagger },
            { "Echoes", Echoes },
            { "GoldExperience", GoldExperience },
            { "GoldExperienceRequiem", GoldExperienceRequiem },
            { "GratefulDead", GratefulDead },
            { "HermitPurple", HermitPurple },
            { "HierophantGreen", HierophantGreen },
            { "KillerQueen", KillerQueen },
            { "KillerQueenBTD", KillerQueenBTD },
            { "KingCrimson", KingCrimson },
            { "Lock", Lock },
            { "MagiciansRed", MagiciansRed },
            { "SexPistols", SexPistols },
            { "SilverChariot", SilverChariot },
            { "SoftAndWet", SoftAndWet },
            { "StarPlatinum", StarPlatinum },
            { "StickyFingers", StickyFingers },
            { "StoneFree", StoneFree },
            { "TheHand", TheHand },
            { "TheWorld", TheWorld },
            { "TowerOfGray", TowerOfGray },
            { "TuskAct1", Tusk },
            { "TuskAct2", Tusk },
            { "TuskAct3", Tusk },
            { "TuskAct4", Tusk },
            { "Whitesnake", Whitesnake },
            { "BackInBlack", BackInBlack },
            { "Banks", Banks },
            { "Blur", Blur },
            { "CoolOut", CoolOut },
            { "Expanses", Expanses },
            { "FollowMe", FollowMe },
            { "LucyInTheSky", LucyInTheSky },
            { "Megalovania", Megalovania },
            { "MortalReminder", MortalReminder },
            { "RoseColoredBoy", RoseColoredBoy },
            { "SlavesOfFear", SlavesOfFear },
            { "TheFates", TheFates },
            { "TheWorldOverHeaven", TheWorldOverHeaven },
            { "WaywardSon", WaywardSon }
        };

        private Vector2 secondaryDirection;
        private bool secondaryAbilityStab = false;
        private int stabNPCTarget = -1;
        private float floatTimer = 0;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            Projectile.frameCounter++;
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            int highestDamage = PunchDamage;
            for (int i = 0; i < player.inventory.Length; i++)
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

            int firstStandType = -1;
            float standCompletion = 0f;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i] != null)
                {
                    if (player.inventory[i].ModItem is FanStandItemClass)
                    {
                        firstStandType = StandTypes[(player.inventory[i].ModItem as FanStandItemClass).StandIdentifierName];
                        standCompletion = (player.inventory[i].ModItem as FanStandItemClass).StandTier / 4f;
                        break;
                    }
                    else if (player.inventory[i].ModItem is StandItemClass)
                    {
                        firstStandType = StandTypes[(player.inventory[i].ModItem as StandItemClass).StandIdentifierName];
                        standCompletion = (player.inventory[i].ModItem as StandItemClass).StandTier / 4f;
                        break;
                    }
                }
            }
            if (firstStandType == Megalovania)
            {
                StayBehind();
                return;
            }

            canAttack = true;
            canDraw = true;
            if (standAbilities == null)
            {
                standAbilities = new WaywardSonAbilities(mPlayer.standTier);
                standAbilities.UpdateInformation(firstStandType, player);
            }
            else
            {
                standAbilities.UpdateInformation(firstStandType, player);
                canAttack = standAbilities.canAttack;
                canDraw = standAbilities.canDraw;
                limitDistance = standAbilities.limitDistance;
            }

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft && canAttack)
                    {
                        int curAnimationState = (int)currentAnimationState;
                        if (standAbilities.OverrideMainAttack(ref curAnimationState))
                        {
                            int punchIndex = Punch(ModContent.ProjectileType<FanStandFists>(), new Vector2(mouseX, mouseY), afterImages: false);
                            if (punchIndex != -1)
                            {
                                (Main.projectile[punchIndex].ModProjectile as FanStandFists).standInstance = Projectile.ModProjectile;
                                shootCount = standAbilities.CloneSpeedChanges(firstStandType, standCompletion);
                            }

                            currentAnimationState = AnimationState.Attack;
                            for (int n = 0; n < Main.maxNPCs; n++)
                            {
                                if (Main.npc[n].active)
                                {
                                    NPC npc = Main.npc[n];
                                    bool directionCheck = Projectile.direction == 1 ? npc.Center.X > Projectile.Center.X : npc.Center.X < Projectile.Center.X;
                                    float npcDistance = Vector2.Distance(npc.Center, Projectile.Center);
                                    if (directionCheck && npcDistance < AttackVacuumRange)
                                    {
                                        Vector2 direction = Projectile.Center - npc.Center;
                                        direction.Normalize();
                                        direction *= AttackVacuumForce;
                                        npc.velocity += direction;
                                    }
                                }
                            }
                            standAbilities.WhirlwindAttackEffects(Projectile, AttackVacuumRange);
                            AttackClone(firstStandType, standCompletion);
                        }
                        else
                            currentAnimationState = (AnimationState)curAnimationState;
                    }
                    else
                    {
                        attacking = false;
                        if (!secondaryAbility)
                        {
                            if (limitDistance)
                                StayBehind();
                            currentAnimationState = AnimationState.Idle;
                        }
                    }
                    if (Main.mouseRight && !playerHasAbilityCooldown && !secondaryAbility)
                    {
                        stabNPCTarget = -1;
                        secondaryAbility = true;
                        secondaryAbilityStab = false;
                        secondaryDirection = Main.MouseWorld - Projectile.Center;
                        secondaryDirection.Normalize();
                        secondaryDirection *= 12f;
                        Projectile.netUpdate = true;
                    }
                    standAbilities.ManageAbilities(Projectile);
                }

                float playerDistance = (player.Center - Projectile.Center).Length();
                if (secondaryAbility)
                {
                    if (!secondaryAbilityStab)
                    {
                        currentAnimationState = AnimationState.SecondaryAbility;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.velocity = secondaryDirection;
                            Projectile.direction = 1;
                            if (Projectile.velocity.X < 0)
                                Projectile.direction = -1;
                        }

                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.CanBeChasedBy(this) && Projectile.Distance(npc.Center) <= 3f * 16f)
                            {
                                if (Projectile.owner == Main.myPlayer)
                                {
                                    stabNPCTarget = n;
                                    secondaryAbilityStab = true;
                                    Projectile.frame = 0;
                                    Projectile.frameCounter = 0;
                                    Projectile.netUpdate = true;
                                    break;
                                }
                            }
                        }
                        if (playerDistance > newMaxDistance * 3)
                        {
                            secondaryAbility = false;
                            standAbilities.OnSecondaryUse(Projectile);
                            Projectile.netUpdate = true;
                        }
                    }
                    else
                    {
                        Projectile.velocity = Vector2.Zero;
                        currentAnimationState = AnimationState.SecondaryAbilityStab;
                        NPC npc = Main.npc[stabNPCTarget];
                        if (!npc.active || npc.life <= 0 || Projectile.frame >= 3)
                        {
                            stabNPCTarget = -1;
                            secondaryAbility = false;
                            secondaryAbilityStab = false;
                            player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(2));
                            standAbilities.OnSecondaryUse(Projectile);
                            Projectile.netUpdate = true;
                            return;
                        }

                        if (Projectile.owner == Main.myPlayer && npc.active && (Projectile.frame == 0 || Projectile.frame == 2) && shootCount <= 0)     //hit frames
                        {
                            shootCount += 4;
                            NPC.HitInfo hitInfo = new NPC.HitInfo()
                            {
                                Damage = newAltDamage,
                                Knockback = 1f,
                                HitDirection = Projectile.direction,
                                Crit = true
                            };
                            npc.StrikeNPC(hitInfo);
                            NetMessage.SendStrikeNPC(npc, hitInfo, Projectile.owner);
                            SoundEngine.PlaySound(SoundID.Item2);
                        }
                    }
                }
                else
                {
                    LimitDistance();
                }

                if (mPlayer.posing)
                    currentAnimationState = AnimationState.Pose;

                if (SecondSpecialKeyPressed(false) && shootCount <= 0)
                {
                    secondaryAbility = false;
                    Projectile.frame = 0;
                    Projectile.frameCounter = 0;
                    mPlayer.standControlStyle = MyPlayer.StandControlStyle.Remote;
                }
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                BasicPunchAI();
                if (!attacking)
                    currentAnimationState = AnimationState.Idle;
                else
                    currentAnimationState = AnimationState.Attack;
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Remote)
            {
                float halfScreenWidth = (float)Main.screenWidth / 2f;
                float halfScreenHeight = (float)Main.screenHeight / 2f;
                mPlayer.standRemoteModeCameraPosition = Projectile.Center - new Vector2(halfScreenWidth, halfScreenHeight);
                if (mouseX > Projectile.Center.X)
                    Projectile.direction = 1;
                else
                    Projectile.direction = -1;
                Projectile.spriteDirection = Projectile.direction;
                floatTimer += 0.06f;
                currentAnimationState = AnimationState.Idle;

                bool aboveTile = false;
                if (firstStandType != Aerosmith)
                {
                    aboveTile = Collision.SolidTiles((int)Projectile.Center.X / 16, (int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, (int)(Projectile.Center.Y / 16) + 4);
                    if (aboveTile)
                    {
                        Projectile.velocity.Y = (float)Math.Sin(floatTimer) / 5f;
                    }
                    else
                    {
                        if (Projectile.velocity.Y < 6f)
                            Projectile.velocity.Y += 0.2f;
                        if (Vector2.Distance(Projectile.Center, player.Center) >= RemoteControlMaxDistance)
                        {
                            Projectile.velocity = player.Center - Projectile.Center;
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= 0.8f;
                        }
                    }
                }

                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft)
                    {
                        Vector2 moveVelocity = Main.MouseWorld - Projectile.Center;
                        moveVelocity.Normalize();
                        Projectile.velocity.X = moveVelocity.X * 5f;
                        if (aboveTile)
                            Projectile.velocity.Y += moveVelocity.Y * 2f;

                        if (Vector2.Distance(Projectile.Center, player.Center) >= RemoteControlMaxDistance)
                        {
                            Projectile.velocity = player.Center - Projectile.Center;
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= 0.8f;
                        }
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        Projectile.velocity.X *= 0.78f;
                        Projectile.netUpdate = true;
                    }

                    if (Main.mouseRight)
                    {
                        int curAnimationState = (int)currentAnimationState;
                        if (standAbilities.OverrideMainAttack(ref curAnimationState))
                        {
                            int punchIndex = -1;
                            Vector2 targetPosition = Projectile.Center + new Vector2(5 * Projectile.direction, 0f);
                            float movementSpeed = 5f;

                            attacking = true;
                            currentAnimationState = AnimationState.Attack;
                            float rotaY = targetPosition.Y - Projectile.Center.Y;
                            Projectile.rotation = MathHelper.ToRadians((rotaY * Projectile.spriteDirection) / 6f);
                            Vector2 velocityAddition = targetPosition - Projectile.Center;
                            velocityAddition.Normalize();
                            velocityAddition *= movementSpeed + mPlayer.standTier;

                            Projectile.spriteDirection = Projectile.direction = targetPosition.X > Projectile.Center.X ? 1 : -1;
                            float targetDistance = Vector2.Distance(targetPosition, Projectile.Center);
                            if (targetDistance > 16f)
                                Projectile.velocity = player.velocity + velocityAddition;
                            else
                                Projectile.velocity = Vector2.Zero;

                            PlayPunchSound();
                            if (shootCount <= 0)
                            {
                                shootCount += newPunchTime;
                                Vector2 shootVel = targetPosition - Projectile.Center;
                                if (shootVel == Vector2.Zero)
                                    shootVel = new Vector2(0f, 1f);

                                shootVel.Normalize();
                                shootVel *= ProjectileSpeed;
                                int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<FanStandFists>(), newPunchDamage, PunchKnockback, Projectile.owner, FistID, TierNumber);
                                Main.projectile[projIndex].timeLeft = (int)(Main.projectile[projIndex].timeLeft);
                                Main.projectile[projIndex].netUpdate = true;
                                punchIndex = projIndex;
                            }
                            Projectile.netUpdate = true;
                            if (punchIndex != -1)
                            {
                                (Main.projectile[punchIndex].ModProjectile as FanStandFists).standInstance = Projectile.ModProjectile;
                                shootCount = standAbilities.CloneSpeedChanges(firstStandType, standCompletion);
                            }

                            currentAnimationState = AnimationState.Attack;
                            for (int n = 0; n < Main.maxNPCs; n++)
                            {
                                if (Main.npc[n].active)
                                {
                                    NPC npc = Main.npc[n];
                                    bool directionCheck = Projectile.direction == 1 ? npc.Center.X > Projectile.Center.X : npc.Center.X < Projectile.Center.X;
                                    float npcDistance = Vector2.Distance(npc.Center, Projectile.Center);
                                    if (directionCheck && npcDistance < AttackVacuumRange)
                                    {
                                        Vector2 direction = Projectile.Center - npc.Center;
                                        direction.Normalize();
                                        direction *= AttackVacuumForce;
                                        npc.velocity += direction;
                                    }
                                }
                            }
                            standAbilities.WhirlwindAttackEffects(Projectile, AttackVacuumRange);
                            AttackClone(firstStandType, standCompletion);
                        }
                        else
                            currentAnimationState = (AnimationState)curAnimationState;
                    }
                }

                if (SecondSpecialKeyPressed(false) && shootCount <= 0)
                {
                    shootCount += 30;
                    mPlayer.standControlStyle = MyPlayer.StandControlStyle.Manual;
                }
            }


            /*int amountOfParticles = Main.rand.Next(1, 2 + 1);
            for (int i = 0; i < amountOfParticles; i++)
            {
                Vector2 spawnPosition = Projectile.Center - (new Vector2(84, 86) / 2f) + HeadParticlePoint.ToVector2() + StandOffset;
                float angle = Main.rand.Next(115, 155 + 1) + 90;
                Vector2 dustVelocity = MathHelper.ToRadians(angle).ToRotationVector2() * 1.4f;
                int dustIndex = Dust.NewDust(spawnPosition, 4, 4, DustID.Platinum, dustVelocity.X, dustVelocity.Y, Scale: 0.7f);
                Main.dust[dustIndex].noGravity = true;
            }

            amountOfParticles = Main.rand.Next(1, 2 + 1);
            for (int i = 0; i < amountOfParticles; i++)
            {
                Vector2 spawnPosition = Projectile.Center - (new Vector2(84, 86) / 2f) + ArmParticlePoint.ToVector2() + StandOffset;
                float angle = Main.rand.Next(133, 137 + 1);
                Vector2 dustVelocity = MathHelper.ToRadians(angle).ToRotationVector2() * 1.4f;
                int dustIndex = Dust.NewDust(spawnPosition, 4, 4, DustID.Copper, dustVelocity.X, dustVelocity.Y, Scale: 0.7f);
                Main.dust[dustIndex].noGravity = true;
            }

            amountOfParticles = Main.rand.Next(1, 2 + 1);
            for (int i = 0; i < amountOfParticles; i++)
            {
                Vector2 spawnPosition = Projectile.Center - (new Vector2(84, 86) / 2f) + LegParticlePoint.ToVector2() + StandOffset;
                float angle = Main.rand.Next(133, 137 + 1);
                Vector2 dustVelocity = MathHelper.ToRadians(angle).ToRotationVector2() * 1.4f;
                int dustIndex = Dust.NewDust(spawnPosition, 4, 4, DustID.Platinum, dustVelocity.X, dustVelocity.Y, Scale: 0.7f);
                Main.dust[dustIndex].noGravity = true;
            }*/
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(secondaryAbilityStab);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            secondaryAbilityStab = reader.ReadBoolean();
        }

        public override bool CustomStandDrawing => true;

        public override void CustomDrawStand(Color drawColor)
        {
            if (!canDraw)
                return;

            if (UseProjectileAlpha)
                drawColor *= Projectile.alpha / 255f;

            effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;

            if (standTexture != null && Main.netMode != NetmodeID.Server)
            {
                int frameHeight = standTexture.Height / amountOfFrames;
                Vector2 drawOffset = StandOffset;
                drawOffset.X *= Projectile.spriteDirection;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                Rectangle animRect = new Rectangle(0, frameHeight * Projectile.frame, standTexture.Width, frameHeight);
                Vector2 standOrigin = new Vector2(standTexture.Width / 2f, frameHeight / 2f);
                Main.EntitySpriteDraw(standTexture, drawPosition, animRect, drawColor, Projectile.rotation, standOrigin, Projectile.scale, effects, 0);
            }
        }

        private void AttackClone(int firstStandType, float standCompletion)
        {
            if (firstStandType == -1)
                return;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (firstStandType == TheHand)
            {
                if (Main.mouseLeft && !standAbilities.scrapeMode)
                {
                    if (!player.GetModPlayer<MyPlayer>().canStandBasicAttack)
                    {
                        currentAnimationState = AnimationState.Idle;
                        return;
                    }

                    currentAnimationState = AnimationState.SecondaryAbility;
                    float rotaY = Main.MouseWorld.Y - Projectile.Center.Y;
                    Projectile.rotation = MathHelper.ToRadians((rotaY * Projectile.spriteDirection) / 6f);

                    if (mouseX > player.position.X)
                        player.direction = 1;
                    else
                        player.direction = -1;

                    Vector2 velocityAddition = Main.MouseWorld - Projectile.position;
                    velocityAddition.Normalize();
                    velocityAddition *= 5f;
                    float mouseDistance = Vector2.Distance(Main.MouseWorld, Projectile.Center);
                    if (mouseDistance > 40f)
                        Projectile.velocity = player.velocity + velocityAddition;
                    else
                        Projectile.velocity = Vector2.Zero;

                    if (shootCount <= 0 && (Projectile.frame == 1 || Projectile.frame == 4))
                    {
                        shootCount += (int)(newPunchTime * 1.2);
                        Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                        shootVel.Normalize();
                        shootVel *= ProjectileSpeed;

                        int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<FanStandFists>(), (int)(newPunchDamage * 2.5f), PunchKnockback, Projectile.owner, FistID);
                        Main.projectile[projIndex].netUpdate = true;
                        SoundStyle theHandScrapeSound = WaywardSonAbilities.ScrapeSoundEffect;
                        theHandScrapeSound.Pitch = Main.rand.NextFloat(0, 0.6f + 1f);
                        theHandScrapeSound.Volume = JoJoStands.JoJoStands.ModSoundsVolume;
                        SoundEngine.PlaySound(theHandScrapeSound, Projectile.Center);
                    }
                    Projectile.netUpdate = true;
                    LimitDistance();
                }
            }
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
                PlayAnimation("Dash");
            else if (currentAnimationState == AnimationState.SecondaryAbilityStab)
                PlayAnimation("Slash");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/WaywardSon/WaywardSon_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime / 2, true);
            else if (animationName == "Dash")
                AnimateStand(animationName, 2, 15, true);
            else if (animationName == "Slash")
                AnimateStand(animationName, 4, 6, false);
            else if (animationName == "Pose")
                AnimateStand(animationName, 2, 8, true);
        }
    }
}