using Microsoft.Xna.Framework;
using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using System.IO;
using JoJoFanStands.NPCs;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using JoJoStands.Projectiles;
using JoJoStands.Buffs.Debuffs;
using Terraria.Audio;

namespace JoJoFanStands.Projectiles.PlayerStands.SlavesOfFear
{
    public class VirtualInsanityStandFinal : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 96;
        public override int AltDamage => 84;
        public override int PunchTime => 9;
        public override int TierNumber => 4;
        public override bool CanUseAfterImagePunches => false;
        public override int FistID => FanStandFists.VirtualInsanityFists;
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        public new enum AnimationState
        {
            Idle,
            Attack,
            SecondaryAbility,
            Special,
            RealityOverwritePunch,
            Pose
        }

        private int mouseRightHoldTimer = 0;
        private bool powerInstallBuff = false;

        private byte attackType = Attack_Barrage;

        private const byte Attack_Barrage = 0;
        private const byte Attack_Sword = 1;
        private const byte Attack_Cannon = 2;
        private readonly string[] AttackStyleNames = new string[3] { "Barrage", "Sword", "Cannon" };
        private readonly int[] AttackStyleIdleFrameAmounts = new int[3] { 5, 5, 4 };
        private readonly int[] AttackStyleAttackFrameAmounts = new int[3] { 17, 18, 4 };

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (attackType == Attack_Sword)
                mPlayer.standRangeBoosts += 4f * 16f;

            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.mouseLeft)
                {
                    if (attackType == Attack_Barrage)
                    {
                        Punch(afterImages: false);
                        if (attacking)
                            currentAnimationState = AnimationState.Attack;
                    }
                    else if (attackType == Attack_Sword)
                    {
                        Vector2 targetPosition = Main.MouseWorld;
                        if (JoJoStands.JoJoStands.StandAimAssist)
                        {
                            float lowestDistance = 4f * 16f;
                            for (int n = 0; n < Main.maxNPCs; n++)
                            {
                                NPC npc = Main.npc[n];
                                if (npc.active && npc.CanBeChasedBy(this, false))
                                {
                                    float distance = Vector2.Distance(npc.Center, Main.MouseWorld);
                                    if (distance < lowestDistance && Collision.CanHitLine(Projectile.Center, Projectile.width, Projectile.height, npc.position, npc.width, npc.height) && npc.lifeMax > 5 && !npc.immortal && !npc.hide && !npc.townNPC && !npc.friendly)
                                    {
                                        targetPosition = npc.Center;
                                        lowestDistance = distance;
                                    }
                                }
                            }
                        }

                        if (!mPlayer.canStandBasicAttack)
                        {
                            currentAnimationState = AnimationState.Idle;
                            return;
                        }

                        attacking = true;
                        currentAnimationState = AnimationState.Attack;
                        float rotaY = targetPosition.Y - Projectile.Center.Y;
                        Projectile.rotation = MathHelper.ToRadians((rotaY * Projectile.spriteDirection) / 6f);
                        Vector2 velocityAddition = targetPosition - Projectile.Center;
                        velocityAddition.Normalize();
                        velocityAddition *= 5f + mPlayer.standTier;

                        Projectile.spriteDirection = Projectile.direction = targetPosition.X > Projectile.Center.X ? 1 : -1;
                        float targetDistance = Vector2.Distance(targetPosition, Projectile.Center);
                        if (targetDistance > 16f)
                            Projectile.velocity = player.velocity + velocityAddition;
                        else
                            Projectile.velocity = Vector2.Zero;

                        PlayPunchSound();
                        if (!powerInstallBuff)
                        {
                            if (Projectile.frame == 2 || Projectile.frame == 8 || Projectile.frame == 14)
                            {
                                Vector2 shootVel = targetPosition - Projectile.Center;
                                if (shootVel == Vector2.Zero)
                                    shootVel = new Vector2(0f, 1f);

                                shootVel.Normalize();
                                shootVel *= ProjectileSpeed;
                                int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<FanStandFists>(), newPunchDamage, PunchKnockback, Projectile.owner, FistID, TierNumber);
                                Main.projectile[projIndex].Resize(60, 60);
                                Main.projectile[projIndex].timeLeft = (int)(Main.projectile[projIndex].timeLeft);
                                Main.projectile[projIndex].netUpdate = true;
                            }
                        }
                        else
                        {
                            if (Projectile.frame == 16)
                            {
                                int rectWidth = 382;
                                int rectHeight = 357;
                                int rectXPosition = Projectile.direction == 1 ? (int)Projectile.position.X : (int)Projectile.position.X - rectWidth;
                                Rectangle attackHitbox = new Rectangle(rectXPosition, (int)Projectile.position.Y - (rectHeight / 2), rectWidth, rectHeight);
                                for (int n = 0; n < Main.maxNPCs; n++)
                                {
                                    NPC npc = Main.npc[n];
                                    if (npc.CanBeChasedBy(this) && npc.Hitbox.Intersects(attackHitbox))
                                    {
                                        int damage = newPunchDamage * 4;
                                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                                        {
                                            Damage = damage,
                                            Knockback = PunchKnockback * 4f,
                                            HitDirection = npc.direction
                                        };
                                        npc.StrikeNPC(hitInfo);
                                    }
                                }
                            }
                        }

                        LimitDistance();
                        Projectile.netUpdate = true;
                    }
                    else if (attackType == Attack_Cannon)
                    {
                        if (!mPlayer.canStandBasicAttack)
                        {
                            currentAnimationState = AnimationState.Idle;
                            return;
                        }

                        attacking = true;
                        currentAnimationState = AnimationState.Attack;
                        Projectile.netUpdate = true;
                        if (shootCount <= 0)
                        {
                            shootCount += newShootTime;
                            Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                            if (shootVel == Vector2.Zero)
                                shootVel = new Vector2(0f, 1f);

                            shootVel.Normalize();
                            shootVel *= ProjectileSpeed;
                            int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<FireAnkh>(), newProjectileDamage, 3f, Projectile.owner);
                            Main.projectile[projIndex].netUpdate = true;
                            Projectile.netUpdate = true;
                        }
                    }
                }
                else
                {
                    if (attackType == Attack_Barrage)
                    {
                        if (!secondaryAbility)
                            StayBehind();
                    }
                    else if (attackType == Attack_Sword)
                    {
                        if (!secondaryAbility)
                            StayBehind();
                    }
                    else if (attackType == Attack_Cannon)
                    {
                        if (!attacking)
                            StayBehind();
                        else
                            GoInFront();
                    }
                    if (!attacking)
                        currentAnimationState = AnimationState.Idle;

                }
                if (Main.mouseRight)
                {
                    secondaryAbility = true;
                    currentAnimationState = AnimationState.SecondaryAbility;
                    if (attackType == Attack_Barrage)       //throw
                    {
                        Punch(afterImages: false);
                    }
                    else if (attackType == Attack_Sword)        //mega slash
                    {

                    }
                    else if (attackType == Attack_Cannon)       //Charged shots 1 (2s) & 2 (5s)
                    {
                        mouseRightHoldTimer++;
                        if (mouseRightHoldTimer >= 2 * 60)
                        {
                            int chargedProjectile = ModContent.ProjectileType<FireAnkh>();
                            if (mouseRightHoldTimer <= 5 * 50)
                                chargedProjectile = ModContent.ProjectileType<FireAnkh>();

                            shootCount += newShootTime * 3;
                            Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                            if (shootVel == Vector2.Zero)
                                shootVel = new Vector2(0f, 1f);

                            shootVel.Normalize();
                            shootVel *= ProjectileSpeed;
                            int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, chargedProjectile, newProjectileDamage, 3f, Projectile.owner);
                            Main.projectile[projIndex].netUpdate = true;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
            

            if (SpecialKeyPressed(false))
            {
                attackType++;
                if (attackType > Attack_Cannon)
                    attackType = Attack_Barrage;

                Main.NewText("Attack Style: " + AttackStyleNames[attackType]);
            }
            
            LimitDistance();
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(attackType);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            attackType = reader.ReadByte();
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
                PlayAnimation("Weld");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/" + AttackStyleNames[attackType] + "_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, AttackStyleIdleFrameAmounts[attackType], 14, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, AttackStyleAttackFrameAmounts[attackType], newPunchTime, true);
            else if (animationName == "Secondary")
                AnimateStand(animationName, 1, 15, true);
            else if (animationName == "Weld")
                AnimateStand(animationName, 1, 15, true);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 300, false);
        }
    }
}