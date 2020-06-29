using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using JoJoStands;
using Terraria.ID;

namespace JoJoFanStands
{
    public class FanPlayer : ModPlayer
    {
        public static bool spawnPao = false;

        private int activationTimer = 0;

        public bool BrianEnoAct1 = false;
        public bool BrianEnoAct2 = false;
        public bool BrianEnoAct3 = false;
        public bool InTheShadowsPet = false;
        public bool SpinBoost = false;
        public bool RoseColoredSunActive = false;
        public bool wearingStandAccessory = false;

        public override void ResetEffects()
        {
            SpinBoost = false;
            InTheShadowsPet = false;
            BrianEnoAct1 = false;
            BrianEnoAct2 = false;
            BrianEnoAct3 = false;
            wearingStandAccessory = false;
            RoseColoredSunActive = false;
        }

        public override void PreUpdate()
        {
            if (activationTimer > 0)
            {
                activationTimer--;
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            if (JoJoStands.JoJoStands.StandOut.JustPressed && !mPlayer.StandOut && activationTimer <= 0)
            {
                SpawnStand();       //this is
                activationTimer += 30;
            }
        }

        public void SpawnStand()
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            int inputItemtype = mPlayer.StandSlot.Item.type;

            if (inputItemtype == mod.ItemType("CoolOutT1"))
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("CoolOutStandT1"), 0, 0f, Main.myPlayer);
                MyPlayer.spawningOtherStands = true;
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
                then use layer[index].visible = false; for example*/
        }
    }
}