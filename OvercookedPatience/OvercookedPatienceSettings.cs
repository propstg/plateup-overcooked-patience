using KitchenLib;
using KitchenLib.Utils;

namespace KitchenOvercookedPatience {

    public class OvercookedPatienceSettings {

        public static readonly string LOSE_COINS_KEY = "losecoins";
        public static readonly string USE_COOLDOWN_KEY = "usecooldown";

        public static int getLoseCoinsSelected() {
            return PreferenceUtils.Get<IntPreference>(Mod.MOD_ID, LOSE_COINS_KEY).Value;
        }

        public static bool getUseCooldownOnPatienceLost() {
            return PreferenceUtils.Get<BoolPreference>(Mod.MOD_ID, USE_COOLDOWN_KEY).Value;
        }
    }
}
