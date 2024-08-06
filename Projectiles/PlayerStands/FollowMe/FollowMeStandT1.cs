using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.FollowMe
{
    public class FollowMeStandT1 : StandClass
    {
        public override int PunchDamage => 16;
        public override int AltDamage => 31;
        public override int HalfStandHeight => 39;
        public override int TierNumber => 1;
        public override StandAttackType StandType => StandAttackType.Melee;
        public override bool CanUseAfterImagePunches => false;

        private bool grabbing = false;
        private float windUpForce = 1f;

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

            if (Projectile.owner == Main.myPlayer)
            {
                if (Main.mouseLeft || Main.mouseRight && !grabbing)
                {
                    float rotaY = Main.MouseWorld.Y - Projectile.Center.Y;
                    Projectile.rotation = MathHelper.ToRadians((rotaY * Projectile.spriteDirection) / 6f);

                    Projectile.direction = 1;
                    if (Main.MouseWorld.X < Projectile.position.X)
                        Projectile.direction = -1;
                    Projectile.spriteDirection = Projectile.direction;
                    Vector2 velocityAddition = Main.MouseWorld - Projectile.position;
                    velocityAddition.Normalize();
                    velocityAddition *= 5f;
                    float mouseDistance = Vector2.Distance(Main.MouseWorld, Projectile.Center);
                    if (mouseDistance > 16f)
                        Projectile.velocity = player.velocity + velocityAddition;
                    else
                        Projectile.velocity = Vector2.Zero;
                }
                if (Main.mouseLeft)
                {
                    currentAnimationState = AnimationState.Attack;
                    windUpForce += 0.02f;
                    Main.mouseRight = false;
                    Projectile.frame = 1;
                }
                else
                {
                    if (!grabbing && !Main.mouseRight && windUpForce == 1f)
                    {
                        Vector2 vector131 = player.Center;
                        vector131.X -= (float)((9 + player.width / 2) * player.direction);
                        vector131.Y -= -35f + HalfStandHeight;
                        Projectile.Center = Vector2.Lerp(Projectile.Center, vector131, 0.2f);
                        Projectile.velocity *= 0.8f;
                        Projectile.direction = (Projectile.spriteDirection = player.direction);
                        Projectile.rotation = 0;
                        currentAnimationState = AnimationState.Idle;
                    }
                }
                if (!Main.mouseLeft && windUpForce != 1f && Projectile.frame == 6)
                {
                    shootCount += 12;
                    Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = new Vector2(0f, 1f);

                    shootVel.Normalize();
                    shootVel *= ProjectileSpeed;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<FanStandFists>(), (int)(newPunchDamage * windUpForce), 4f * windUpForce, Main.myPlayer);
                    Main.projectile[proj].timeLeft = 6;
                    Main.projectile[proj].netUpdate = true;
                    Projectile.netUpdate = true;
                    windUpForce = 1f;
                    currentAnimationState = AnimationState.Idle;

                }
                if (Main.mouseRight && shootCount <= 0 && !grabbing)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active)
                        {
                            if (Projectile.Distance(npc.Center) <= 30f && !npc.boss && !npc.immortal && !npc.hide)
                            {
                                Projectile.ai[0] = npc.whoAmI;
                                grabbing = true;
                                break;
                            }
                        }
                    }
                    Projectile.netUpdate = true;
                    currentAnimationState = AnimationState.SecondaryAbility;
                }
            }
            if (grabbing && Projectile.ai[0] != -1f)
            {
                currentAnimationState = AnimationState.SecondaryAbility;
                Projectile.velocity = Vector2.Zero;
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                npc.direction = -Projectile.direction;
                npc.position = Projectile.position + new Vector2(5f * Projectile.direction, -2f - npc.height / 3f);
                npc.velocity = Vector2.Zero;
                if (Projectile.frame == 7)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                        {
                            Damage = newPunchDamage,
                            Knockback = 7f,
                            HitDirection = Projectile.direction
                        };
                        npc.StrikeNPC(hitInfo, noPlayerInteraction: true);
                        NetMessage.SendStrikeNPC(npc, hitInfo, Main.myPlayer);
                    }
                    shootCount += 180;
                    Projectile.ai[0] = -1f;
                    grabbing = false;
                }
            }
            if (!grabbing)
                LimitDistance();
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(grabbing);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            grabbing = reader.ReadBoolean();
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
                PlayAnimation("WindUp");
            else if (currentAnimationState == AnimationState.SecondaryAbility)
                PlayAnimation("Grab");

            if (Main.mouseRight && Projectile.owner == Main.myPlayer && shootCount <= 0 && !grabbing)       //loops grabbing
            {
                if (Projectile.frame <= 0)
                    Projectile.frame = 1;
                else if (Projectile.frame >= 3)
                    Projectile.frame = 1;
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/FollowMe/FollowMe_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "WindUp")
                AnimateStand(animationName, 7, 6, false);
            else if (animationName == "Grab")
                AnimateStand(animationName, 9, 12, false);
        }
    }
}