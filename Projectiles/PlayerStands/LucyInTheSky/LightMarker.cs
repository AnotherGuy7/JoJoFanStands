using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.LucyInTheSky
{
    public class LightMarker : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 1500;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.alpha = 0;
        }

        private int sinTimer = 0;
        private bool transportingToSelectedMarker = false;
        private int deathDetectionTime;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            sinTimer++;
            if (sinTimer >= 360)
                sinTimer = 0;
            Projectile.timeLeft = 2;
            if (deathDetectionTime > 0)
                deathDetectionTime--;

            if (Projectile.Distance(player.Center) > Main.screenHeight)     //Sometimes when loading tiles, the lighting doesn't load for a few frames
                deathDetectionTime = 30;

            if (player.getRect().Intersects(Projectile.getRect()) && !player.HasBuff(ModContent.BuffType<AbilityCooldown>()))
                transportingToSelectedMarker = true;

            if (transportingToSelectedMarker)
            {
                SoundStyle coinSound = SoundID.CoinPickup;
                coinSound.Pitch = 2f;
                coinSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(coinSound, Projectile.Center);

                Vector2 targetPosition = Main.projectile[player.GetModPlayer<FanPlayer>().lucySelectedMarkerWhoAmI].Center;
                player.Center = Vector2.Lerp(player.Center, targetPosition, 0.32f);
                if (Vector2.Distance(player.Center, targetPosition) <= 2 * 16f)
                {
                    transportingToSelectedMarker = false;
                    player.AddBuff(ModContent.BuffType<AbilityCooldown>(), player.GetModPlayer<MyPlayer>().AbilityCooldownTime(5));
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 7)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 9)
                    Projectile.frame = 0;
            }

            Vector3 lightLevel = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16).ToVector3();
            if (lightLevel.Length() <= 1.3f && deathDetectionTime <= 0)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White * (0.8f + ((float)Math.Abs(Math.Sin(MathHelper.ToRadians(sinTimer))) * 0.2f));
            return Projectile.owner == Main.myPlayer;
        }

        private Texture2D lightBridgeTexture;
        private Rectangle lightBridgeSourceRect;
        private Vector2 lightBridgeOrigin;
        private const int lightBridgeFrameHeight = 6;

        public override bool PreDrawExtras()
        {
            if (lightBridgeTexture == null)
            {
                lightBridgeTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/LucyInTheSky/LightBridge", AssetRequestMode.ImmediateLoad).Value;
                lightBridgeSourceRect = new Rectangle(0, 0, lightBridgeTexture.Width, lightBridgeFrameHeight);
                lightBridgeOrigin = new Vector2(lightBridgeTexture.Width, lightBridgeFrameHeight) / 2f;
            }

            if (Projectile.owner == Main.myPlayer && Projectile.ai[0] > 0f)
            {
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile otherProj = Main.projectile[p];
                    if (otherProj.active && otherProj.owner == Projectile.owner && otherProj.type == Projectile.type && otherProj.ai[0] < Projectile.ai[0])
                    {
                        DrawLineTo(otherProj.Center, Projectile.Center);
                    }
                }
            }
            return Projectile.owner == Main.myPlayer;
        }

        private void DrawLineTo(Vector2 to, Vector2 from)
        {
            float rotation = (to - from).ToRotation();
            for (float k = 0; k <= 1; k += 1 / (Vector2.Distance(from, to) / lightBridgeTexture.Width))     //basically, getting the amount of space between the 2 points, dividing it by the textures width, then making it a fraction, so saying you 'each takes 1/x space, make x of them to fill it up to 1'
            {
                Vector2 pos = Vector2.Lerp(from, to, k) - Main.screenPosition;       //getting the distance and making points by 'k', then bringing it into view
                Color drawColor = Color.White * (0.2f + ((float)Math.Abs(Math.Sin(MathHelper.ToRadians(sinTimer) + Math.Cos(k * 4f) / 2f)) * 0.25f));
                lightBridgeSourceRect.Y += 1 * lightBridgeFrameHeight;
                if (lightBridgeSourceRect.Y / lightBridgeFrameHeight >= 14)
                    lightBridgeSourceRect.Y = 0;

                Main.EntitySpriteDraw(lightBridgeTexture, pos, lightBridgeSourceRect, drawColor, rotation, lightBridgeOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
        }
    }
}