using Kitchen;
using UnityEngine;

namespace KitchenOvercookedPatience {

    class StrikeSystem : StartOfDaySystem {

        private static int strikes = 0;

        protected override void OnUpdate() {
            SDay day = GetSingleton<SDay>();

            if (!OvercookedPatienceSettings.getResetStrikesEachDay() && day.Day != 1) {
                Debug.Log($"[{Mod.MOD_ID}] New day, but reset strikes each day is disabled.");
            } else {
                Debug.Log($"[{Mod.MOD_ID}] New day; clearing strikes.");
                strikes = 0;
            }
        }

        public static int getStrikes() {
            return strikes;
        }

        public static void addStrike() {
            strikes++;
        }
    }
}
