using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoFanStands.Items
{
    public class WaterKatanaT2 : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4f;
            Item.value = 0;
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = true;
            Item.noUseGraphic = false;
            Item.noMelee = false;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water Katana");
            // Tooltip.SetDefault("A katana imbued with the power of flowing water.");
        }
    }
}