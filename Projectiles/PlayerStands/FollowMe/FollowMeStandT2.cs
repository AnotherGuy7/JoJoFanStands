using JoJoFanStands.Buffs;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles.PlayerStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.FollowMe
{
    public class FollowMeStandT2 : StandClass
    {
        public override int PunchDamage => 28;
        public override int AltDamage => 45;
        public override int HalfStandHeight => 39;
        public override int TierNumber => 2;
        public override StandAttackType StandType => StandAttackType.Melee;

        private Vector2 velocityAddition;
        private float mouseDistance;
        private bool grabbing = false;
        private bool intangible = false;
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
            DrawOriginOffsetY = -HalfStandHeight;

            if (Projectile.direction == -1)
                DrawOffsetX = -30;
            else
                DrawOffsetX = 0;

            if (Main.mouseLeft || Main.mouseRight && !grabbing)
            {
                float rotaY = Main.MouseWorld.Y - Projectile.Center.Y;
                Projectile.rotation = MathHelper.ToRadians((rotaY * Projectile.spriteDirection) / 6f);

                Projectile.direction = 1;
                if (Main.MouseWorld.X < Projectile.position.X)
                    Projectile.direction = -1;
                Projectile.spriteDirection = Projectile.direction;
                /*if (Projectile.position.X < Main.MouseWorld.X - 5f)
                {
                    velocityAddition.X = 5f;
                }
                if (Projectile.position.X > Main.MouseWorld.X + 5f)
                {
                    velocityAddition.X = -5f;
                }
                if (Projectile.position.X > Main.MouseWorld.X - 5f && Projectile.position.X < Main.MouseWorld.X + 5f)
                {
                    velocityAddition.X = 0f;
                }
                if (Projectile.position.Y > Main.MouseWorld.Y + 5f)
                {
                    velocityAddition.Y = -5f;
                }
                if (Projectile.position.Y < Main.MouseWorld.Y - 5f)
                {
                    velocityAddition.Y = 5f;
                }
                if (Projectile.position.Y < Main.MouseWorld.Y + 5f && Projectile.position.Y > Main.MouseWorld.Y - 5f)
                {
                    velocityAddition.Y = 0f;
                }*/
                velocityAddition = Main.MouseWorld - Projectile.position;
                velocityAddition.Normalize();
                velocityAddition *= 5f;
                mouseDistance = Vector2.Distance(Main.MouseWorld, Projectile.Center);
                if (mouseDistance > 40f)
                {
                    Projectile.velocity = player.velocity + velocityAddition;
                }
                if (mouseDistance <= 40f)
                {
                    Projectile.velocity = Vector2.Zero;
                }
            }
            if (Main.mouseLeft && Projectile.owner == Main.myPlayer)
            {
                idleFrames = false;
                attackFrames = true;
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
                    idleFrames = true;
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
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, shootVel.X, shootVel.Y, ModContent.ProjectileType<Fists>(), (int)(newPunchDamage * windUpForce), 4f * windUpForce, Main.myPlayer);
                Main.projectile[proj].timeLeft = 6;
                Main.projectile[proj].netUpdate = true;
                Projectile.netUpdate = true;
                windUpForce = 1f;
                idleFrames = true;
                attackFrames = false;
            }
            if (Main.mouseRight && Projectile.owner == Main.myPlayer && shootCount <= 0 && !grabbing)
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
                        }
                    }
                }
            }
            if (grabbing && Projectile.ai[0] != -1f)
            {
                Projectile.velocity = Vector2.Zero;
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                npc.direction = -Projectile.direction;
                npc.position = Projectile.position + new Vector2(5f * Projectile.direction, -2f - npc.height / 3f);
                npc.velocity = Vector2.Zero;
                if (Projectile.frame == 7)
                {
                    npc.StrikeNPC(newPunchDamage, 7f, Projectile.direction, true);
                    shootCount += 180;
                    Projectile.ai[0] = -1f;
                    grabbing = false;
                }
            }
            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed && !player.HasBuff(ModContent.BuffType<Intangible>()) && !player.HasBuff(ModContent.BuffType<AbilityCooldown>()))
            {
                player.AddBuff(ModContent.BuffType<Intangible>(), 7200);
            }
            intangible = player.HasBuff(ModContent.BuffType<Intangible>());
            if (!grabbing)
            {
                LimitDistance();
            }
            if (intangible)
            {
                Projectile.alpha = 80;
            }
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(grabbing);
            writer.Write(intangible);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            grabbing = reader.ReadBoolean();
            intangible = reader.ReadBoolean();
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                idleFrames = false;
                PlayAnimation("WindUp");
            }
            if (idleFrames)
            {
                attackFrames = false;
                if (!intangible)
                {
                    PlayAnimation("Idle");
                }
                else
                {
                    PlayAnimation("NoClip");
                }
            }
            if (grabbing)
            {
                idleFrames = false;
                attackFrames = false;
                secondaryAbilityFrames = false;
                PlayAnimation("Grab");
            }
            if (Main.mouseRight && Projectile.owner == Main.myPlayer && shootCount <= 0 && !grabbing)       //loops grabbing
            {
                if (Projectile.frame <= 0)
                {
                    Projectile.frame = 1;
                }
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 1;
                }
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/FollowMe/FollowMe_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 15, true);
            }
            if (animationName == "WindUp")
            {
                AnimateStand(animationName, 7, 6, false);
            }
            if (animationName == "Grab")
            {
                AnimateStand(animationName, 9, 12, false);
            }
            if (animationName == "NoClip")
            {
                AnimateStand(animationName, 5, 15, true);
            }
        }
    }
}