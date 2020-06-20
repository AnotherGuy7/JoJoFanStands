using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace JoJoFanStands.Items.Armor
{
    public class BrianEnoAct3 : ModItem
    {
        public int abilityDuration = 0;

        public void SetStaticDefault()
        {
            DisplayName.SetDefault("Brian Eno (Act 3, Mother Whale Eyeless)");
            Tooltip.SetDefault("Use the special ability key to make yourself immune to damage, but unable to move or use items.");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.accessory = true;
            item.rare = 6;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FanPlayer>().wearingStandAccessory = true;
            player.GetModPlayer<FanPlayer>().BrianEnoAct3 = true;
            abilityDuration--;
            if (abilityDuration <= 0)
            {
                abilityDuration = 0;
            }
            if (abilityDuration > 0)
            {
                player.meleeCrit += 20;
                player.immune = true;
            }
            if (player.mount.Type != Mount.None)
            {
                player.moveSpeed += 0.1f;
                player.meleeCrit += 10;
            }
            if (JoJoFanStands.AccessorySpecialKey.JustPressed && /*JoJoStands.MyPlayer.HamonCounter >= 60 &&*/ player.mount.Type != Mount.None)
            {
                //JoJoStands.MyPlayer.HamonCounter -= 60;
                abilityDuration = 90;
            }
            if (JoJoFanStands.AccessorySpecialKey.JustPressed && player.mount.Type == Mount.None && !player.wet)
            {
                player.mount.SetMount(mod.MountType("SkySawMount"), Main.player[Main.myPlayer]);
            }
            if (JoJoFanStands.AccessorySpecialKey.JustPressed && player.mount.Type == Mount.None && player.wet)
            {
                player.mount.SetMount(mod.MountType("BathysphereMount"), Main.player[Main.myPlayer]);
            }
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            if (player.GetModPlayer<FanPlayer>().wearingStandAccessory)
            {
                return false;
            }
            return true;
        }

        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType("StandArrow"));
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
    }
}