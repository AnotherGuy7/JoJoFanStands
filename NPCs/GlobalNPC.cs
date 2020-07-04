using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.NPCs
{
    public class FanGlobalNPC : GlobalNPC
    {
        public bool welded = false;
        public int weldTimer = 0;
        public int weldMaxTimer = 0;
        public bool nonExistant = false;
        public bool affectedByAvalance = false;
        public int icicleTimer = 0;
        public static List<int> nonExistantTypes = new List<int>(5);

        public override bool InstancePerEntity
        {
            get { return true; }
        }

        public override bool PreAI(NPC npc)
        {
            FanPlayer FPlayer = Main.LocalPlayer.GetModPlayer<FanPlayer>();
            /*if (FPlayer.avalanche)
            {
                affectedByAvalance = true;
            }*/
            if (welded)
            {
                npc.velocity = Vector2.Zero;
                weldTimer++;
                if (weldTimer >= weldMaxTimer)
                {
                    weldTimer = 0;
                    weldMaxTimer = 0;
                    welded = false;
                }
                return false;
            }
            /*if (affectedByAvalance)
            {
                icicleTimer++;
                npc.velocity = Vector2.Zero;
                npc.color = Color.LightBlue;
                if (icicleTimer == 170)
                {
                    Projectile.NewProjectile(npc.position.X, npc.position.Y - Main.screenHeight, 0f, 5f, mod.ProjectileType("Icicle"), 23, 1f, Main.myPlayer);
                }
                if (icicleTimer == 175)
                {
                    Projectile.NewProjectile(npc.position.X, npc.position.Y - Main.screenHeight, 0f, 5f, mod.ProjectileType("Icicle"), 23, 1f, Main.myPlayer);
                }
                if (icicleTimer == 180)
                {
                    Projectile.NewProjectile(npc.position.X, npc.position.Y - Main.screenHeight, 0f, 5f, mod.ProjectileType("Icicle"), 23, 1f, Main.myPlayer);
                    FPlayer.avalanche = false;
                }
                if (icicleTimer >= 200)
                {
                    affectedByAvalance = false;
                }
                return false;
            }*/
            return true;
        }

        public override bool CheckActive(NPC npc)
        {
            for (int i = 0; i < nonExistantTypes.Count; i++)
            {
                if (npc.type == nonExistantTypes[i])
                {
                    npc.active = false;
                }
            }
            return true;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (welded)
                npc.color = Color.DarkGray;
            else
                npc.color = drawColor;
        }
    }
}