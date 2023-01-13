using Kitchen;
using System;
using UnityEngine;

namespace KitchenOvercookedPatience {

    public enum OvercookedPatienceMode {
        OFF = 0,
        LOSE_COINS_FIXED = 1,
        STRIKES = 2,
        NO_PATIENCE_PENALTY = 3,
        LOSE_COINS_PROGRESSIVE = 4,
        LOSE_COINS_EXPONENTIAL = 5,
        LOSE_COINS_ALL = 6,
    }

    public class OvercookedPatienceSettings {

        public static readonly int ALL_COINS_OLD = -1;
        public static readonly int PROGRESSIVE_OLD = -2;
        public static readonly int EXPONENTIAL_OLD = -3;
        public static readonly int MAX_STRIKES = 3;
        public static readonly int ACTIVATE_ON_DAY = 4;

        private static readonly int DEFAULT_MODE_KEY = Convert.ToInt32(OvercookedPatienceMode.LOSE_COINS_FIXED);
        private static readonly int DEFAULT_LOSE_COINS_VALUE = 5;
        private static readonly bool DEFAULT_USE_COOLDOWN_VALUE = false;
        private static readonly bool DEFAULT_RESET_STRIKES_EACH_DAY_VALUE = true;

        private static Pref ModePref = new Pref(Mod.MOD_ID, nameof(ModePref));
        private static Pref LoseCoinsPref = new Pref(Mod.MOD_ID, nameof(LoseCoinsPref));
        private static Pref UseCooldownPref = new Pref(Mod.MOD_ID, nameof(UseCooldownPref));
        private static Pref ResetStrikesEachDayPref = new Pref(Mod.MOD_ID, nameof(ResetStrikesEachDayPref));
        private static Pref ModVersionPref = new Pref(Mod.MOD_ID, nameof(ModVersionPref));

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

        public static bool getResetStrikesEachDay() {
            return Preferences.Get<bool>(ResetStrikesEachDayPref);
        }

        public static void setResetStrikesEachDay(bool value) {
            Preferences.Set<bool>(ResetStrikesEachDayPref, value);
        }

        public static void registerPreferences(float newVersion) {
            Preferences.AddPreference<int>(new IntPreference(ModePref, DEFAULT_MODE_KEY));
            Preferences.AddPreference<int>(new IntPreference(LoseCoinsPref, DEFAULT_LOSE_COINS_VALUE));
            Preferences.AddPreference<bool>(new BoolPreference(UseCooldownPref, DEFAULT_USE_COOLDOWN_VALUE));
            Preferences.AddPreference<bool>(new BoolPreference(ResetStrikesEachDayPref, DEFAULT_RESET_STRIKES_EACH_DAY_VALUE));
            Preferences.AddPreference<float>(new FloatPreference(ModVersionPref, 0f));
            Preferences.Load();
            transitionPreferencesTo0_6_0IfNeeded(newVersion);
        }

        private static void transitionPreferencesTo0_6_0IfNeeded(float newVersion) {
            float previousVersion = Preferences.Get<float>(ModVersionPref);
            if (previousVersion < 0.6f) {
                log($"Settings need updated to {newVersion} from {previousVersion}");
                if (getMode() == OvercookedPatienceMode.LOSE_COINS_FIXED) {
                    log($"Lose Coins is selected. Will need to potentially migrate mode.");
                    if (getLoseCoinsSelected() == ALL_COINS_OLD) {
                        log($"All coins selected. Setting new mode to 'LOSE_COINS_ALL', with coins set to 5.");
                        setMode(OvercookedPatienceMode.LOSE_COINS_ALL);
                        setLoseCoinsSelected(5);
                    } else if (getLoseCoinsSelected() == PROGRESSIVE_OLD) {
                        log($"Progressive selected. Setting new mode to 'LOSE_COINS_PROGRESSIVE', with coins set to 5.");
                        setMode(OvercookedPatienceMode.LOSE_COINS_PROGRESSIVE);
                        setLoseCoinsSelected(5);
                    } else if (getLoseCoinsSelected() == EXPONENTIAL_OLD) {
                        log($"Progressive selected. Setting new mode to 'LOSE_COINS_EXPONENTIAL', with coins set to 5.");
                        setMode(OvercookedPatienceMode.LOSE_COINS_EXPONENTIAL);
                        setLoseCoinsSelected(5);
                    } else if (getLoseCoinsSelected() == 0) {
                        log($"0 Coins were selected. Setting new mode to 'NO_PATIENCE_PENALTY', with coins set to 5.");
                        setMode(OvercookedPatienceMode.NO_PATIENCE_PENALTY);
                        setLoseCoinsSelected(5);
                    }
                }
                log("Saving new version to preferences.");
                Preferences.Set<float>(ModVersionPref, newVersion);
            }
        }

        private static void log(object message) {
            Debug.Log($"[{Mod.MOD_ID}] {message}");
        }
    }
}
