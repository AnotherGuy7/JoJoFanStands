using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using JoJoFanStands.Projectiles;
using JoJoFanStands.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace JoJoFanStands.Projectiles.PlayerStands.Expanses
{
    public class ExpansesStandFinal : StandClass
    {
        public override int ProjectileDamage => 80;
        public override int AltDamage => 150;
        public override int ShootTime => 4;
        public override StandAttackType StandType => StandAttackType.Ranged;
        public override Vector2 StandOffset => Vector2.Zero;
        public override int HalfStandHeight => 37;
        public override float MaxDistance => 0f;
	    private bool IsCrystallized = false;

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
                    shootVel *= 30f;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<CrystalProj>(), newProjectileDamage, 8f, Main.myPlayer);
                    Main.projectile[proj].netUpdate = true;
                    Projectile.netUpdate = true;
                }
            }
            if(player.ownedProjectileCounts[ModContent.ProjectileType<ColumnProj>()] == 0) {idleFrames = true;attackFrames = false;}
			
            if (Main.mouseRight && shootCount <= 0f && player.ownedProjectileCounts[ModContent.ProjectileType<ColumnProj>()] <= 3 && Projectile.owner == Main.myPlayer)
            {
				idleFrames = false;
                attackFrames = true;
				Vector2 shootVel = Main.MouseWorld - Projectile.Center;shootVel.Normalize();shootVel *= 25f;
				Projectile.ai[0] += 1f;
                int column = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<ColumnProj>(), (int)(AltDamage * mPlayer.standDamageBoosts), 8f, Main.myPlayer);
                Main.projectile[column].netUpdate = true;
			if (Projectile.ai[0]== 180f)
			{
                int columnb = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<ColumnProj>(), (int)(AltDamage * mPlayer.standDamageBoosts), 4f, Main.myPlayer);
                Main.projectile[columnb].netUpdate = true;
			}
			if (Projectile.ai[0] == 360f)
			{
                int columnc = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<ColumnProj>(), (int)(AltDamage * mPlayer.standDamageBoosts), 4f, Main.myPlayer);
                Main.projectile[columnc].netUpdate = true;
			}
			if (Projectile.ai[0] == 540f)
			{
                int columnd = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<ColumnProj>(), (int)(AltDamage * mPlayer.standDamageBoosts), 4f, Main.myPlayer);
                Main.projectile[columnd].netUpdate = true;
				Projectile.ai[0] = 0f;
			}
                Projectile.netUpdate = true;
			}
            if (SecondSpecialKeyPressed())
            {
		IsCrystallized = !IsCrystallized;
		if(!IsCrystallized){player.ClearBuff(ModContent.BuffType<SelfCrystallize>());}
		else{player.AddBuff(ModContent.BuffType<SelfCrystallize>(), 16000, true, false);}
            }
            if (SpecialKeyPressed())
            {
			player.position = Main.MouseWorld;
			SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
			player.AddBuff(JoJoFanStands.JoJoStandsMod.Find<ModBuff>("AbilityCooldown").Type, mPlayer.AbilityCooldownTime(5), true, false);
            }
	}
        public override void SelectAnimation()
        {
            if (idleFrames)
            {attackFrames = false;PlayAnimation("Idle");}
            if (attackFrames)
            {idleFrames = false;PlayAnimation("Attack");}
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().posing)
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