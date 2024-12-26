using JoJoStands;
using JoJoStands.Dusts;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.WaywardSon
{
    public class WaywardSonTiny : StandClass
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 26;
            Projectile.shouldFallThrough = false;
        }

        public override string PoseSoundName => "StandReadyFire";
        public override StandAttackType StandType => StandAttackType.Ranged;
        public override Vector2 StandOffset => new Vector2(8f, 0f);
        public override float ProjectileSpeed => 12f;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        public int updateTimer = 0;

        private bool setStats = false;
        private int projectileDamage = 0;
        private int shootTime = 0;
        private float speedRandom = 0f;     //So the AI isn't always the same
        private int shootStartTimeOffset = 0;
        private int stabCooldownTimer = 0;

        public new enum AnimationState
        {
            Idle,
            Walk,
            Stab,
        }

        public override void AI()
        {
            SelectAnimation();
            updateTimer++;
            if (shootCount > 0)
                shootCount--;
            if (stabCooldownTimer > 0)
                stabCooldownTimer--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            mPlayer.poseSoundName = PoseSoundName;
            if (mPlayer.standOut && mPlayer.standTier != 0)
                Projectile.timeLeft = 2;

            if (updateTimer >= 90)      //an automatic netUpdate so that if something goes wrong it'll at least fix in about a second
            {
                updateTimer = 0;
                Projectile.netUpdate = true;
            }

            if (!setStats)
            {
                if (Projectile.ai[0] == 1f)
                {
                    projectileDamage = 12;
                    shootTime = 90;
                }
                else if (Projectile.ai[0] == 2f)
                {
                    projectileDamage = 21;
                    shootTime = 80;
                }
                else if (Projectile.ai[0] == 3f)
                {
                    projectileDamage = 34;
                    shootTime = 70;
                }
                else if (Projectile.ai[0] == 4f)
                {
                    projectileDamage = 46;
                    shootTime = 60;
                }
                shootTime += Main.rand.Next(-15, 15 + 1);
                speedRandom = Main.rand.NextFloat(-0.08f, 0f);
                shootStartTimeOffset = Main.rand.Next(0, shootTime);
                setStats = true;

                int amountOfParticles = Main.rand.Next(1, 2);
                int[] dustTypes = new int[3] { ModContent.DustType<StandSummonParticles>(), ModContent.DustType<StandSummonShine1>(), ModContent.DustType<StandSummonShine2>() };
                Vector2 dustSpawnOffset = StandOffset;
                dustSpawnOffset.X *= Projectile.spriteDirection;
                for (int i = 0; i < amountOfParticles; i++)
                {
                    int dustType = dustTypes[Main.rand.Next(0, 3)];
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Scale: (float)Main.rand.Next(80, 120) / 100f);
                }
                for (int i = 0; i < Main.rand.Next(2, 5 + 1); i++)
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, Main.rand.NextFloat(-0.3f, 1f + 0.3f), Main.rand.NextFloat(-0.3f, 0.3f + 1f), Scale: Main.rand.NextFloat(-1f, 1f + 1f));
            }

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                MovementAI();
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.direction = 1;
                    if (Main.MouseWorld.X <= Projectile.position.X)
                        Projectile.direction = -1;
                    Projectile.spriteDirection = Projectile.direction;

                    if (Main.mouseLeft && mPlayer.canStandBasicAttack)
                    {
                        Vector2 velocity = (Main.MouseWorld + new Vector2(Main.rand.Next(-40, 40 + 1) / 10f, Main.rand.Next(-40, 40 + 1) / 10f) * 16f) - Projectile.Center;
                        velocity.Normalize();
                        velocity *= 4f;
                        Projectile.velocity = velocity;

                        NPC targetNPC = null;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.active && npc.lifeMax > 5 && !npc.townNPC && Vector2.Distance(npc.Center, Main.MouseWorld) <= 24f && Vector2.Distance(npc.Center, Projectile.Center) <= 3 * 16f)
                            {
                                targetNPC = npc;
                                break;
                            }
                        }

                        if (targetNPC != null)
                        {
                            velocity = targetNPC.Center - Projectile.Center;
                            velocity.Normalize();
                            velocity *= 1.3f;
                            Projectile.velocity = velocity;

                            if (stabCooldownTimer <= 0)
                            {
                                NPC.HitInfo hitInfo = new NPC.HitInfo()
                                {
                                    Damage = (int)(projectileDamage * 1.5f),
                                    Knockback = 1.2f,
                                    HitDirection = (int)(Math.Abs(velocity.X) / -velocity.X)
                                };
                                targetNPC.StrikeNPC(hitInfo);
                                stabCooldownTimer += 45 + Main.rand.Next(1, 6 + 1);
                                if (Main.rand.Next(0, 4 + 1) == 0)
                                    SoundEngine.PlaySound(SoundID.Item2, Projectile.Center);
                            }
                        }
                        currentAnimationState = AnimationState.Stab;
                        if (Main.rand.Next(0, 5 + 1) == 0)
                            Projectile.frameCounter--;
                    }
                    else
                        shootCount = shootStartTimeOffset;
                }
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                float detectionRange = (20f + (4f * mPlayer.standTier)) * 16f;
                NPC target = FindNearestTarget(detectionRange, new Vector2(0f, -16f));
                if (target != null)
                {
                    Vector2 velocity = target.Center - Projectile.Center;
                    velocity.Normalize();
                    velocity *= 1.3f;
                    Projectile.velocity = velocity;
                    if (stabCooldownTimer <= 0 && target.Distance(Projectile.Center) < 3f * 16f)
                    {
                        currentAnimationState = AnimationState.Stab;
                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                        {
                            Damage = (int)(projectileDamage * 1.5f),
                            Knockback = 1.2f,
                            HitDirection = (int)(Math.Abs(velocity.X) / -velocity.X)
                        };
                        target.StrikeNPC(hitInfo);
                        stabCooldownTimer += 45 + Main.rand.Next(1, 6 + 1);
                    }
                }
                MovementAI();
            }
            if (!mPlayer.standOut)
                Projectile.Kill();

            bool collisionResult = Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
            Projectile.tileCollide = !collisionResult;
            if (collisionResult)
            {
                Projectile.velocity.Y = 0f;
                Projectile.position.Y -= 2f;
            }
            newMaxDistance = MaxDistance + mPlayer.standRangeBoosts;
            LimitDistance();
        }

        public override void StandKillEffects()
        {
            for (int i = 0; i < Main.rand.Next(2, 5 + 1); i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, Main.rand.NextFloat(-0.3f, 1f + 0.3f), Main.rand.NextFloat(-0.3f, 0.3f + 1f), Scale: Main.rand.NextFloat(-1f, 1f + 1f));
            }
        }

        private const float IdleRange = 8 * 16f;
        private const float MaxFlyingIdleDistance = 6 * 16f;

        private void MovementAI()       //Pretty much the pet AI
        {
            Player player = Main.player[Projectile.owner];
            Vector2 directionToPlayer = player.Center - Projectile.Center;
            directionToPlayer.Normalize();
            directionToPlayer *= player.moveSpeed;
            float xDist = Math.Abs(player.position.X - Projectile.position.X);
            bool standingOnPlatform = TileID.Sets.Platforms[Main.tile[(int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f) + 1].TileType];
            if (!WorldGen.SolidTile((int)(Projectile.position.X / 16f), (int)(Projectile.position.Y / 16f) + 2) && !standingOnPlatform)
            {
                Projectile.ai[0] = 1f;
            }
            else
            {
                Projectile.ai[0] = 0f;
            }

            float distance = Vector2.Distance(player.Center, Projectile.Center);
            if (Projectile.ai[0] == 0f)
            {
                Projectile.tileCollide = true;
                if (Projectile.velocity.Y < 6f)
                    Projectile.velocity.Y += 0.3f;

                if (xDist >= IdleRange)
                    Projectile.velocity.X = directionToPlayer.X * xDist / 14;
                else
                    Projectile.velocity.X *= 0.96f + speedRandom;
                if (Math.Abs(Projectile.velocity.X) > 0.01f)
                    currentAnimationState = AnimationState.Walk;
                else
                    currentAnimationState = AnimationState.Idle;
            }
            else if (Projectile.ai[0] == 1f)        //Flying
            {
                currentAnimationState = AnimationState.Idle;
                Projectile.velocity.Y += 0.03f;
                if (distance >= MaxFlyingIdleDistance)
                {
                    if (Math.Abs(player.velocity.X) > 1f || Math.Abs(player.velocity.Y) > 1f)
                    {
                        directionToPlayer *= distance / 16f;
                        Projectile.velocity = directionToPlayer;
                    }
                    else
                    {
                        directionToPlayer *= (0.9f + speedRandom) * (distance / 60f);
                        Projectile.velocity = directionToPlayer;
                    }
                }
            }
            Projectile.velocity *= 0.99f;
            if (distance >= IdleRange + (2f * 16f))        //Out of range
            {
                Projectile.tileCollide = false;
                directionToPlayer *= distance / 90f;
                Projectile.velocity += directionToPlayer;
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
            else if (currentAnimationState == AnimationState.Walk)
                PlayAnimation("Walk");
            else if (currentAnimationState == AnimationState.Stab)
                PlayAnimation("Attack");
        }

        public override void PlayAnimation(string animationName)
        {
            if (Main.netMode != NetmodeID.Server)
                standTexture = (Texture2D)ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/WaywardSon/WaywardSon_Tiny_" + animationName);

            if (animationName == "Idle")
                AnimateStand(animationName, 1, 8, true);
            else if (animationName == "Walk")
                AnimateStand(animationName, 4, 20 - (int)Projectile.velocity.X, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, 5, true);
        }
    }
}