using JoJoFanStands.Buffs;
using JoJoFanStands.Items.Stands;
using JoJoFanStands.Mounts;
using JoJoStands;
using JoJoStands.Items;
using JoJoStands.Items.Hamon;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands
{
    public class FanPlayer : ModPlayer
    {
        public static bool spawnPao = false;

        private int standKeyPressTimer = 0;

        public bool BrianEnoAct1 = false;
        public bool BrianEnoAct2 = false;
        public bool BrianEnoAct3 = false;
        public bool anyBrianEno = false;
        public bool SpinBoost = false;
        public bool RoseColoredSunActive = false;

        public int banksDefenseReduction = 0;

        public override void ResetEffects()
        {
            SpinBoost = false;
            RoseColoredSunActive = false;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            HamonPlayer hPlayer = player.GetModPlayer<HamonPlayer>();

            if (JoJoStands.JoJoStands.StandOutHotKey.JustPressed && !mPlayer.standOut && standKeyPressTimer <= 0)
            {
                SpawnFanStand();
                standKeyPressTimer += 30;
            }
            if (JoJoStands.JoJoStands.StandOutHotKey.JustPressed && mPlayer.standOut && standKeyPressTimer <= 0)
            {
                standKeyPressTimer += 30;
                if (anyBrianEno)
                {
                    BrianEnoAct1 = false;
                    BrianEnoAct2 = false;
                    BrianEnoAct3 = false;
                    anyBrianEno = false;
                    player.ClearBuff(BuffType<BrianEnoActiveBuff>());
                }
            }
            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed)
            {
                if (player.mount.Type != Mount.None)
                {
                    SpinBoost = true;
                }
                else
                {
                    if (BrianEnoAct1 && !player.wet)
                        player.mount.SetMount(MountType<BrianEnoMount>(), player);
                    if (BrianEnoAct2)
                        if (player.wet)
                            player.mount.SetMount(MountType<BathysphereMount>(), player);
                        else
                            player.mount.SetMount(MountType<BrianEnoMount>(), player);
                    if (BrianEnoAct3)
                        if (player.wet)
                            player.mount.SetMount(MountType<BathysphereMount>(), player);
                        else
                            player.mount.SetMount(MountType<SkySawMount>(), player);
                }
            }
            if (anyBrianEno && JoJoStands.JoJoStands.SpecialHotKey.Current && !player.wet && hPlayer.amountOfHamon > 0)
            {
                hPlayer.amountOfHamon -= 1;
                SpinBoost = true;
            }
        }

        public override void PreUpdate()
        {
            Player player = Main.player[Main.myPlayer];

            anyBrianEno = BrianEnoAct1 || BrianEnoAct2 || BrianEnoAct3;
            if (standKeyPressTimer > 0)
                standKeyPressTimer--;

            if (BrianEnoAct1)
            {
                if (player.mount.Type != Mount.None && !player.wet)
                {
                    player.moveSpeed += 0.1f;
                    player.meleeCrit += 10;
                }
                if (SpinBoost)
                {
                    player.moveSpeed += 0.2f;
                }
            }
            if (BrianEnoAct2)
            {
                if (player.mount.Type != Mount.None)
                {
                    player.moveSpeed += 0.1f;
                    player.meleeCrit += 10;
                }
                if (SpinBoost)
                {
                    player.moveSpeed += 0.3f;
                }
            }
            if (BrianEnoAct3)
            {
                if (player.mount.Type != Mount.None)
                {
                    player.moveSpeed += 0.1f;
                    player.meleeCrit += 10;
                }
                if (SpinBoost)
                {
                    player.moveSpeed += 0.4f;
                }
            }
        }

        public void SpawnFanStand()
        {
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            Item inputItem = mPlayer.StandSlot.Item;

            if (inputItem.IsAir)
            {
                Main.NewText("There is no stand in the Stand Slot!", Color.Red);
                mPlayer.standOut = false;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    JoJoStands.Networking.ModNetHandler.playerSync.SendStandOut(256, player.whoAmI, false, player.whoAmI);
                }
                return;
            }

            if (!(inputItem.modItem is FanStandItemClass))
            {
                mPlayer.standOut = false;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    JoJoStands.Networking.ModNetHandler.playerSync.SendStandOut(256, player.whoAmI, false, player.whoAmI);
                }
                return;
            }

            FanStandItemClass standItem = inputItem.modItem as FanStandItemClass;

            mPlayer.standOut = true;
            mPlayer.standDefenseToAdd = 4 + (2 * standItem.standTier);
            if (standItem.standType == 2)
                mPlayer.standDefenseToAdd /= 2;

            string standClassName = standItem.standProjectileName + "StandT" + standItem.standTier;
            if (standClassName.Contains("T4"))
                standClassName = standItem.standProjectileName + "StandFinal";

            if (player.ownedProjectileCounts[mod.ProjectileType(standClassName)] > 0)
            {
                mPlayer.standOut = false;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    JoJoStands.Networking.ModNetHandler.playerSync.SendStandOut(256, player.whoAmI, false, player.whoAmI);
                }
                return;
            }

            if (!standItem.ManualStandSpawning(player))
            {
                int standProjectileType = mod.ProjectileType(standClassName);

                Projectile.NewProjectile(player.position, player.velocity, standProjectileType, 0, 0f, Main.myPlayer);
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (anyBrianEno && player.mount.Type != Mount.None && !player.wet)
            {
                if (BrianEnoAct1 && (Main.rand.NextFloat(0, 101) <= 5f || SpinBoost))
                {
                    damage = 0;
                    player.velocity.X -= 5f * player.direction;
                }
            }
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (player.mount.Type == MountType<BrianEnoMount>())
            {
                PlayerLayer.Legs.visible = false;
                PlayerLayer.Skin.visible = false;
                PlayerLayer.ShoeAcc.visible = false;
            }
            if (player.mount.Type == MountType<BathysphereMount>())
            {
                PlayerLayer.Legs.visible = false;
                PlayerLayer.Body.visible = false;
                PlayerLayer.Skin.visible = false;
                PlayerLayer.Arms.visible = false;
                PlayerLayer.HeldItem.visible = false;
                PlayerLayer.ShieldAcc.visible = false;
                PlayerLayer.ShoeAcc.visible = false;
                PlayerLayer.BalloonAcc.visible = false;
                PlayerLayer.Wings.visible = false;
            }
            if (player.mount.Type == MountType<SkySawMount>())
            {
                PlayerLayer.Legs.visible = false;
                PlayerLayer.Skin.visible = false;
                PlayerLayer.ShoeAcc.visible = false;
            }
        }
    }
}