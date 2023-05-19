using JoJoFanStands.Buffs;
using JoJoFanStands.Items.Stands;
using JoJoFanStands.Mounts;
using JoJoStands;
using JoJoStands.Items.Hamon;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands
{
    public class FanPlayer : ModPlayer
    {
        public static bool NaturalPaoSpawns = false;

        public const int MaxBlur = 100;

        private int standKeyPressTimer = 0;

        public bool brianEnoAct1 = false;
        public bool brianEnoAct2 = false;
        public bool brianEnoAct3 = false;
        public bool anyBrianEno = false;
        public bool spinBoost = false;
        public bool roseColoredSunActive = false;
        public bool hidingInLucyMarker = false;
        public bool litsIntoTheLightAbilityActive = false;
        public bool blurLightningFastReflexes = false;
        public bool blurInfiniteVelocity = false;

        public int banksDefenseReduction = 0;
        public int lucySelectedMarkerWhoAmI;
        public int amountOfBlurEnergy = 0;
        public int blurStage = 0;

        public override void ResetEffects()
        {
            spinBoost = false;
            roseColoredSunActive = false;
            blurLightningFastReflexes = false;
            blurInfiniteVelocity = false;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            HamonPlayer hPlayer = player.GetModPlayer<HamonPlayer>();

            if (JoJoStands.JoJoStands.StandOutHotKey.JustPressed && mPlayer.standOut && standKeyPressTimer <= 0)
            {
                standKeyPressTimer += 30;
                mPlayer.immuneToTimestopEffects = false;
                if (anyBrianEno)
                {
                    brianEnoAct1 = false;
                    brianEnoAct2 = false;
                    brianEnoAct3 = false;
                    anyBrianEno = false;
                    player.ClearBuff(BuffType<BrianEnoActiveBuff>());
                }
            }
            if (JoJoStands.JoJoStands.SpecialHotKey.JustPressed)
            {
                if (player.mount.Type != MountID.None)
                {
                    spinBoost = true;
                }
                else
                {
                    if (brianEnoAct1 && !player.wet)
                        player.mount.SetMount(MountType<BrianEnoMount>(), player);
                    if (brianEnoAct2)
                        if (player.wet)
                            player.mount.SetMount(MountType<BathysphereMount>(), player);
                        else
                            player.mount.SetMount(MountType<BrianEnoMount>(), player);
                    if (brianEnoAct3)
                        if (player.wet)
                            player.mount.SetMount(MountType<BathysphereMount>(), player);
                        else
                            player.mount.SetMount(MountType<SkySawMount>(), player);
                }
            }
            if (anyBrianEno && JoJoStands.JoJoStands.SpecialHotKey.JustPressed && !player.wet && hPlayer.amountOfHamon > 0)
            {
                hPlayer.amountOfHamon -= 1;
                spinBoost = true;
            }
        }

        public override void PreUpdate()
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            mPlayer.hideAllPlayerLayers = mPlayer.hideAllPlayerLayers || hidingInLucyMarker;
            anyBrianEno = brianEnoAct1 || brianEnoAct2 || brianEnoAct3;
            if (standKeyPressTimer > 0)
                standKeyPressTimer--;

            if (brianEnoAct1)
            {
                if (player.mount.Type != MountID.None && !player.wet)
                {
                    player.moveSpeed += 0.1f;
                    player.GetCritChance(DamageClass.Melee) += 10;
                }
                if (spinBoost)
                {
                    player.moveSpeed += 0.2f;
                }
            }
            if (brianEnoAct2)
            {
                if (player.mount.Type != MountID.None)
                {
                    player.moveSpeed += 0.1f;
                    player.GetCritChance(DamageClass.Melee) += 10;
                }
                if (spinBoost)
                {
                    player.moveSpeed += 0.3f;
                }
            }
            if (brianEnoAct3)
            {
                if (player.mount.Type != MountID.None)
                {
                    player.moveSpeed += 0.1f;
                    player.GetCritChance(DamageClass.Melee) += 10;
                }
                if (spinBoost)
                {
                    player.moveSpeed += 0.4f;
                }
            }

            if (!Main.dedServ && Player.whoAmI == Main.myPlayer)
            {
                //if (blurLightningFastReflexes)
                //Filters.Scene.Activate(JoJoFanStandsShaders.CircularGreyscale);
                JoJoStandsShaders.ChangeShaderActiveState(JoJoFanStandsShaders.CircularGreyscale, blurLightningFastReflexes || blurInfiniteVelocity);
                if (blurInfiniteVelocity)
                    JoJoStandsShaders.ChangeShaderUseProgress(JoJoFanStandsShaders.CircularGreyscale, 2f);
                else
                    JoJoStandsShaders.ChangeShaderUseProgress(JoJoFanStandsShaders.CircularGreyscale, 1f);
            }
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (Player.HasBuff<Vibration>())
                return true;

            return false;
        }

        public void SpawnFanStand()
        {
            MyPlayer mPlayer = Player.GetModPlayer<MyPlayer>();
            Item inputItem = mPlayer.StandSlot.SlotItem;

            FanStandItemClass standItem = inputItem.ModItem as FanStandItemClass;
            string standClassName = standItem.StandProjectileName + "StandT" + standItem.StandTier;
            if (standClassName.Contains("T4"))
                standClassName = standItem.StandProjectileName + "StandFinal";

            int standProjectileType = Mod.Find<ModProjectile>(standClassName).Type;
            Projectile.NewProjectile(inputItem.GetSource_FromThis(), Player.position, Player.velocity, standProjectileType, 0, 0f, Player.whoAmI);
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (anyBrianEno && Player.mount.Type != MountID.None && !Player.wet)
            {
                if (brianEnoAct1 && (Main.rand.NextFloat(0, 101) <= 5f || spinBoost))
                {
                    modifiers.FinalDamage *= 0;
                    Player.velocity.X -= 5f * Player.direction;
                }
            }
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (Player.mount.Type == MountType<BrianEnoMount>() || Player.mount.Type == MountType<SkySawMount>())
            {
                PlayerDrawLayers.Skin.Hide();
                PlayerDrawLayers.Leggings.Hide();
                PlayerDrawLayers.Shoes.Hide();
            }
            if (Player.mount.Type == MountType<BathysphereMount>())
            {
                PlayerDrawLayers.HeldItem.Hide();
                PlayerDrawLayers.Shield.Hide();
                PlayerDrawLayers.Skin.Hide();
                PlayerDrawLayers.Torso.Hide();
                PlayerDrawLayers.Wings.Hide();
                PlayerDrawLayers.BalloonAcc.Hide();
                PlayerDrawLayers.Leggings.Hide();
                PlayerDrawLayers.Shoes.Hide();
            }
        }
    }
}