using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using JoJoFanStands.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.Expanses
{
    public class ExpansesStandT2 : StandClass
    {
        public override int projectileDamage => 16;
        public override int altDamage => 40;
        public override int shootTime => 10;
        public override StandType standType => StandType.Ranged;
        public override int standOffset => 0;
        public override int halfStandHeight => 37;
        public override float maxDistance => 0f;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            SelectAnimation();
            UpdateStandInfo();
		base.Projectile.position = player.Center - new Vector2((float)base.Projectile.width / 2f, (float)player.height + 20f);
		base.Projectile.spriteDirection = player.direction;
            Lighting.AddLight(Projectile.position, 974);
            if (shootCount > 0) shootCount--;
            if (mPlayer.standOut) Projectile.timeLeft = 2;
            if (Main.mouseLeft )
            {
                if (shootCount <= 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                    shootCount += newShootTime;
                    Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                    if (shootVel == Vector2.Zero){shootVel = new Vector2(0f, 1f);}
                    shootVel.Normalize();
                    shootVel *= 20f;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<CrystalProj>(), newProjectileDamage, 8f, Main.myPlayer);
                    Main.projectile[proj].netUpdate = true;
                    Projectile.netUpdate = true;
                }
            }
            if(player.ownedProjectileCounts[ModContent.ProjectileType<ColumnProj>()] == 0) {idleFrames = true;attackFrames = false;}
            if (Main.mouseRight && shootCount <= 0f && player.ownedProjectileCounts[ModContent.ProjectileType<ColumnProj>()] == 0 && Projectile.owner == Main.myPlayer)
            {
		idleFrames = false;
                attackFrames = true;
		Vector2 shootVel = Main.MouseWorld - Projectile.Center;shootVel.Normalize();shootVel *= 25f;
		Projectile.ai[0] += 1f;
                int column = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<ColumnProj>(), (int)(altDamage * mPlayer.standDamageBoosts), 8f, Main.myPlayer);
                Main.projectile[column].netUpdate = true;
                Projectile.netUpdate = true;
		if (Projectile.ai[0]== 180f)
		{
                int columnb = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<ColumnProj>(), (int)(altDamage * mPlayer.standDamageBoosts), 4f, Main.myPlayer);
                Main.projectile[columnb].netUpdate = true;
		}
                Projectile.netUpdate = true;
	    }
	}
        public override void SelectAnimation()
        {
            if (idleFrames)
            {attackFrames = false;PlayAnimation("Idle");}
            if (attackFrames)
            {idleFrames = false;PlayAnimation("Attack");}
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {idleFrames = false; PlayAnimation("Pose");}
        }
        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Expanses/Expanses_" + animationName).Value;
            if (animationName == "Idle")
            {AnimateStand(animationName, 16, 7, true);}
            if (animationName == "Attack")
            {AnimateStand(animationName, 16, 7, true);}
            if (animationName == "Pose")
            {AnimateStand(animationName, 1, 1, true);}
        }
    }
}