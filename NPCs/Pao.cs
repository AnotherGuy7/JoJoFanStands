using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.NPCs
{
    public class Pao : ModNPC
    {
        private int frame = 0;
        private bool timeToDie = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.defense = 999999999;
            NPC.lifeMax = 999999999;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 2f;
            NPC.chaseable = false;
            NPC.aiStyle = 0;
            NPC.immortal = true;
        }

        public override bool CheckDead()
        {
            if (!timeToDie)
            {
                int npc2 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.position.X, (int)NPC.position.Y, NPC.type);
                int npc3 = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.position.X, (int)NPC.position.Y, NPC.type);
                Main.npc[npc2].GivenName = "Pao Alt #" + NPC.whoAmI;
                Main.npc[npc3].GivenName = "Pao Alt #" + NPC.whoAmI;
                Main.NewText("You can't get rid of me that easily.", Color.DarkRed);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            Vector2 vector131 = player.Center;
            vector131.X -= (float)((12 + player.width / 2) * player.direction);
            NPC.Center = Vector2.Lerp(NPC.Center, vector131, 0.2f);
            NPC.velocity *= 0.8f;
            NPC.direction = (NPC.spriteDirection = player.direction);
            NPC.ai[0]++;
            NPC.ai[2]++;
            if (NPC.ai[2] >= 72000)
            {
                timeToDie = true;
                int proj = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, Vector2.Zero, ProjectileID.Bomb, 250, 10f);
                Main.projectile[proj].timeLeft = 2;
                Main.NewText("Bye dad.");
                NPC.StrikeNPC(NPC.life + 1, 999f, 1, true);
            }
            if (NPC.ai[0] >= NPC.ai[1])
            {
                NPC.ai[0] = 0;
                NPC.ai[1] = Main.rand.NextFloat(120f, 450f);
                int randText = Main.rand.Next(1, 12);
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
                if (randText == 10)
                {
                    Main.NewText("Dead.");
                }
                if (randText == 11)
                {
                    Main.NewText("How die???");
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (FanPlayer.SpawnPao)
                return 0.02f;

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            frameHeight = 20;
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20)
            {
                frame += 1;
                NPC.frameCounter = 0;
            }
            if (frame >= 2)
            {
                frame = 0;
            }
            NPC.frame.Y = frame * frameHeight;
        }
    }
}