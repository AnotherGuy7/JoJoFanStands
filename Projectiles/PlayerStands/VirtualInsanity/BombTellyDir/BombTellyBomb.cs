using JoJoStands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.BombTellyDir
{
    public class BombTellyBomb : ModProjectile
    {
        public static readonly SoundStyle ExplosionSound = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/BombDrone/Impact")
        {
            Volume = JoJoStands.JoJoStands.ModSoundsVolume
        };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 10 * 60;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.3f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;

                if (Projectile.frame >= 6)
                    Projectile.frame = 0;
            }
        }

        private const float ExplosionRadius = 6f * 16f;

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, Alpha: 100, Scale: 1.5f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            for (int i = 0; i < 20; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Alpha: 100, Scale: 3.5f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 7f;
                dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, Alpha: 100, Scale: 1.5f);
                Main.dust[dustIndex].velocity *= 3f;
            }
            bool crit = false;
            MyPlayer mPlayer = Main.player[Projectile.owner].GetModPlayer<MyPlayer>();
            if (Main.rand.Next(1, 100 + 1) <= mPlayer.standCritChangeBoosts)
                crit = true;
            for (int n = 0; n < Main.maxNPCs; n++)
            {
                NPC npc = Main.npc[n];
                if (npc.active)
                {
                    if (npc.lifeMax > 5 && !npc.friendly && !npc.hide && !npc.immortal && npc.Distance(Projectile.Center) <= ExplosionRadius)
                    {
                        int hitDirection = -1;
                        if (npc.position.X - Projectile.position.X > 0)
                            hitDirection = 1;

                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                        {
                            Damage = Projectile.damage,
                            Knockback = 4f,
                            HitDirection = hitDirection,
                            Crit = crit
                        };
                        npc.StrikeNPC(hitInfo);
                    }
                }
            }
            if (JoJoFanStands.SoundsLoaded)
                SoundEngine.PlaySound(ExplosionSound, Projectile.Center);
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().standTier >= 3)
            {
                int projectileIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 292, Projectile.damage, 0f, Projectile.owner);
                Main.projectile[projectileIndex].hostile = false;
                Main.projectile[projectileIndex].friendly = true;
            }
        }
    }
}