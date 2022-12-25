using Kitchen;
using System;
using UnityEngine;

namespace KitchenOvercookedPatience {

    public class OvercookedPatienceSettings {

        public static readonly string MODE_KEY = "mode";
        public static readonly string LOSE_COINS_KEY = "losecoins";
        public static readonly string USE_COOLDOWN_KEY = "usecooldown";
        public static readonly int ALL_COINS = -1;
        public static readonly int PROGRESSIVE = -2;

        private static readonly int DEFAULT_MODE_KEY = Convert.ToInt32(OvercookedPatienceMode.LOSE_COINS);
        private static readonly int DEFAULT_LOSE_COINS_VALUE = ALL_COINS;
        private static readonly bool DEFAULT_USE_COOLDOWN_VALUE = false;

        private static Pref ModePref = new Pref(Mod.MOD_ID, nameof(ModePref));
        private static Pref LoseCoinsPref = new Pref(Mod.MOD_ID, nameof(LoseCoinsPref));
        private static Pref UseCooldownPref = new Pref(Mod.MOD_ID, nameof(UseCooldownPref));

        public static OvercookedPatienceMode getMode() {
            return (OvercookedPatienceMode)Preferences.Get<int>(ModePref);
        }

        public static void setMode(OvercookedPatienceMode mode) {
            Preferences.Set<int>(ModePref, Convert.ToInt32(mode));
        }

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
            Preferences.AddPreference<int>(new IntPreference(ModePref, DEFAULT_MODE_KEY));
            Preferences.AddPreference<int>(new IntPreference(LoseCoinsPref, DEFAULT_LOSE_COINS_VALUE));
            Preferences.AddPreference<bool>(new BoolPreference(UseCooldownPref, DEFAULT_USE_COOLDOWN_VALUE));
            Preferences.Load();
        }
    }

    public enum OvercookedPatienceMode {
        OFF = 0,
        LOSE_COINS = 1,
        STRIKES = 2,
    }
}
