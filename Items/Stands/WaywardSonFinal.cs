using JoJoStands;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class WaywardSonFinal : FanStandItemClass
    {
        public override int StandSpeed => 13;
        public override int StandType => 1;
        public override string StandIdentifierName => "WaywardSon";
        public override int StandTier => 4;
        public override Color StandTierDisplayColor => Color.RosyBrown;
        public override bool FanStandItem => true;
        public override string Texture => Mod.Name + "/Items/Stands/WaywardSonT1";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slaves Of Fear (Tier 1)");
            // Tooltip.SetDefault("Left-click to punch enemies at a really fast rate!\nUser Name: The Phantom One \nReference: SLAVES OF FEAR by HEALTH");
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.width = 38;
            Item.height = 48;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            player.GetModPlayer<FanPlayer>().SpawnFanStand();
            return true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (!Main.mouseItem.IsAir)
                return;

            int highestDamage = 98;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i] != null && player.inventory[i].type != Item.type)
                {
                    if (player.inventory[i].damage > highestDamage)
                        highestDamage = player.inventory[i].damage;
                }
            }
            damage.Base = (int)(highestDamage * player.GetModPlayer<MyPlayer>().standDamageBoosts);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<WaywardSonT3>())
                .AddIngredient(ItemID.BottomlessShimmerBucket)
                .AddTile(ModContent.TileType<RemixTableTile>())
                .Register();
        }
    }
}
