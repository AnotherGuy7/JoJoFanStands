using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using JoJoStands;
using JoJoFanStands.Buffs;
using System;

namespace JoJoFanStands.Projectiles
{
    public class CrystalProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 10; 
            Projectile.aiStyle = 0; 
            Projectile.friendly  = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = true;
	    Projectile.ignoreWater = true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {target.AddBuff(BuffID.Bleeding, 360);}
		public override void AI()
		{Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);}
    }
}