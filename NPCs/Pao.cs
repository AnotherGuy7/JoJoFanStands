using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.NPCs
{
    public class Pao : ModNPC
    {
        public int frame = 0;
        public bool timeToDie = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 32;
            npc.defense = 999999999;
            npc.lifeMax = 999999999;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 2f;
            npc.chaseable = false;
            npc.aiStyle = 0;
            npc.immortal = true;
        }

        public override bool CheckDead()
        {
            if (!timeToDie)
            {
                int npc2 = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, npc.type);
                int npc3 = NPC.NewNPC((int)npc.position.X, (int)npc.position.Y, npc.type);
                Main.npc[npc2].GivenName = "Pao Alt #" + npc.whoAmI;
                Main.npc[npc3].GivenName = "Pao Alt #" + npc.whoAmI;
                Main.NewText("I can't be rid of that easily.", Color.DarkRed);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];
            Vector2 vector131 = player.Center;
            vector131.X -= (float)((12 + player.width / 2) * player.direction);
            npc.Center = Vector2.Lerp(npc.Center, vector131, 0.2f);
            npc.velocity *= 0.8f;
            npc.direction = (npc.spriteDirection = player.direction);
            npc.ai[0]++;
            npc.ai[2]++;
            if (npc.ai[2] >= 72000)
            {
                timeToDie = true;
                int proj = Projectile.NewProjectile(npc.position, Vector2.Zero, ProjectileID.Bomb, 250, 10f);
                Main.projectile[proj].timeLeft = 2;
                Main.NewText("Bye dad.");
                npc.StrikeNPC(npc.life + 1, 999f, 1, true);
            }
            if (npc.ai[0] >= npc.ai[1])
            {
                npc.ai[0] = 0;
                npc.ai[1] = Main.rand.NextFloat(120f, 450f);
                int randText = Main.rand.Next(1, 10);
                if (randText == 1)
                {
                    Main.NewText("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGGGGGGGGGGGGGGGGGGGGGGGHHHHHHHHHHHHHHHHH");
                }
                if (randText == 2)
                {
                    Main.NewText("You're a NICE CAR");
                }
                if (randText == 3)
                {
                    Main.NewText("Give me admin... OR DIE!");
                }
                if (randText == 4)
                {
                    Main.NewText("SOPA DE MACACO!");
                }
                if (randText == 5)
                {
                    Main.NewText("Uma delicia.");
                }
                if (randText == 6)
                {
                    Main.NewText("Give me ULF Ratt");
                }
                if (randText == 7)
                {
                    Main.NewText("Got any traps???");
                }
                if (randText == 8)
                {
                    Main.NewText("Hi dad.");
                }
                if (randText == 9)
                {
                    Main.NewText("ghlaghaoigh eioh ngia ophgoa hg haoiegh auoguh augn hgioagh oiahn goea");
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (FanPlayer.spawnPao)
            {
                return 0.05f;
            }
            else
            {
                return 0f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            frameHeight = 20;
            npc.frameCounter++;
            if (npc.frameCounter >= 20)
            {
                frame += 1;
                npc.frameCounter = 0;
            }
            if (frame >= 2)
            {
                frame = 0;
            }
            npc.frame.Y = frame * frameHeight;
        }
    }
}