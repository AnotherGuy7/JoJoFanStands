using JoJoFanStands.Items.Stands;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Items;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.WaywardSon
{
    public class WaywardSonStandT3 : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 68;
        public override int AltDamage => 76;
        public override int PunchTime => 11;
        public override int TierNumber => 3;
        public override int FistID => FanStandFists.WaywardSonFists;
        public override Vector2 StandOffset => new Vector2(-24, 0f);
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;
        public WaywardSonAbilities standAbilities;
        public bool canAttack = true;
        public bool canDraw = true;

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

        private const float AttackVacuumRange = 12f * 16f;
        private const float AttackVacuumForce = 0.25f;

        private Vector2 secondaryDirection;
        private bool secondaryAbilityStab = false;
        private int stabNPCTarget = -1;

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
            }

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

            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.mouseLeft && canAttack)
                {
                    int curAnimationState = (int)currentAnimationState;
                    if (!standAbilities.OverrideMainAttack(ref curAnimationState))
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
                    currentAnimationState = (AnimationState)curAnimationState;
                }
                else
                {
                    if (!secondaryAbility)
                    {
                        StayBehind();
                        currentAnimationState = AnimationState.Idle;
                    }
                }
                if (Main.mouseRight)
                {
                    stabNPCTarget = -1;
                    secondaryAbility = true;
                    secondaryAbilityStab = false;
                    currentAnimationState = AnimationState.SecondaryAbility;
                    secondaryDirection = Main.MouseWorld - Projectile.Center;
                    secondaryDirection.Normalize();
                    secondaryDirection *= 12f;
                }
                standAbilities.ManageAbilities(Projectile);
            }

            float playerDistance = (player.Center - Projectile.Center).Length();
            if (secondaryAbility)
            {
                if (!secondaryAbilityStab)
                {
                    currentAnimationState = AnimationState.SecondaryAbility;
                    Projectile.velocity = secondaryDirection;
                    Projectile.direction = 1;
                    if (Projectile.velocity.X < 0)
                        Projectile.direction = -1;
                    Projectile.rotation = secondaryDirection.ToRotation();

                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        NPC npc = Main.npc[n];
                        if (npc.active && Projectile.Distance(npc.Center) <= 15f)
                        {
                            if (Projectile.owner == Main.myPlayer)
                            {
                                stabNPCTarget = n;
                                secondaryAbilityStab = true;
                                break;
                            }
                        }
                    }
                    if (playerDistance > newMaxDistance * 2)
                    {
                        secondaryAbility = false;
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(10));
                        standAbilities.OnSecondaryUse(Projectile);
                    }
                }
                else
                {
                    currentAnimationState = AnimationState.SecondaryAbilityStab;
                    NPC npc = Main.npc[stabNPCTarget];
                    if (!npc.active || npc.life <= 0)
                    {
                        stabNPCTarget = -1;
                        secondaryAbility = false;
                        secondaryAbilityStab = false;
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(15));
                        return;
                    }

                    if (npc.active && (Projectile.frame == 2 || Projectile.frame == 5))     //hit frames
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            NPC.HitInfo hitInfo = new NPC.HitInfo()
                            {
                                Damage = AltDamage,
                                Knockback = 1f,
                                HitDirection = Projectile.direction,
                                Crit = true
                            };
                            npc.StrikeNPC(hitInfo);
                            NetMessage.SendStrikeNPC(npc, hitInfo, Main.myPlayer);
                        }
                    }
                }
            }
            else
            {
                LimitDistance();
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
            if (firstStandType == StarPlatinum)
            {

            }
            else if (firstStandType == TheHand)
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

        /*private void SpecialClone(int firstStandType, float standCompletion)
        {
            if (firstStandType == -1)
                return;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            
            if (firstStandType == )
        }*/

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
                PlayAnimation("Weld");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/WaywardSon/WaywardSon_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
            else if (animationName == "Secondary")
                AnimateStand(animationName, 1, 15, true);
            else if (animationName == "Weld")
                AnimateStand(animationName, 1, 15, true);
        }
    }
}