using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;

namespace JoJoFanStands.Buffs
{
	public class SelfCrystallize : ModBuff
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Self-Crystallized");
			Description.SetDefault("All of your internal organs is turned into a light crystals\n" + "You gain immense mobility boost(doesn't works in a mount) and immune to fall damage\n" + "However, your physical capabilities are weakened");
			Main.persistentBuff[base.Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.NurseCannotRemoveDebuff[base.Type] = true;
		}	
		public override void Update(Player player, ref int buffIndex) 
		{
			player.moveSpeed += 1f;
			player.jumpSpeedBoost += 5f;
			player.maxRunSpeed += 2f;
			player.noFallDmg = true;
			player.statDefense -= 25;
			player.endurance -= 0.3f;
			player.GetDamage(DamageClass.Generic) -= 0.2f;
		}
	}
}
