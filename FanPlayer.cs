using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using JoJoStands;
using static Terraria.ModLoader.ModContent;
using JoJoFanStands.Items.Stands;
using JoJoFanStands.Items.Armor;
using JoJoFanStands.Mounts;
using JoJoStands.Items.Hamon;
using JoJoFanStands.Buffs;

namespace JoJoFanStands
{
    public class FanPlayer : ModPlayer
    {
        public static bool spawnPao = false;

        private int activationTimer = 0;

        public bool BrianEnoAct1 = false;
        public bool BrianEnoAct2 = false;
        public bool BrianEnoAct3 = false;
        public bool anyBrianEno = false;
        public bool SpinBoost = false;
        public bool RoseColoredSunActive = false;

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

            if (JoJoStands.JoJoStands.StandOut.JustPressed && !mPlayer.StandOut && activationTimer <= 0)
            {
                SpawnStand();       //this is
                MyPlayer.spawningOtherStands = true;
                activationTimer += 30;
            }
            if (JoJoStands.JoJoStands.StandOut.JustPressed && mPlayer.StandOut && activationTimer <= 0)
            {
                activationTimer += 30;
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
                        player.mount.SetMount(MountType<BrianEnoMount>(), Main.player[Main.myPlayer]);
                    if (BrianEnoAct2)
                        if (player.wet)
                            player.mount.SetMount(MountType<BathysphereMount>(), Main.player[Main.myPlayer]);
                        else
                            player.mount.SetMount(MountType<BrianEnoMount>(), Main.player[Main.myPlayer]);
                    if (BrianEnoAct3)
                        if (player.wet)
                            player.mount.SetMount(MountType<BathysphereMount>(), Main.player[Main.myPlayer]);
                        else
                            player.mount.SetMount(MountType<SkySawMount>(), Main.player[Main.myPlayer]);
                }
            }
            if (anyBrianEno && JoJoStands.JoJoStands.SpecialHotKey.Current && !player.wet && hPlayer.HamonCounter > 0)
            {
                hPlayer.HamonCounter -= 1;
                SpinBoost = true;
            }
        }

        public override void PreUpdate()
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            HamonPlayer hPlayer = player.GetModPlayer<HamonPlayer>();

            anyBrianEno = BrianEnoAct1 || BrianEnoAct2 || BrianEnoAct3;
            if (activationTimer > 0)
            {
                activationTimer--;
            }
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

        public void SpawnStand()
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            int inputItemtype = mPlayer.StandSlot.Item.type;

            if (inputItemtype == ItemType<BrianEnoAct1>())
            {
                BrianEnoAct1 = true;
                player.AddBuff(BuffType<BrianEnoActiveBuff>(), 2);
            }
            if (inputItemtype == ItemType<BrianEnoAct2>())
            {
                BrianEnoAct2 = true;
                player.AddBuff(BuffType<BrianEnoActiveBuff>(), 2);
            }
            if (inputItemtype == ItemType<BrianEnoAct3>())
            {
                BrianEnoAct3 = true;
                player.AddBuff(BuffType<BrianEnoActiveBuff>(), 2);
            }
            else if (inputItemtype == ItemType<BackInBlack>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("BackInBlackStand"), 0, 0f, Main.myPlayer);
            }
            if (inputItemtype == ItemType<CoolOutT1>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("CoolOutStandT1"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<CoolOutT2>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("CoolOutStandT2"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<CoolOutT3>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("CoolOutStandT3"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<CoolOutFinal>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("CoolOutStandFinal"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<FollowMeT1>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("FollowMeStandT1"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<FollowMeT2>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("FollowMeStandT2"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<Megalovania>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("MegalovaniaStand"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<MortalReminderT1>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("MortalReminderStandT1"), 0, 0f, Main.myPlayer);
            }
            /*else if (inputItemtype == ItemType<RoseColoredBoy>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("FollowMeStandT2"), 0, 0f, Main.myPlayer);
            }*/
            else if (inputItemtype == ItemType<SlavesOfFearT1>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("SlavesOfFearStandT1"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<SlavesOfFearT2>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("SlavesOfFearStandT2"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<SlavesOfFearT3>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("SlavesOfFearStandT3"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<SlavesOfFearFinal>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("SlavesOfFearStandFinal"), 0, 0f, Main.myPlayer);
            }
            else if (inputItemtype == ItemType<TheFatesT1>())
            {
                Projectile.NewProjectile(player.position, player.velocity, mod.ProjectileType("TheFatesStand"), 0, 0f, Main.myPlayer);
            }
            else
            {
                MyPlayer.spawningOtherStands = false;
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (anyBrianEno && player.mount.Type != Mount.None && !player.wet)
            {
                if (BrianEnoAct1 && (Main.rand.NextFloat(0, 101) <= 5f || SpinBoost))
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