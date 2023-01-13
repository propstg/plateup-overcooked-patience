using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenOvercookedPatience {

    public struct SOvercookedStrikes : IComponentData {
        public int Strikes;
    }

    class StrikeSystem : StartOfDaySystem, IModSystem {

        protected override void OnUpdate() {
            SDay day = GetSingleton<SDay>();

            if (!OvercookedPatienceSettings.getResetStrikesEachDay() && day.Day != 1) {
                Debug.Log($"[{Mod.MOD_ID}] New day, but reset strikes each day is disabled.");
            } else {
                Debug.Log($"[{Mod.MOD_ID}] New day; clearing strikes.");
                SOvercookedStrikes strikes =  GetOrDefault<SOvercookedStrikes>();
                strikes.Strikes = 0;
                Set(strikes);
            }
        }
    }
}
