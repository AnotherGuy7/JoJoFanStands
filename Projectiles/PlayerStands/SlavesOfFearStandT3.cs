using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;

namespace JoJoFanStands.Projectiles.PlayerStands
{
    public class SlavesOfFearStandT3 : StandClass
    {
        public override string Texture
        {
            get { return mod.Name + "/Projectiles/PlayerStands/SlavesOfFearStand"; }
        }

        public override void SetStaticDefaults()
        {
            ///Main.projPet[projectile.type] = true;
            Main.projFrames[projectile.type] = 14;
        }

        public bool normalFrames = false;
        public bool attackFrames = false;
        public bool shuntFrames = false;
        public bool weldFrames = false;

        /*public Vector2 velocityAddition = Vector2.Zero;
        public float mouseDistance = 0f;
        protected float shootSpeed = 16f;
        public bool normalFrames = false;
        public bool attackFrames = false;
        public bool shuntFrames = false;
        public float maxDistance = 0f;
        public int punchDamage = 47;
        public int altDamage = 54;*/

        public override float maxDistance       
        {
            get { return 98f; }
        }

        public override int halfStandHeight => 37;          //a simpler version of doing the above

        public override int punchDamage => 68;          //this is just a test to see whether I really know enough
        public override int altDamage => 76;

        public override int punchTime => 11;

        public override bool useNormalDistance => false;


        public override void AI()
        {
            SelectFrame();
            shootCount--;
            Player player = Main.player[projectile.owner];
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            projectile.frameCounter++;
            fPlayer.StandOut = true;
            if (player.HeldItem.type == mod.ItemType("SlavesOfFearT3") && fPlayer.StandOut)
            {
                projectile.timeLeft = 2;
            }
            if (player.HeldItem.type != mod.ItemType("SlavesOfFearT3") || !fPlayer.StandOut || player.dead)
            {
                projectile.active = false;
            }

            if (Main.mouseLeft)
            {
                attackFrames = true;
                normalFrames = false;
                shuntFrames = false;
                Main.mouseRight = false;
                Punch();
            }
            else
            {
                if (!shuntFrames && !weldFrames)
                {
                    attackFrames = false;
                    normalFrames = true;
                    shuntFrames = false;
                    StayBehind();
                }
            }
            if (Main.mouseRight)
            {
                shuntFrames = true;
            }
            weldFrames = JoJoStands.JoJoStands.SpecialHotKey.Current;
            if (weldFrames)
            {
                shuntFrames = false;
                attackFrames = false;
                normalFrames = false;
                if (projectile.Distance(Main.MouseWorld) > 40f)
                {
                    projectile.velocity = Main.MouseWorld - projectile.Center;
                    projectile.velocity.Normalize();
                    projectile.velocity *= 2f;
                }
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && projectile.Distance(npc.Center) <= 40f)
                    {
                        npc.GetGlobalNPC<NPCs.FanGlobalNPC>().welded = true;
                        npc.GetGlobalNPC<NPCs.FanGlobalNPC>().weldMaxTimer = 600;
                    }
                }
            }
            Vector2 direction = player.Center - projectile.Center;
            float distanceTo = direction.Length();
            if (shuntFrames)
            {
                normalFrames = false;
                attackFrames = false;
                projectile.velocity.X = 10f * projectile.direction;
                projectile.position.Y = player.position.Y;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && projectile.Distance(npc.Center) <= 15f)
                    {
                        npc.StrikeNPC(altDamage, 8f, projectile.direction);
                    }
                }
                if (distanceTo > maxDistance * 2)
                {
                    shuntFrames = false;
                }
            }
            else
            {
                if (distanceTo > maxDistance)
                {
                    if (projectile.position.X <= player.position.X - 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X + 0.2f, projectile.position.Y);
                        projectile.velocity = Vector2.Zero;
                    }
                    if (projectile.position.X >= player.position.X + 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X - 0.2f, projectile.position.Y);
                        projectile.velocity = Vector2.Zero;
                    }
                    if (projectile.position.Y >= player.position.Y + 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X, projectile.position.Y - 0.2f);
                        projectile.velocity = Vector2.Zero;
                    }
                    if (projectile.position.Y <= player.position.Y - 15f)
                    {
                        projectile.position = new Vector2(projectile.position.X, projectile.position.Y + 0.2f);
                        projectile.velocity = Vector2.Zero;
                    }
                }
                if (distanceTo >= maxDistance + 22f)
                {
                    Main.mouseLeft = false;
                    Main.mouseRight = false;
                    projectile.Center = player.Center;
                }
            }
        }

        public virtual void SelectFrame()
        {
            projectile.frameCounter++;
            if (attackFrames)
            {
                normalFrames = false;
                if (projectile.frameCounter >= 12)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame <= 1)
                {
                    projectile.frame = 2;
                }
                if (projectile.frame >= 6)
                {
                    projectile.frame = 2;
                }
            }
            if (normalFrames)
            {
                if (projectile.frameCounter >= 30)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 2)
                {
                    projectile.frame = 0;
                }
            }
            if (shuntFrames)
            {
                if (projectile.frameCounter >= 10)
                {
                    if (projectile.frame == 6)
                    {
                        projectile.frame += 1;
                    }
                    projectile.frameCounter = 0;
                    if (projectile.frame == 7)
                    {
                        projectile.frame = 8;
                    }
                    if (projectile.frame == 8)
                    {
                        projectile.frame = 7;
                    }
                }
                if (projectile.frame <= 5)
                {
                    projectile.frame = 6;
                }
                if (projectile.frame >= 9)
                {
                    projectile.frame = 6;
                }
            }
            if (weldFrames)
            {
                if (projectile.frameCounter >= 13)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame <= 8)
                {
                    projectile.frame = 9;
                }
                if (projectile.frame >= 14)
                {
                    projectile.frame = 11;
                }
            }
        }
    }
}