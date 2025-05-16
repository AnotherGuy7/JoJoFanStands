using JoJoFanStands.Buffs;
using JoJoStands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.GlueManDir
{
    public class GlueBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        private bool spawned = false;
        private const float ExplosionRadius = 2f * 16f;

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;

                if (Projectile.frame >= 4)
                    Projectile.Kill();
            }

            if (!spawned)
            {
                spawned = true;
                for (int i = 0; i < 20; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Pearlsand, Alpha: 100, Scale: 1.5f);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].velocity *= 3f;
                }
                for (int n = 0; n < Main.maxNPCs; n++)
                {
                    NPC npc = Main.npc[n];
                    if (npc.active)
                    {
                        if (npc.lifeMax > 5 && !npc.friendly && !npc.hide && !npc.immortal && npc.Distance(Projectile.Center) <= ExplosionRadius + (2 * Main.player[Projectile.owner].GetModPlayer<MyPlayer>().standTier * 16f))
                        {
                            int hitDirection = -1;
                            if (npc.position.X - Projectile.position.X > 0)
                                hitDirection = 1;

                            NPC.HitInfo hitInfo = new NPC.HitInfo()
                            {
                                Damage = Projectile.damage,
                                Knockback = 1f,
                                HitDirection = hitDirection,
                                Crit = false
                            };
                            npc.StrikeNPC(hitInfo);
                            npc.AddBuff(ModContent.BuffType<GlueStuck>(), 2 * Main.player[Projectile.owner].GetModPlayer<MyPlayer>().standTier * 60);
                            npc.AddBuff(BuffID.Ichor, 2 * Main.player[Projectile.owner].GetModPlayer<MyPlayer>().standTier * 60);
                            SoundEngine.PlaySound(GlueMan.GlueStick, Projectile.Center);
                        }
                    }
                }
                SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
            }
        }
    }
}