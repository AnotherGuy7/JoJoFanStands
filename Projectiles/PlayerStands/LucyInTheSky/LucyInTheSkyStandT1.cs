using Terraria;
using JoJoStands;
using JoJoStands.Projectiles.PlayerStands;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace JoJoFanStands.Projectiles.PlayerStands.LucyInTheSky
{
    public class LucyInTheSkyStandT1 : StandClass
    {
        public override int punchDamage => 23;
        public override int punchTime => 13;
        public override int altDamage => 96;
        public override int halfStandHeight => 37;
        public override int standOffset => 0;
        public override StandType standType => StandType.Melee;

        private const int MaxAmountOfMarkers = 3;

        private int amountOfBridges = 0;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;

            if (Main.mouseLeft)
            {
                Punch();
            }
            else
            {
                StayBehind();
                Projectile.position.X += 4f * -Projectile.direction;
            }

            if (Main.mouseRight && shootCount <= 0 && player.whoAmI == Main.myPlayer)
            {
                Vector3 lightLevel = Lighting.GetColor((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16).ToVector3();       //1.703 is max light
                if (lightLevel.Length() > 1.3f && amountOfBridges < MaxAmountOfMarkers)
                {
                    int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld - new Vector2(8f), Vector2.Zero, ModContent.ProjectileType<LightMarker>(), 0, 0f, Projectile.owner, amountOfBridges);
                    amountOfBridges += 1;
                    player.GetModPlayer<FanPlayer>().lucySelectedMarkerWhoAmI = index;
                }
                else
                {
                    //MarkerMenu.Show();
                }
                shootCount += 30;
            }
            LimitDistance();
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                idleFrames = false;
                PlayAnimation("Attack");
            }
            if (idleFrames)
            {
                attackFrames = false;
                PlayAnimation("Idle");
            }
            if (Main.player[Projectile.owner].GetModPlayer<MyPlayer>().poseMode)
            {
                idleFrames = false;
                attackFrames = false;
                PlayAnimation("Pose");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/LucyInTheSky/LucyInTheSky_" + animationName).Value;
            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 15, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 4, newPunchTime, true);
            }
            if (animationName == "Pose")
            {
                AnimateStand(animationName, 1, 60, true);
            }
        }
    }
}