using JoJoFanStands.Buffs;
using JoJoFanStands.Projectiles.PlayerStands.Metempsychosis;
using JoJoStands;
using JoJoStands.Buffs.Debuffs;
using JoJoStands.Projectiles.PlayerStands;
using JoJoStands.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static JoJoFanStands.Projectiles.PlayerStands.Metempsychosis.MetempsychosisStandFinal;

namespace JoJoFanStands.Projectiles.PlayerStands.MetempsychosisRequiem
{
    public class MetempsychosisRequiemStand : StandClass
    {
        public override int HalfStandHeight => 43;
        public override int PunchDamage => 131;
        public override int AltDamage => 162;
        public override int PunchTime => 8;
        public override int TierNumber => 5;
        public override bool CanUseAfterImagePunches => false;
        public override Vector2 StandOffset => new Vector2(-12f, 0f);
        public override StandAttackType StandType => StandAttackType.Melee;
        public new AnimationState currentAnimationState;
        public new AnimationState oldAnimationState;

        private int robeGlowmaskTimer = 0;
        private int weaponGlowmaskTimer = 0;
        private bool claimingSouls = false;
        private bool soulsClaimed = false;
        private bool usingRend = false;
        private Vector2 rendMousePosition;
        private bool playedDeathLine = false;

        public new enum AnimationState
        {
            Idle,
            Attack,
            SecondaryAbility,
            Special1,
            Special2,
            Pose
        }

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            shootCount--;
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            FanPlayer fPlayer = player.GetModPlayer<FanPlayer>();
            if (mPlayer.standOut)
                Projectile.timeLeft = 2;
            if (player.dead && !playedDeathLine)
            {
                playedDeathLine = true;
                SoundEngine.PlaySound(Main.rand.NextBool() ? Death_1 : Death_2, Projectile.Center);
            }
            if (!player.dead && playedDeathLine)
            {
                playedDeathLine = false;
                SoundEngine.PlaySound(Main.rand.NextBool() ? Revival_1 : Revival_2, Projectile.Center);
            }

            if (robeGlowmaskTimer < 360)
            {
                robeGlowmaskTimer += 1;
                if (robeGlowmaskTimer >= 360)
                    robeGlowmaskTimer -= 360;
            }

            if (weaponGlowmaskTimer < 360)
            {
                weaponGlowmaskTimer += 2;
                if (weaponGlowmaskTimer >= 360)
                    weaponGlowmaskTimer -= 360;
            }

            if (player.HasBuff<GirdSoul>())
            {
                robeGlowmaskTimer = 270;
                weaponGlowmaskTimer = 270;
            }

            Projectile.tileCollide = !usingRend && !secondaryAbility;
            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (Main.mouseLeft)
                    {
                        Punch(ModContent.ProjectileType<FanStandFists>(), Main.MouseWorld, afterImages: false);
                        currentAnimationState = AnimationState.Attack;
                    }
                    else
                    {
                        attacking = false;
                        currentAnimationState = AnimationState.Idle;
                        if (!secondaryAbility && !usingRend && !claimingSouls)
                            StayBehind();
                    }
                    if (Main.mouseRight && !playerHasAbilityCooldown && !secondaryAbility && !attacking && !claimingSouls && !usingRend)
                    {
                        secondaryAbility = true;
                        currentAnimationState = AnimationState.SecondaryAbility;
                        SoundEngine.PlaySound(Main.rand.NextBool() ? Claim_1 : Claim_2, Projectile.Center);
                    }
                    if (SpecialKeyPressed() && fPlayer.metempsychosisPoints >= 25 && !attacking && !secondaryAbility && !claimingSouls && !usingRend)
                    {
                        claimingSouls = true;
                        soulsClaimed = false;
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                        Projectile.netUpdate = true;
                        fPlayer.metempsychosisPoints -= 25;
                        SoundEngine.PlaySound(SoundID.Item8.WithPitchOffset(-0.8f), Projectile.Center);
                        SoundEngine.PlaySound(Main.rand.NextBool() ? Claim_1 : Claim_2, Projectile.Center);
                    }
                    if (SecondSpecialKeyPressed() && fPlayer.metempsychosisPoints >= 10 && !claimingSouls && !usingRend)
                    {
                        usingRend = true;
                        rendMousePosition = Main.MouseWorld;
                        Projectile.netUpdate = true;
                        fPlayer.metempsychosisPoints -= 10;
                        SoundEngine.PlaySound(Main.rand.NextBool() ? Rend_1 : Rend_2, Projectile.Center);
                    }
                }

                if (claimingSouls)      //Special 1
                {
                    GoInFront();
                    currentAnimationState = AnimationState.Special1;
                    float lightStrength = 2.5f;
                    Lighting.AddLight(Projectile.Center, 63 * lightStrength / 255f, 205 * lightStrength / 255f, 189 * lightStrength / 255f);
                    if (Projectile.frame == 2 && !soulsClaimed)
                    {
                        soulsClaimed = true;
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(30 / TierNumber));
                        if (Main.myPlayer == Projectile.owner)
                        {
                            for (int n = 0; n < Main.maxNPCs; n++)
                            {
                                NPC npc = Main.npc[n];
                                if (npc.CanBeChasedBy(this) && Projectile.Distance(npc.Center) <= newMaxDistance * 4)
                                {
                                    int newHealth = (int)(npc.life * (1f - (0.05f * TierNumber))) - npc.defense;
                                    if (npc.boss || newHealth > 0.25f * npc.lifeMax)
                                    {
                                        int damage = npc.life - newHealth;
                                        NPC.HitInfo hitInfo = new NPC.HitInfo()
                                        {
                                            Damage = damage,
                                            Knockback = 0f,
                                            HitDirection = -Projectile.direction,
                                        };
                                        npc.StrikeNPC(hitInfo);
                                        NetMessage.SendStrikeNPC(npc, hitInfo, Projectile.owner);
                                    }
                                    else
                                    {
                                        npc.StrikeInstantKill();
                                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<SoulEffect>(), 0, 0f, Projectile.owner, player.statLifeMax2 * 0.05f);
                                    }
                                }
                            }
                        }
                    }
                }

                if (usingRend)      //Special 2
                {
                    currentAnimationState = AnimationState.Special2;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 rendVelocity = rendMousePosition - Projectile.Center;
                        rendVelocity.Normalize();
                        rendVelocity *= 12f;
                        Projectile.velocity = rendVelocity;
                        Projectile.spriteDirection = Projectile.direction = rendVelocity.X > 0f ? 1 : -1;
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.CanBeChasedBy(this) && Projectile.Distance(npc.Center) <= HalfStandHeight * 2)
                            {
                                int newHealth = (int)(npc.life * 0.5f);
                                if (newHealth / npc.lifeMax > 0.35f)
                                {
                                    int damage = npc.life - newHealth + npc.defense;
                                    NPC.HitInfo hitInfo = new NPC.HitInfo()
                                    {
                                        Damage = damage,
                                        Knockback = 0f,
                                        HitDirection = -Projectile.direction,
                                    };
                                    npc.StrikeNPC(hitInfo);
                                    NetMessage.SendStrikeNPC(npc, hitInfo, Projectile.owner);
                                }
                                else
                                {
                                    npc.StrikeInstantKill();
                                    player.Heal(npc.lifeMax);
                                }
                                usingRend = false;
                                player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(30 / TierNumber));
                                break;
                            }
                        }
                    }

                    if (Vector2.Distance(rendMousePosition, Projectile.Center) < 8f)
                    {
                        usingRend = false;
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(5 / TierNumber));
                    }
                }

                Vector2 direction = player.Center - Projectile.Center;
                float distanceTo = direction.Length();
                if (secondaryAbility)
                {
                    currentAnimationState = AnimationState.SecondaryAbility;
                    Projectile.velocity.X = 12f * Projectile.direction;
                    Projectile.position.Y = player.position.Y + HalfStandHeight / 2;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        for (int n = 0; n < Main.maxNPCs; n++)
                        {
                            NPC npc = Main.npc[n];
                            if (npc.CanBeChasedBy(this) && Projectile.Distance(npc.Center) <= 3f * 16f)
                            {
                                if (Projectile.owner == Main.myPlayer)
                                {
                                    NPC.HitInfo hitInfo = new NPC.HitInfo()
                                    {
                                        Damage = (int)(newAltDamage * (1.05f * TierNumber)),
                                        Knockback = 6f,
                                        HitDirection = Projectile.direction
                                    };
                                    npc.StrikeNPC(hitInfo);
                                    NetMessage.SendStrikeNPC(npc, hitInfo, Projectile.owner);
                                }
                            }
                        }
                    }
                    if (distanceTo > newMaxDistance * 2.5f)
                    {
                        secondaryAbility = false;
                        player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(15 / TierNumber));
                    }
                }

                if (!secondaryAbility && !usingRend)
                    LimitDistance();

                if (mPlayer.posing)
                    currentAnimationState = AnimationState.Pose;
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                BasicPunchAI();
                if (attacking)
                    currentAnimationState = AnimationState.Attack;
                else
                    currentAnimationState = AnimationState.Idle;

                if (SpecialKeyPressed() && fPlayer.metempsychosisPoints >= 10 && !player.HasBuff<GirdSoul>())
                {
                    robeGlowmaskTimer = 180;
                    weaponGlowmaskTimer = 180;
                    player.AddBuff(ModContent.BuffType<GirdSoul>(), 10 * 60);
                    SoundEngine.PlaySound(Main.rand.NextBool() ? GirdSoul_1 : GirdSoul_2, Projectile.Center);
                }
            }
        }

        public override void ExtraSpawnEffects()
        {
            SoulBar.ShowSoulBar();
            SoundEngine.PlaySound(Main.rand.NextBool() ? Summon_1 : Summon_2, Projectile.Center);
        }

        public override void StandKillEffects()
        {
            SoulBar.HideSoulBar();
        }

        public override void SendExtraStates(BinaryWriter writer)
        {
            writer.Write(claimingSouls);
            writer.Write(usingRend);
        }

        public override void ReceiveExtraStates(BinaryReader reader)
        {
            claimingSouls = reader.ReadBoolean();
            usingRend = reader.ReadBoolean();
        }

        public override bool PreDraw(ref Color drawColor)
        {
            string animationName = string.Empty;
            if (currentAnimationState == AnimationState.Idle)
                animationName = "Idle";
            else if (currentAnimationState == AnimationState.Attack)
                animationName = "Attack";
            else if (currentAnimationState == AnimationState.SecondaryAbility)
                animationName = "Reave";
            else if (currentAnimationState == AnimationState.Special1)
                animationName = "Claim";
            else if (currentAnimationState == AnimationState.Special2)
                animationName = "Rend";
            else if (currentAnimationState == AnimationState.Pose)
                animationName = "Pose";

            Main.spriteBatch.End();

            GameShaders.Misc["AuraShader"].UseImage1(ModContent.Request<Texture2D>("JoJoFanStands/Extras/Noise", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            GameShaders.Misc["AuraShader"].UseImage2(ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Metempsychosis/Metempsychosis_" + animationName));
            GameShaders.Misc["AuraShader"].UseSecondaryColor(new Color(63, 205, 189));
            GameShaders.Misc["AuraShader"].UseShaderSpecificData(new Vector4((Projectile.frame * (HalfStandHeight * 2)) / (HalfStandHeight * 2 * amountOfFrames), ((Projectile.frame + 1) * (HalfStandHeight * 2)) / (HalfStandHeight * 2 * amountOfFrames), 0f, 0f));
            Vector2 drawOffset2 = StandOffset;
            drawOffset2.X *= Projectile.spriteDirection;
            Vector2 drawPosition2 = Projectile.Center - Main.screenPosition + drawOffset2 + new Vector2(0f, -26f);
            Vector2 standOrigin2 = new Vector2(standTexture.Width / 2f, (standTexture.Height / amountOfFrames) / 2f);
            DrawData data = new DrawData(ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Metempsychosis/Metempsychosis_AuraBubble", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, drawPosition2, null, drawColor, Projectile.rotation, standOrigin2, 1f, effects, 0);
            GameShaders.Misc["AuraShader"].Apply(data);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, GameShaders.Misc["AuraShader"].Shader, Main.GameViewMatrix.ZoomMatrix);        //starting a draw with dyes that work
            Main.EntitySpriteDraw(data);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);        //starting a draw with dyes that work

            if (UseProjectileAlpha)
                drawColor *= Projectile.alpha / 255f;

            effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;

            if (standTexture != null && Main.netMode != NetmodeID.Server)
            {
                int frameHeight = standTexture.Height / amountOfFrames;
                Vector2 drawOffset = StandOffset;
                drawOffset.X *= Projectile.spriteDirection;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                Rectangle animRect = new Rectangle(0, frameHeight * Projectile.frame, standTexture.Width, frameHeight);
                Vector2 standOrigin = new Vector2(standTexture.Width / 2f, frameHeight / 2f);
                Main.EntitySpriteDraw(standTexture, drawPosition, animRect, drawColor, Projectile.rotation, standOrigin, 1f, effects, 0);
            }
            return true;
        }

        public override void PostDrawExtras()
        {
            string animationName = string.Empty;
            if (currentAnimationState == AnimationState.Idle)
                animationName = "Idle";
            else if (currentAnimationState == AnimationState.Attack)
                animationName = "Attack";
            else if (currentAnimationState == AnimationState.SecondaryAbility)
                animationName = "Reave";
            else if (currentAnimationState == AnimationState.Special1)
                animationName = "Claim";
            else if (currentAnimationState == AnimationState.Special2)
                animationName = "Rend";
            else if (currentAnimationState == AnimationState.Pose)
                animationName = "Pose";

            Texture2D robeGlowmask = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Metempsychosis/Metempsychosis_" + animationName + "_Glowmask").Value;
            Texture2D weaponGlowmask = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Metempsychosis/Metempsychosis_" + animationName + "_Weapon").Value;
            float robeAlpha = (0.4f * (float)Math.Abs(Math.Sin(Math.Tau * (robeGlowmaskTimer / 360f)))) + 0.2f;
            float weaponAlpha = (0.9f * (float)Math.Abs(Math.Sin(Math.Tau * (weaponGlowmaskTimer / 360f)))) + 0.5f;

            effects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;

            if (standTexture != null && Main.netMode != NetmodeID.Server)
            {
                int frameHeight = standTexture.Height / amountOfFrames;
                Vector2 drawOffset = StandOffset;
                drawOffset.X *= Projectile.spriteDirection;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;
                Rectangle animRect = new Rectangle(0, frameHeight * Projectile.frame, standTexture.Width, frameHeight);
                Vector2 standOrigin = new Vector2(standTexture.Width / 2f, frameHeight / 2f);
                Main.EntitySpriteDraw(robeGlowmask, drawPosition, animRect, Color.White * robeAlpha, Projectile.rotation, standOrigin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(weaponGlowmask, drawPosition, animRect, Color.White * weaponAlpha, Projectile.rotation, standOrigin, Projectile.scale, effects, 0);
            }
        }

        public override void AnimationCompleted(string animationName)
        {
            if (animationName == "Claim")
            {
                claimingSouls = false;
                soulsClaimed = false;
                currentAnimationState = AnimationState.Idle;
                Main.player[Projectile.owner].AddBuff(ModContent.BuffType<AbilityCooldown>(), Main.player[Projectile.owner].GetModPlayer<MyPlayer>().AbilityCooldownTime(30 / TierNumber));
            }
        }

        public override void SelectAnimation()
        {
            if (oldAnimationState != currentAnimationState)
            {
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
                oldAnimationState = currentAnimationState;
                Projectile.netUpdate = true;
            }


            if (currentAnimationState == AnimationState.Idle)
                PlayAnimation("Idle");
            else if (currentAnimationState == AnimationState.Attack)
                PlayAnimation("Attack");
            else if (currentAnimationState == AnimationState.SecondaryAbility)
                PlayAnimation("Reave");
            else if (currentAnimationState == AnimationState.Special1)
                PlayAnimation("Claim");
            else if (currentAnimationState == AnimationState.Special2)
                PlayAnimation("Rend");
            else if (currentAnimationState == AnimationState.Pose)
                PlayAnimation("Pose");
        }

        public override void PlayAnimation(string animationName)
        {
            standTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Metempsychosis/Metempsychosis_" + animationName).Value;
            if (animationName == "Idle")
                AnimateStand(animationName, 4, 15, true);
            else if (animationName == "Attack")
                AnimateStand(animationName, 4, newPunchTime, true);
            else if (animationName == "Reave")
                AnimateStand(animationName, 2, 15, true);
            else if (animationName == "Claim")
                AnimateStand(animationName, 4, 15, false);
            else if (animationName == "Rend")
                AnimateStand(animationName, 2, 15, true);
            else if (animationName == "Pose")
                AnimateStand(animationName, 1, 500, true);
        }
    }
}