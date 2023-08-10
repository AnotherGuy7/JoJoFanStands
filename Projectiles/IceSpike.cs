using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles
{
    public class IceSpike : ModProjectile
    {
        private float Yaddition = 0;
        private bool spawnedAnother = false;
        private bool canSpawn = false;
        private int moveDistanceX = 16;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.5f;
        }

        public override void AI()
        {
            Projectile ownerProj = Main.projectile[(int)Projectile.ai[0]];

            Projectile.direction = (int)Projectile.ai[1];
            Projectile.velocity.Y += 5f;
            Projectile.scale -= 0.025f;     //0.025f obtained by doing (max scale - max time left)
            DrawOriginOffsetY = 8 + (int)((1.5f - Projectile.scale) * 20f);
            if (!ownerProj.active || Projectile.scale <= 0f)
                Projectile.Kill();

            /*if (Collision.SolidTiles((int)Projectile.position.X / 16, (int)(Projectile.position.X + moveDistanceX) / 16, (int)Projectile.position.Y / 16, (int)(Projectile.position.Y - Yaddition) / 16))
            {
                spawnedAnother = true;
                Yaddition += 1;
            }*/
            bool colliding = Collision.SolidTiles(new Vector2((int)(Projectile.position.X + (moveDistanceX * Projectile.direction)) / 16, (int)(Projectile.position.Y - Yaddition) / 16), Projectile.width, Projectile.height);
            if (!colliding)
                canSpawn = true;
            else
                Yaddition += 1;

            if (canSpawn && !spawnedAnother && Projectile.timeLeft <= 58)        //&& Projectile.timeLeft <= 5
            {
                spawnedAnother = true;
                canSpawn = false;
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + (8f * Projectile.direction), Projectile.Center.Y - Yaddition, 0f, 0f, Mod.Find<ModProjectile>("IceSpike").Type, Projectile.damage, 2f, Main.myPlayer, ownerProj.whoAmI, Projectile.direction);
                Main.projectile[proj].direction = Projectile.direction;
                Main.projectile[proj].netUpdate = true;
            }
        }

        private Texture2D iceSpikeTexture;
        private readonly Vector2 IceSpikeOrigin = new Vector2(6.5f, 20f);

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Main.dedServ && iceSpikeTexture == null)
                iceSpikeTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/IceSpike", AssetRequestMode.ImmediateLoad).Value;

            Main.EntitySpriteDraw(iceSpikeTexture, Projectile.position + IceSpikeOrigin - Main.screenPosition, null, lightColor, 0f, IceSpikeOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}