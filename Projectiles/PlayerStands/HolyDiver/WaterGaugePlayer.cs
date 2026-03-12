using Terraria;
using Terraria.ModLoader;

namespace JoJoFanStands.Projectiles.PlayerStands.HolyDiver
{
    /// <summary>
    /// Holds Holy Diver's Water Gauge value and handles passive regeneration.
    ///
    /// Costs (adjust freely):
    ///   Scorching Torrent Barrage  —  2 per hit  (called from stand AI)
    ///   Water Cannon (beam shot)   — 10 per tick
    ///   Water Missile salvo        — 20 per salvo
    ///   Mine placement             —  5
    ///   Hydro Symbiosis drain      —  1 per frame (handled in stand AI)
    ///   Holy Water heal            —  8 per use
    ///   Water Absorption           — restores 25 per use
    /// </summary>
    public class WaterGaugePlayer : ModPlayer
    {
        // -------------------------------------------------------
        // Tuning
        // -------------------------------------------------------
        public static int MaxWater = 100;

        /// <summary>Passive ticks between +1 water drops when no source is nearby.</summary>
        private const int PassiveRegenInterval = 30;

        public int CurrentWater { get; private set; } = MaxWater;
        private int _regenTimer = 0;


        /// <summary>Returns true and spends cost if enough water is available.</summary>
        public bool TrySpend(int cost)
        {
            if (CurrentWater < cost) return false;
            CurrentWater -= cost;
            if (CurrentWater < 0) CurrentWater = 0;
            return true;
        }

        /// <summary>Spend without checking (clamps to 0).</summary>
        public void Spend(int cost)
        {
            CurrentWater -= cost;
            if (CurrentWater < 0) CurrentWater = 0;
        }

        /// <summary>Restore water (e.g. Water Absorption ability).</summary>
        public void Restore(int amount)
        {
            CurrentWater += amount;
            if (CurrentWater > MaxWater) CurrentWater = MaxWater;
        }

        /// <summary>Fill to max instantly (e.g. debug / item).</summary>
        public void FillFull() => CurrentWater = MaxWater;

        public bool IsFull => CurrentWater >= MaxWater;
        public bool IsEmpty => CurrentWater <= 0;

        // -------------------------------------------------------
        // ResetEffects — called every frame before buffs apply
        // -------------------------------------------------------
        public override void ResetEffects() { /* nothing to reset */ }

        // -------------------------------------------------------
        // PostUpdate — passive regen tick
        // -------------------------------------------------------
        public override void PostUpdate()
        {
            if (IsFull) { _regenTimer = 0; return; }

            _regenTimer++;
            if (_regenTimer >= PassiveRegenInterval)
            {
                CurrentWater++;
                _regenTimer = 0;
            }
        }

        // -------------------------------------------------------
        // Saving / loading
        // -------------------------------------------------------
        public override void SaveData(Terraria.ModLoader.IO.TagCompound tag)
        {
            tag["currentWater"] = CurrentWater;
        }

        public override void LoadData(Terraria.ModLoader.IO.TagCompound tag)
        {
            CurrentWater = tag.ContainsKey("currentWater")
                ? tag.GetInt("currentWater")
                : MaxWater;
        }

        public void SetWater(int value) => CurrentWater = System.Math.Clamp(value, 0, MaxWater);
    }
}