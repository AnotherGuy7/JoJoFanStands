using JoJoFanStands.Buffs;
using JoJoStands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class JoJoGlobalProjectile : GlobalProjectile
    {
        public bool slowedByBlur = false;
        public Vector2 slowedVelocity;

        public override bool InstancePerEntity
        {
            get { return true; }
        }

        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            if (fPlayer.blurLightningFastReflexes && !player.HasBuff(ModContent.BuffType<LightningFastReflex>()))
            {
                if (!slowedByBlur)
                {
                    projectile.velocity *= 0.1f;
                    slowedVelocity = projectile.velocity;
                    slowedByBlur = true;
                }
                if (slowedVelocity != projectile.velocity)
                {
                    Vector2 difference = projectile.velocity - slowedVelocity;
                    difference *= 0.1f;
                    slowedVelocity += difference;
                    projectile.velocity = slowedVelocity;
                }
            }
            else
            {
                if (slowedByBlur)
                {
                    projectile.velocity *= 10f;
                    slowedByBlur = true;
                }
            }
            if (fPlayer.blurInfiniteVelocity && !player.HasBuff(ModContent.BuffType<InfiniteVelocity>()))
            {
                if (projectile.timeLeft < 5)
                    projectile.timeLeft = 5;
                return false;
            }

            return true;
        }

        public override bool ShouldUpdatePosition(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (fPlayer.blurInfiniteVelocity && !player.HasBuff(ModContent.BuffType<InfiniteVelocity>()))
                return false;

            return true;
        }
    }
}