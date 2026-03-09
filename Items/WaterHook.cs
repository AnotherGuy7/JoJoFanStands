// WaterHook.cs
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items
{
    public class WaterHook : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water Hook");
        }

        public override void SetDefaults()
        {
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<WaterHookProjectile>();
            Item.width = 28;
            Item.height = 28;
            Item.value = 0;
            Item.rare = ItemRarityID.LightPurple;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool? UseItem(Player player) => true;
    }
}