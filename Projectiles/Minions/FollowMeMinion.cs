using Microsoft.Xna.Framework;
using JoJoFanStands;
using Terraria;
using Terraria.ID;
using JoJoStands;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Projectiles.Minions
{  
    public class FollowMeMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 6;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 42;
            projectile.height = 58;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.netImportant = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            MyPlayer.stopImmune.Add(mod.ProjectileType(Name));
        }

        protected float shootCool = 12f;
        protected float shootSpeed = 12f;
        public int windUpTime = 0;
        public int windUpPower = 0;
        public int abilityNumber = 0;
        public bool saidAbility = false;
        public bool windUpRelease = false;
        public bool normalFrames = false;
        public bool attackFrames = false;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FanPlayer Fplayer = player.GetModPlayer<FanPlayer>();
            SelectFrame();
            shootCool--;
            if (shootCool <= 0f)
            {
                shootCool = 0f;
            }
            if (Fplayer.FollowMeActive)
            {
                projectile.timeLeft++;
            }
            else
            {
                projectile.Kill();
            }
            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed && !saidAbility)
            {
                abilityNumber += 1;
                saidAbility = true;
            }
            if (abilityNumber == 0 && saidAbility)
            {
                Main.NewText("Right-Click: Wind Up Punch");
                saidAbility = false;
            }
            if (abilityNumber == 1 && saidAbility)
            {
                Main.NewText("Right-Click: Enemy Grab");
                saidAbility = false;
            }
            if (abilityNumber == 2 && saidAbility)
            {
                Main.NewText("Right-Click: Intangible");
                saidAbility = false;
            }
            if (abilityNumber >= 3)
            {
                abilityNumber = 0;
            }
            Vector2 vector131 = player.Center;
            vector131.X -= (float)((12 + player.width / 2) * player.direction);
            vector131.Y -= 25f;
            projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
            projectile.velocity *= 0.8f;
            projectile.direction = (projectile.spriteDirection = player.direction);
            Lighting.AddLight(projectile.position, 1.78f, 2.21f, 2.54f);
            if (Main.mouseLeft && shootCool <= 0f)
            {
                Main.PlaySound(SoundID.Item28, projectile.position);
                shootCool += 24;
                Vector2 shootVel = Main.MouseWorld - projectile.Center;
                if (shootVel == Vector2.Zero)
                {
                    shootVel = new Vector2(0f, 1f);
                }
                shootVel.Normalize();
                shootVel *= shootSpeed;
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, mod.ProjectileType("FollowMeFist"), 62, 3f, Main.myPlayer);
                Main.projectile[proj].netUpdate = true;
                projectile.netUpdate = true;
            }
            if (Main.mouseRight && shootCool <= 0f && abilityNumber == 0)
            {
                normalFrames = false;
                attackFrames = true;
                windUpTime++;
                if (windUpTime >= 60)
                {
                    windUpTime = 0;
                    windUpPower += 1;
                    Main.NewText(windUpPower);
                }
            }
            else
            {
                windUpTime = 0;
                normalFrames = true;
                attackFrames = false;
            }
            if (windUpRelease)
            {
                Main.PlaySound(SoundID.Item28, projectile.position);
                shootCool += 240;
                Vector2 shootVel = Main.MouseWorld - projectile.Center;
                if (shootVel == Vector2.Zero)
                {
                    shootVel = new Vector2(0f, 1f);
                }
                shootVel.Normalize();
                shootVel *= shootSpeed;
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, mod.ProjectileType("FollowMeFist"), 62 * windUpPower, 10f + windUpPower, Main.myPlayer);
                Main.projectile[proj].netUpdate = true;
                projectile.netUpdate = true;
                windUpRelease = false;
                windUpPower = 0;
            }
            if (Main.mouseRight && shootCool <= 0f && abilityNumber == 1)
            {

            }
            else
            {
                normalFrames = true;
                attackFrames = false;
            }
            if (Main.mouseRight && shootCool <= 0f && abilityNumber == 2)
            {
                Main.player[projectile.owner].AddBuff(mod.BuffType("Intangible"), 1200);
            }
        }

        public virtual void SelectFrame()
        {
            projectile.frameCounter++;
            if (normalFrames)
            {
                if (projectile.frameCounter >= 12)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 5)
                {
                    projectile.frame = 0;
                }
            }
            if (attackFrames)
            {
                projectile.frame = 5;
            }
        }
    }
}