using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
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
        public override int ProjectileDamage => 16;
        public override int AltDamage => 40;
        public override int ShootTime => 13;
        public override StandAttackType StandType => StandAttackType.Ranged;
        public override Vector2 StandOffset => Vector2.Zero;
        public override int HalfStandHeight => 37;
        public override float MaxDistance => 0f;
        private float[] crystalRotations;

        public override void ExtraSpawnEffects()
        {
            crystalRotations = new float[4];
            for (int i = 0; i < crystalRotations.Length; i++)
            {
                crystalRotations[i] = MathHelper.PiOver2 * i;
            }
        }

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

            for (int i = 0; i < crystalRotations.Length; i++)
            {
                crystalRotations[i] -= MathHelper.PiOver4 / 16f;
                if (crystalRotations[i] <= -MathHelper.TwoPi)
                    crystalRotations[i] = 0f;
            }
            if (Main.mouseLeft)
            {
                if (shootCount <= 0f)
                {
                    SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                    shootCount += newShootTime;
                    Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                    if (shootVel == Vector2.Zero) { shootVel = new Vector2(0f, 1f); }
                    shootVel.Normalize();
                    shootVel *= 9f;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<CrystalProj>(), newProjectileDamage, 2f, Main.myPlayer);
                    Main.projectile[proj].netUpdate = true;
                    Projectile.netUpdate = true;
                }
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ColumnProj>()] == 0) { idleFrames = true; attackFrames = false; }
            if (Main.mouseRight && shootCount <= 0f && Projectile.owner == Main.myPlayer)
            {
                idleFrames = false;
                attackFrames = true;
                Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                shootVel.Normalize();
                shootVel *= 13f;
                Projectile.ai[0] += 1;
                if (Projectile.ai[0] >= 90)
                {
                    int columnb = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<ColumnProj>(), (int)(AltDamage * mPlayer.standDamageBoosts), 8f, Main.myPlayer);
                    Main.projectile[columnb].netUpdate = true;
                    Projectile.ai[0] = 0;
                }
                Projectile.netUpdate = true;
            }
            else
                Projectile.ai[0] = 0f;
        }

        public override void SelectAnimation()
        {
            if (idleFrames || attackFrames)
            { PlayAnimation("Idle"); }
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().posing)
            { idleFrames = false; PlayAnimation("Pose"); }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Expanses/Expanses_" + animationName).Value;
            if (animationName == "Idle")
            { AnimateStand(animationName, 1, 15, true); }
            if (animationName == "Pose")
            { AnimateStand(animationName, 1, 15, true); }
        }

        private Texture2D crystalTexture;
        private readonly Vector2 crystalOrigin = new Vector2(5, 20);

        public override bool PreDraw(ref Color drawColor)
        {
            if (crystalTexture == null)
                crystalTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Expanses/CrystalPillar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            for (int i = 0; i < crystalRotations.Length; i++)
            {
                if (crystalRotations[i] < -MathHelper.Pi)
                    continue;

                Vector2 drawPosition = Projectile.Center - Main.screenPosition;
                drawPosition += crystalRotations[i].ToRotationVector2() * new Vector2(20f, 17f);

                Main.EntitySpriteDraw(crystalTexture, drawPosition, null, drawColor, Projectile.rotation, crystalOrigin, 1f, effects, 0);
            }

            return base.PreDraw(ref drawColor);
        }

        public override void PostDraw(Color drawColor)
        {
            if (crystalTexture == null)
                crystalTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Expanses/CrystalPillar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            for (int i = 0; i < crystalRotations.Length; i++)
            {
                if (crystalRotations[i] > -MathHelper.Pi)
                    continue;

                Vector2 drawPosition = Projectile.Center - Main.screenPosition;
                drawPosition += crystalRotations[i].ToRotationVector2() * new Vector2(20f, 17f);

                Main.EntitySpriteDraw(crystalTexture, drawPosition, null, drawColor, Projectile.rotation, crystalOrigin, 1f, effects, 0);
            }
            base.PostDraw(drawColor);
        }
    }
}