using JoJoFanStands.Items.Armor;
using JoJoFanStands.Items.Stands;
using JoJoFanStands.Projectiles.PlayerStands.Blur;
using JoJoFanStands.Projectiles.PlayerStands.TheWorldOverHeaven;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.BombTellyDir;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.GlueManDir;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.GreenDevilDir;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.PowerMusclerDir;
using JoJoFanStands.Projectiles.PlayerStands.VirtualInsanity.YellowDevilDir;
using JoJoFanStands.Projectiles.PlayerStands.WaywardSon;
using JoJoStands.UI;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands
{
    public class JoJoFanStands : Mod
    {
        public static Mod JoJoStandsMod;
        public static Mod JoJoStandsSoundsMod;
        public static bool SoundsLoaded;
        public static JoJoFanStands Instance;

        public override void Load()
        {
            Instance = ModContent.GetInstance<JoJoFanStands>();
            JoJoStandsMod = ModLoader.GetMod("JoJoStands");
            SoundsLoaded = ModLoader.TryGetMod("JoJoStandsSounds", out JoJoStandsSoundsMod);


            BlurStandT1.punchTextures = new Texture2D[2];
            BlurStandT1.punchTextures[0] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Blur/Blur_Punch_1", AssetRequestMode.ImmediateLoad).Value;
            BlurStandT1.punchTextures[1] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/Blur/Blur_Punch_2", AssetRequestMode.ImmediateLoad).Value;

            VirtualInsanityStandFinal.AttackStyleTextures = new Texture2D[3];
            VirtualInsanityStandFinal.AttackStyleTextures[0] = ModContent.Request<Texture2D>("JoJoFanStands/Extras/FistKanji", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.AttackStyleTextures[1] = ModContent.Request<Texture2D>("JoJoFanStands/Extras/SwordKanji", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.AttackStyleTextures[2] = ModContent.Request<Texture2D>("JoJoFanStands/Extras/GunKanji", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.PowerInstallKanji = ModContent.Request<Texture2D>("JoJoFanStands/Extras/PowerInstallKanji", AssetRequestMode.ImmediateLoad).Value;

            VirtualInsanityStandFinal.PortalTextures = new Texture2D[3];
            VirtualInsanityStandFinal.PortalTextures[0] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Portal/PortalOpen", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.PortalTextures[1] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Portal/PortalIdle", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.PortalTextures[2] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Portal/PortalClose", AssetRequestMode.ImmediateLoad).Value;

            VirtualInsanityStandFinal.ArmCannonSpritesheets = new Texture2D[3];
            VirtualInsanityStandFinal.ArmCannonSpritesheets[0] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Cannon_Arm", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.ArmCannonSpritesheets[1] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Cannon_ArmShoot", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.ArmCannonSpritesheets[2] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Cannon_ArmFlash", AssetRequestMode.ImmediateLoad).Value;

            VirtualInsanityStandFinal.CannonHeadSpritesheets = new Texture2D[3];
            VirtualInsanityStandFinal.CannonHeadSpritesheets[0] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Cannon_HeadUp", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.CannonHeadSpritesheets[1] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Cannon_HeadStraight", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.CannonHeadSpritesheets[2] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Cannon_HeadDown", AssetRequestMode.ImmediateLoad).Value;

            VirtualInsanityStandFinal.CannonHeadFlashSpritesheets = new Texture2D[3];
            VirtualInsanityStandFinal.CannonHeadFlashSpritesheets[0] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Cannon_HeadUpFlash", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.CannonHeadFlashSpritesheets[1] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Cannon_HeadStraightFlash", AssetRequestMode.ImmediateLoad).Value;
            VirtualInsanityStandFinal.CannonHeadFlashSpritesheets[2] = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/Cannon_HeadDownFlash", AssetRequestMode.ImmediateLoad).Value;

            YellowDevil.splitUpTexture = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/YellowDevilDir/YellowDevil_SplitUp", AssetRequestMode.ImmediateLoad).Value;

            GreenDevil.impactSpritesheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/GreenDevilDir/GreenDevilImpact", AssetRequestMode.ImmediateLoad).Value;
            GoopProjectile.stuckOnSurfaceSpritesheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/GreenDevilDir/GoopStuckOnSurface", AssetRequestMode.ImmediateLoad).Value;

            BombTelly.tellySpawnSpritesheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/BombTellyDir/BombTellySpawn", AssetRequestMode.ImmediateLoad).Value;
            BombTelly.tellySecondSpritesheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/BombTellyDir/BombTelly_2", AssetRequestMode.ImmediateLoad).Value;

            GlueMan.npcGlueIcon = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/GlueManDir/GlueStuck", AssetRequestMode.ImmediateLoad).Value;
            GlueMan.spawnSheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/GlueManDir/GlueSpawn", AssetRequestMode.ImmediateLoad).Value;
            PowerMuscler.spawnSheet = ModContent.Request<Texture2D>("JoJoFanStands/Projectiles/PlayerStands/VirtualInsanity/PowerMusclerDir/PowerMuscler_Spawn", AssetRequestMode.ImmediateLoad).Value;


            BlurBar.blurBarTexture = ModContent.Request<Texture2D>("JoJoFanStands/UI/BlurEnergyBar", AssetRequestMode.ImmediateLoad).Value;
            SoulBar.soulBarTexture = ModContent.Request<Texture2D>("JoJoFanStands/UI/SoulBar", AssetRequestMode.ImmediateLoad).Value;
            SoulBar.soulBarBarTexture = ModContent.Request<Texture2D>("JoJoFanStands/UI/SoulBar_Bar", AssetRequestMode.ImmediateLoad).Value;

            if (!Main.dedServ)
                JoJoFanStandsShaders.LoadShaders();

            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<CoolOutT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<FollowMeT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<MortalReminderT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<SlavesOfFearT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<TheFatesT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<BanksT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<BrianEnoAct1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<ExpansesT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<BlurT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<WaywardSonT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<MetempsychosisT1>());
            JoJoStands.JoJoStands.standTier1List.Add(ModContent.ItemType<VirtualInsanityT1>());

            JoJoStands.JoJoStands.timestopImmune.Add(ModContent.ProjectileType<TheWorldOverHeavenStandT1>());
            JoJoStands.JoJoStands.timestopImmune.Add(ModContent.ProjectileType<TheWorldOverHeavenStandT2>());
            JoJoStands.JoJoStands.timestopImmune.Add(ModContent.ProjectileType<TheWorldOverHeavenStandT3>());
            JoJoStands.JoJoStands.timestopImmune.Add(ModContent.ProjectileType<TheWorldOverHeavenStandFinal>());
            JoJoStands.JoJoStands.timestopImmune.Add(ModContent.ProjectileType<WaywardSonStandT3>());
            JoJoStands.JoJoStands.timestopImmune.Add(ModContent.ProjectileType<WaywardSonStandFinal>());
        }

        public override void PostSetupContent()
        {
            if (SoundsLoaded)
            {
                VirtualInsanityStandT2.PowerInstallTheme = ModContent.Request<SoundEffect>("JoJoStandsSounds/Sounds/Themes/VirtualInsanity/PowerInstall_2", AssetRequestMode.ImmediateLoad).Value;
                VirtualInsanityStandT3.PowerInstallTheme = ModContent.Request<SoundEffect>("JoJoStandsSounds/Sounds/Themes/VirtualInsanity/PowerInstall_3", AssetRequestMode.ImmediateLoad).Value;
                VirtualInsanityStandFinal.PowerInstallTheme = ModContent.Request<SoundEffect>("JoJoStandsSounds/Sounds/Themes/VirtualInsanity/PowerInstall_4", AssetRequestMode.ImmediateLoad).Value;
            }
        }

        public override void Unload()
        {
            JoJoStandsMod = null;
            if (BlurBar.blurBarTexture != null)
                BlurBar.blurBarTexture = null;
            if (BlurStandT1.punchTextures != null)
                BlurStandT1.punchTextures = null;
            Instance = null;
        }
    }
}