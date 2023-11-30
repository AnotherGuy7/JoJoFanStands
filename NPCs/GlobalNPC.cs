using JoJoFanStands.Buffs;
using JoJoStands.Buffs.EffectBuff;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.NPCs
{
    public class FanGlobalNPC : GlobalNPC
    {
        public static List<int> nonExistantTypes = new List<int>(5);

        public bool welded = false;
        public int weldTimer = 0;
        public int weldMaxTimer = 0;
        public bool nonExistant = false;
        public bool affectedByAvalance = false;
        public int icicleTimer = 0;
        public float banksCoinMultiplier = 1f;
        public int frameCounterDelayTimer = 0;

        public override bool InstancePerEntity
        {
            get { return true; }
        }

        public override bool PreAI(NPC npc)
        {
            FanPlayer fPlayer = Main.LocalPlayer.GetModPlayer<FanPlayer>();
            /*if (FPlayer.avalanche)
            {
                affectedByAvalance = true;
            }*/
            if (welded)
            {
                npc.velocity = Vector2.Zero;
                weldTimer++;
                if (weldTimer >= weldMaxTimer)
                {
                    weldTimer = 0;
                    weldMaxTimer = 0;
                    welded = false;
                }
                return false;
            }
            if (fPlayer.blurLightningFastReflexes)
            {
                npc.velocity *= 0.1f;
                frameCounterDelayTimer++;
                if (frameCounterDelayTimer >= 2)
                {
                    npc.frameCounter--;
                    frameCounterDelayTimer = 0;
                }    
            }
            if (fPlayer.blurInfiniteVelocity)
            {
                npc.velocity = Vector2.Zero;
                npc.frameCounter = 1;
                if (!npc.noGravity)
                    npc.velocity.Y -= 0.3f;
                npc.netUpdate = true;
                return false;
            }
            /*if (affectedByAvalance)
            {
                icicleTimer++;
                npc.velocity = Vector2.Zero;
                npc.color = Color.LightBlue;
                if (icicleTimer == 170)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.position.X, npc.position.Y - Main.screenHeight, 0f, 5f, ModContent.ProjectileType<Icicle"), 23, 1f, Main.myPlayer);
                }
                if (icicleTimer == 175)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.position.X, npc.position.Y - Main.screenHeight, 0f, 5f, ModContent.ProjectileType<Icicle"), 23, 1f, Main.myPlayer);
                }
                if (icicleTimer == 180)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.position.X, npc.position.Y - Main.screenHeight, 0f, 5f, ModContent.ProjectileType<Icicle"), 23, 1f, Main.myPlayer);
                    FPlayer.avalanche = false;
                }
                if (icicleTimer >= 200)
                {
                    affectedByAvalance = false;
                }
                return false;
            }*/
            return true;
        }

        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);
        }

        public override void OnKill(NPC npc)
        {
            if (banksCoinMultiplier > 1f)
                Item.NewItem(npc.GetSource_Loot(), npc.getRect(), ItemID.CopperCoin, (int)(npc.value * (banksCoinMultiplier - 1f)));
        }

        public override bool CheckActive(NPC npc)
        {
            for (int n = 0; n < nonExistantTypes.Count; n++)
            {
                if (npc.type == nonExistantTypes[n])
                    npc.active = false;
            }
            return true;
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (welded)
                drawColor = Color.DarkGray;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<RealityRewriteBuff>())
                modifiers.FinalDamage *= 2f;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<RealityRewriteBuff>())
                modifiers.FinalDamage *= 2f;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (npc.HasBuff<RealityRewriteBuff>())
                return false;
            return true;
        }
    }
}