using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.WaywardSon
{
    public class WaywardSonStandT2 : StandClass
    {
        public override int HalfStandHeight => 37;          //a simpler version of doing the above

        public override int PunchDamage => 47;
        public override int AltDamage => 54;
        public override int PunchTime => 12;
        public override int TierNumber => 2;
        public override bool CanUseAfterImagePunches => false;
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        private const float AttackVacuumRange = 10f * 16f;
        private const float AttackVacuumForce = 0.20f;

        private Vector2 secondaryDirection;
        private bool secondaryAbilityStab = false;
        private int stabNPCTarget = -1;

        public new enum AnimationState
        {
            Idle,
            Attack,
            SecondaryAbility,
            SecondaryAbilityStab,
            Special,
            Pose
        }

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            shootCount--;
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.mouseLeft)
                {
                    Punch(afterImages: false);
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

                    int amountOfDusts = Main.rand.Next(0, 2 + 1);
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

        public override void AnimationCompleted(string animationName)
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (animationName == "SecondaryStab")
                player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(15));
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
            else if (currentAnimationState == AnimationState.SecondaryAbilityStab)
                PlayAnimation("SecondaryStab");
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
            else if (animationName == "SecondaryStab")
                AnimateStand(animationName, 4, 10, true);
        }
    }
}