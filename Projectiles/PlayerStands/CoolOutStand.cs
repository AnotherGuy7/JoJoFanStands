using Microsoft.Xna.Framework;
using JoJoFanStands;
using Terraria;
using Terraria.ID;
using JoJoStands;
using Terraria.ModLoader;
 
namespace JoJoFanStands.Projectiles.PlayerStands
{  
    public class CoolOutStand : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 18;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 46;
            projectile.height = 64;
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

        public bool normalFrames = false;
        public bool attackFrames = false;
        public bool slamFrames = false;

        public int shootDamage = 17;
        public int altDamage = 20;
        public int specialDamage = 25;
        public bool altAttacking = false;
        public int proj = 0;
        public int projectilesChosen = 0;
        public int chance = 0;

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
            Fplayer.StandOut = true;
            if (player.HeldItem.type == mod.ItemType("CoolOutT1") && Fplayer.StandOut)
            {
                projectile.timeLeft = 2;
            }
            if (player.HeldItem.type != mod.ItemType("CoolOutT1") || !Fplayer.StandOut || player.dead)
            {
                projectile.active = false;
            }
            Vector2 vector131 = player.Center;
            vector131.X -= (float)((12 + player.width / 2) * player.direction);
            vector131.Y -= 25f;
            projectile.Center = Vector2.Lerp(projectile.Center, vector131, 0.2f);
            projectile.velocity *= 0.8f;
            projectile.direction = (projectile.spriteDirection = player.direction);
            Lighting.AddLight(projectile.position, 1.78f, 2.21f, 2.54f);
            if (Main.mouseLeft)
            {
                normalFrames = false;
                attackFrames = true;
                slamFrames = false;
                if (shootCool <= 0f)
                {
                    Main.PlaySound(SoundID.Item28, projectile.position);
                    shootCool += 30;
                    Vector2 shootVel = Main.MouseWorld - projectile.Center;
                    if (shootVel == Vector2.Zero)
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    shootVel.Normalize();
                    shootVel *= shootSpeed;
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, shootVel.X, shootVel.Y, mod.ProjectileType("IceBolt"), shootDamage, 8f, Main.myPlayer);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                normalFrames = true;
                attackFrames = false;
                slamFrames = false;
            }
            if (Main.mouseRight && shootCool <= 0f && player.ownedProjectileCounts[mod.ProjectileType("IceSpear")] == 0)
            {
                projectile.ai[0] += 0.005f;
                if (projectile.ai[0] >= 2f)
                {
                    player.AddBuff(BuffID.Chilled, 2);
                }
                if (projectile.ai[0] == 0.005f)
                {
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("IceSpear"), altDamage, 10f, Main.myPlayer, projectile.whoAmI);
                    Main.projectile[proj].netUpdate = true;
                    projectile.netUpdate = true;
                }
            }

            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed && shootCool <= 0f)
            {
                normalFrames = false;
                attackFrames = false;
                slamFrames = true;
                Main.PlaySound(SoundID.Item28, projectile.position);
                shootCool += 600;        //make it 600, 10 seconds
                int proj = Projectile.NewProjectile(projectile.Center.X + 7f, projectile.Center.Y + 3f, 0f, 0f, mod.ProjectileType("IceSpike"), specialDamage, 2f, Main.myPlayer);
                Main.projectile[proj].netUpdate = true;
                projectile.netUpdate = true;
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
                if (projectile.frame >= 8)
                {
                    projectile.frame = 0;
                }
            }
            if (attackFrames)
            {
                if (projectile.frameCounter >= 7)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 7)
                {
                    projectile.frame = 8;
                }
                if (projectile.frame <= 12)
                {
                    projectile.frame = 8;
                }
            }
            if (slamFrames)
            {
                if (projectile.frameCounter >= 10)
                {
                    projectile.frame += 1;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 14)
                {
                    projectile.frame = 15;
                }
                if (projectile.frame <= 18)
                {
                    projectile.frame = 15;
                }
            }
        }
    }
}