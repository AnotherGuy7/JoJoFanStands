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
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;
        private WaywardSonAbilities standAbilities;

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
                if (Main.mouseLeft)
                {
                    Punch(ModContent.ProjectileType<FanStandFists>(), afterImages: false);
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

                    int amountOfDusts = Main.rand.Next(1, 2 + 1);
                    for (int i = 0; i < amountOfDusts; i++)
                    {
                        float xOffset = Main.rand.Next(0, (int)AttackVacuumRange + 1) * Projectile.direction;
                        float yOffset = (Main.rand.Next(-100, 100 + 1) / 100f) * (xOffset / AttackVacuumRange) * (AttackVacuumRange * 3f / 4f);       //Cone-like shape
                        Vector2 dustPosition = Projectile.Center + new Vector2(xOffset, yOffset);
                        Vector2 dustVelocity = -new Vector2(xOffset, yOffset);
                        dustVelocity.Normalize();
                        dustVelocity *= 3f * (Main.rand.Next(80, 120 + 1) / 100f);
                        int dustIndex = Dust.NewDust(dustPosition, 2, 2, DustID.Cloud);
                        Main.dust[dustIndex].velocity = dustVelocity;
                        Main.dust[dustIndex].noGravity = true;
                    }
                    AttackClone(firstStandType, standCompletion);
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
                if (standAbilities == null)
                    standAbilities = new WaywardSonAbilities(mPlayer.standTier);
                else
                    standAbilities.ManageAbilities(firstStandType, player, Projectile);
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

        private int CloneDamageChanges(int firstStandType, float standCompletion)
        {
            if (firstStandType == -1)
                return newPunchDamage;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            float newDamage = newPunchDamage;
            if (firstStandType == StarPlatinum)
                newDamage *= 2 * (standCompletion + 1f);
            else if (firstStandType == TheWorld)
                newDamage *= 2 * (standCompletion + 1f);
            else if (firstStandType == CrazyDiamond)
                newDamage *= 1.5f * (standCompletion + 1f);
            else if (firstStandType == Echoes && standCompletion == 0.25f)
                newDamage *= 0.5f;
            else if (firstStandType == KingCrimson)
                newDamage *= 2f * (standCompletion + 1f);
            else if (firstStandType == StoneFree)
                newDamage *= 1.5f * (standCompletion + 1f);
            return (int)newDamage;
        }

        private int CloneSpeedChanges(int firstStandType, float standCompletion)
        {
            if (firstStandType == -1)
                return newPunchTime;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            float punchTime = newPunchDamage;
            if (firstStandType == SilverChariot)
                punchTime *= 0.5f * (standCompletion + 1f);
            else if (firstStandType == MortalReminder)
                punchTime *= 0.5f * (standCompletion + 1f);
            return (int)punchTime;
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