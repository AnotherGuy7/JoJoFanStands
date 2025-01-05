using JoJoFanStands.Projectiles.PlayerStands.TheWorldOverHeaven;
using JoJoStands;
using JoJoStands.Items;
using JoJoStands.Items.CraftingMaterials;
using JoJoStands.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace JoJoFanStands.Items.Stands
{
    public class TheWorldOverHeavenFinal : FanStandItemClass
    {
        public override int StandSpeed => 6;
        public override int StandType => 1;
        public override string StandIdentifierName => "TheWorldOverHeaven";
        public override int StandTier => 5;
        public override Color StandTierDisplayColor => Color.WhiteSmoke;
        public override bool FanStandItem => true;
        public override string Texture
        {
            get { return Mod.Name + "/Items/Stands/TheWorldOverHeavenT1"; }
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blur (Tier 1)");
            // Tooltip.SetDefault("Left-click shoot a random enemy.\nUser Name: Pauline \nReference: Blur by Lincoln");
        }

        public override void SetDefaults()
        {
            Item.damage = 342;
            Item.width = 38;
            Item.height = 46;
            Item.maxStack = 1;
            Item.value = 0;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            Projectile.NewProjectile(Item.GetSource_FromThis(), player.position, player.velocity, ProjectileType<TheWorldOverHeavenStandFinal>(), 0, 0f, Main.myPlayer);
            return true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (!Main.mouseItem.IsAir)
                return;

            int highestDamage = 342;
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
            if (!ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                CreateRecipe()
                    .AddIngredient(ItemType<TheWorldFinal>())
                    .AddIngredient(ItemType<TaintedLifeforce>())
                    .AddIngredient(ItemType<RequiemArrow>())
                    .AddTile(ModContent.TileType<RemixTableTile>())
                    .Register();
                return;
            }

            Recipe recipe = CreateRecipe()
                .AddIngredient(ItemType<TheWorldOverHeavenT3>())
                .AddIngredient(ItemType<TaintedLifeforce>())
                .AddTile(ModContent.TileType<RemixTableTile>());

            if (calamityMod.TryFind<ModItem>("AuricBar", out ModItem auricBar))
                recipe.AddIngredient(auricBar.Type, 18);
            recipe.Register();
        }
    }
}