using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using System;

namespace JoJoFanStands.Projectiles
{
    public class ColumnProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.aiStyle = 0; 
            Projectile.height = 40; 
            Projectile.friendly  = true;
            Projectile.timeLeft = 50;
            Projectile.hostile = false;
            Projectile.penetrate  = -1;
	    Projectile.ignoreWater = true;
	    Projectile.tileCollide = false;
        }
		public override void AI(){Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {target.immune[Projectile.owner] = 0;}
    }
}