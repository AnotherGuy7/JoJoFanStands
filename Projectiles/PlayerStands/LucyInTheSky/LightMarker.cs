using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Buffs.ItemBuff;
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
        private int rightClickTimer = 0;
        private int abilityHurtCooldown = 0;
        private int rightClickCooldown = 0;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();

            sinTimer++;
            if (sinTimer >= 360)
                sinTimer = 0;
            Projectile.timeLeft = 2;
            if (deathDetectionTime > 0)
                deathDetectionTime--;
            if (abilityHurtCooldown > 0)
                abilityHurtCooldown--;
            if (rightClickCooldown > 0)
                rightClickCooldown--;

            if (Projectile.Distance(player.Center) > Main.screenHeight)     //Sometimes when loading tiles, the lighting doesn't load for a few frames
                deathDetectionTime = 30;

            if (player.getRect().Intersects(Projectile.getRect()) && !player.HasBuff(ModContent.BuffType<AbilityCooldown>()) && Projectile.whoAmI != fPlayer.lucySelectedMarkerWhoAmI)
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
                    if (mPlayer.standTier <= 3)
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(4 - mPlayer.standTier));
                }
            }

            if (Main.mouseRight && mPlayer.standTier >= 2 && !fPlayer.hidingInLucyMarker && Projectile.Hitbox.Contains(Main.MouseWorld.ToPoint()) && !player.HasBuff<AbilityCooldown>() && Main.myPlayer == Projectile.owner)
            {
                rightClickTimer++;
                if (rightClickTimer >= 60)
                {
                    fPlayer.hidingInLucyMarker = true;
                    player.position = Projectile.position;
                    fPlayer.lucySelectedMarkerWhoAmI = Projectile.whoAmI;
                    rightClickCooldown += 90;
                    rightClickTimer = 0;
                }
            }
            else
                rightClickTimer = 0;

            if (fPlayer.hidingInLucyMarker && Projectile.whoAmI == fPlayer.lucySelectedMarkerWhoAmI)
            {
                player.controlUseItem = false;
                player.dashType = 0;
                player.bodyVelocity = Vector2.Zero;
                player.controlLeft = false;
                player.controlJump = false;
                player.controlRight = false;
                player.controlDown = false;
                player.controlQuickHeal = false;
                player.controlQuickMana = false;
                player.controlRight = false;
                player.controlUseTile = false;
                player.controlUp = false;
                player.maxRunSpeed = 0;
                player.moveSpeed = 0;
                player.mount._frameCounter = 2;
                player.Center = Projectile.Center;
                mPlayer.hideAllPlayerLayers = true;

                if (Main.mouseRight && rightClickCooldown <= 0 && Main.myPlayer == Projectile.owner)
                {
                    fPlayer.hidingInLucyMarker = false;
                    player.AddBuff(ModContent.BuffType<SurpriseAttack>(), 5 * 60);
                    player.AddBuff(ModContent.BuffType<AbilityCooldown>(), 5 * 60);
                    fPlayer.lucySelectedMarkerWhoAmI = Projectile.whoAmI;
                }
            }

            if (fPlayer.litsIntoTheLightAbilityActive && abilityHurtCooldown <= 0)
            {
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile marker = Main.projectile[p];
                    if (marker.active && marker.type == ModContent.ProjectileType<LightMarker>() && marker.owner == player.whoAmI && Projectile.ai[0] > marker.ai[0])
                    {
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.active && npc.lifeMax > 5 && !npc.townNPC && !npc.friendly && Collision.CheckAABBvLineCollision(npc.position, npc.Hitbox.Size(), Projectile.Center, marker.Center))
                            {
                                abilityHurtCooldown += 5;
                                npc.StrikeNPC(46 + ((mPlayer.standTier - 3) * 17), 4f + ((mPlayer.standTier - 3) * 2f), -npc.direction, Main.rand.Next(1, 100 + 1) <= mPlayer.standCritChangeBoosts ? true : false);
                            }
                        }
                    }
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

            if (Projectile.owner == Main.myPlayer)
            {
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile otherProj = Main.projectile[p];
                    if (otherProj.active && otherProj.owner == Projectile.owner && otherProj.type == Projectile.type)
                    {
                        Vector2 direction = otherProj.Center - Projectile.Center;
                        float originalLength = direction.Length();
                        direction.Normalize();
                        direction *= originalLength * 0.5f;
                        DrawLineTo(Projectile.Center + direction, Projectile.Center);
                    }
                }
            }
            return Projectile.owner == Main.myPlayer;
        }

        private void DrawLineTo(Vector2 to, Vector2 from)
        {
            float rotation = (to - from).ToRotation();
            float limit = 1f;
            float increment = 1 / (Vector2.Distance(from, to) / lightBridgeTexture.Width);
            for (float k = 0; k <= limit; k += increment)     //Checking to see if there is enough space.
            {
                if (k > limit - increment)
                {
                    limit += increment;
                    break;
                }
            }

            for (float k = 0; k <= limit; k += increment)     //basically, getting the amount of space between the 2 points, dividing it by the textures width, then making it a fraction, so saying you 'each takes 1/x space, make x of them to fill it up to 1'
            {
                Vector2 pos = Vector2.Lerp(from, to, k) - Main.screenPosition;       //getting the distance and making points by 'k', then bringing it into view
                Color drawColor = Color.White * (0.2f + ((float)Math.Abs(Math.Sin(MathHelper.ToRadians(sinTimer) + Math.Cos(k * 4f) / 2f)) * 0.25f));
                if (Main.player[Projectile.owner].GetModPlayer<FanPlayer>().litsIntoTheLightAbilityActive)
                {
                    drawColor *= 2f;
                    if (Main.rand.Next(0, 2 + 1) == 0)
                    {
                        int dustIndex = Dust.NewDust(pos, 6, 6, DustID.IceTorch);
                        Main.dust[dustIndex].noGravity = true;
                    }
                }
                lightBridgeSourceRect.Y += 1 * lightBridgeFrameHeight;
                if (lightBridgeSourceRect.Y / lightBridgeFrameHeight >= 14)
                    lightBridgeSourceRect.Y = 0;

                Main.EntitySpriteDraw(lightBridgeTexture, pos, lightBridgeSourceRect, drawColor, rotation, lightBridgeOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
        }
    }
}