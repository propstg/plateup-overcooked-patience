using Kitchen;
using UnityEngine;

namespace KitchenOvercookedPatience {

    public class OvercookedPatienceSettings {

        public static readonly string LOSE_COINS_KEY = "losecoins";
        public static readonly string USE_COOLDOWN_KEY = "usecooldown";

        private static readonly int DEFAULT_LOSE_COINS_VALUE = -1;
        private static readonly bool DEFAULT_USE_COOLDOWN_VALUE = false;

        private static Pref LoseCoinsPref = new Pref("test", nameof(LoseCoinsPref));
        private static Pref UseCooldownPref = new Pref("test", nameof(UseCooldownPref));


        public static int getLoseCoinsSelected() {
            return Preferences.Get<int>(LoseCoinsPref);
        }

        public static void setLoseCoinsSelected(int value) {
            Preferences.Set<int>(LoseCoinsPref, value);
        }

        public static bool getUseCooldownOnPatienceLost() {
            return Preferences.Get<bool>(UseCooldownPref);
        }

        public static void setUseCooldownOnPatienceLost(bool value) {
            Preferences.Set<bool>(UseCooldownPref, value);
        }

        public static void registerPreferences() {
            Preferences.AddPreference<int>(new IntPreference(LoseCoinsPref, DEFAULT_LOSE_COINS_VALUE));
            Preferences.AddPreference<bool>(new BoolPreference(UseCooldownPref, DEFAULT_USE_COOLDOWN_VALUE));
        }
    }
}
