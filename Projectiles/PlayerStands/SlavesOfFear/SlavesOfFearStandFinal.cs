using Microsoft.Xna.Framework;
using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using System.IO;
using JoJoFanStands.NPCs;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace JoJoFanStands.Projectiles.PlayerStands.SlavesOfFear
{
    public class SlavesOfFearStandFinal : StandClass
    {
        public override int HalfStandHeight => 37;
        public override int PunchDamage => 98;
        public override int AltDamage => 112;
        public override int PunchTime => 10;
        public override int TierNumber => 4;
        public override StandAttackType StandType => StandAttackType.Melee;

        private bool weldFrames = false;


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

            if (Main.mouseLeft)
            {
                attackFrames = true;
                Punch();
            }
            else
            {
                if (!secondaryAbilityFrames && !weldFrames)
                {
                    idleFrames = true;
                    StayBehind();
                }
            }
            if (Main.mouseRight)
            {
                secondaryAbilityFrames = true;
            }
            weldFrames = JoJoStands.JoJoStands.SpecialHotKey.Current;
            if (weldFrames)
            {
                secondaryAbilityFrames = false;
                if (Projectile.Distance(Main.MouseWorld) > 2f * 16f)
                {
                    Projectile.velocity = Main.MouseWorld - Projectile.Center;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 2f;
                }
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && Projectile.Distance(npc.Center) <= 3f * 16f)
                    {
                        npc.GetGlobalNPC<FanGlobalNPC>().welded = true;
                        npc.GetGlobalNPC<FanGlobalNPC>().weldMaxTimer = 15 * 60;
                    }
                }
            }
            Vector2 direction = player.Center - Projectile.Center;
            float distanceTo = direction.Length();
            if (secondaryAbilityFrames)
            {
                Projectile.velocity.X = 10f * Projectile.direction;
                Projectile.position.Y = player.position.Y;
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active && Projectile.Distance(npc.Center) <= 15f)
                    {
                        npc.StrikeNPC(AltDamage, 8f, Projectile.direction);
                    }
                }
                if (distanceTo > MaxDistance * 2)
                {
                    secondaryAbilityFrames = false;
                }
            }
            else
            {
                LimitDistance();
            }
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(weldFrames);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            weldFrames = reader.ReadBoolean();
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                idleFrames = false;
                PlayAnimation("Attack");
            }
            if (idleFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
            if (secondaryAbilityFrames)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Secondary");
            }
            if (weldFrames)
            {
                attackFrames = false;
                idleFrames = false;
                secondaryAbility = false;
                PlayAnimation("Weld");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/SlavesOfFear/SlavesOfFear_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 2, 14, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 4, newPunchTime, true);
            }
            if (animationName == "Secondary")
            {
                AnimateStand(animationName, 2, 10, true);
            }
            if (animationName == "Weld")
            {
                AnimateStand(animationName, 4, 13, true);
            }
        }
    }
}