using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands
{
    public class FanPlayer : ModPlayer
    {
        public static bool spawnPao = false;

        public float numberOfSpikesCreated = 0;

        public bool BrianEnoAct1 = false;
        public bool BrianEnoAct2 = false;
        public bool BrianEnoAct3 = false;
        public bool CoolOutActive = false;
        public bool FollowMeActive = false;
        public bool TheFatesActive = false;
        public bool InTheShadowsPet = false;
        public bool SpinBoost = false;
        public bool RoseColoredSunActive = false;
        public bool StandOut = false;
        public bool additionMode = false;
        public bool avalanche = false;
        public bool wearingStandAccessory = false;

        public override void ResetEffects()
        {
            StandOut = false;
            SpinBoost = false;
            CoolOutActive = false;
            FollowMeActive = false;
            TheFatesActive = false;
            InTheShadowsPet = false;
            BrianEnoAct1 = false;
            BrianEnoAct2 = false;
            BrianEnoAct3 = false;
            wearingStandAccessory = false;
            RoseColoredSunActive = false;
        }

        public override void PreUpdate()
        {
            if (numberOfSpikesCreated >= 1f)
            {
                additionMode = false;
            }
            if (numberOfSpikesCreated <= 0f)
            {
                additionMode = true;
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if ((BrianEnoAct1 || BrianEnoAct2 || BrianEnoAct3) && Main.rand.NextFloat(0, 101) <= 5f)
            {
                player.immune = true;
                if (player.direction == 1)
                {
                    player.velocity.X += 5f;
                }
                if (player.direction == -1)
                {
                    player.velocity.X -= 5f;
                }
                damage = 0;
                crit = false;
            }
            base.OnHitByNPC(npc, damage, crit);
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (player.mount.Type == mod.MountType("BrianEnoMount"))
            {
                PlayerLayer.Legs.visible = false;
                PlayerLayer.Skin.visible = false;
            }
            if (player.mount.Type == mod.MountType("SkySawMount"))
            {
                PlayerLayer.Legs.visible = false;
                PlayerLayer.Skin.visible = false;
            }
            if (player.mount.Type == mod.MountType("BathysphereMount"))
            {
                PlayerLayer.Legs.visible = false;
                PlayerLayer.Body.visible = false;
                PlayerLayer.Skin.visible = false;
                PlayerLayer.Arms.visible = false;
                PlayerLayer.HeldItem.visible = false;
                PlayerLayer.ShieldAcc.visible = false;
            }
            /*@AnotherGuy int index = layers.FindIndex(l => l.Name.Equals("MountBack"));
            if (index > -1) //do stuff
                then use layer[index].visible = false; for examlpe*/
        }
    }
}