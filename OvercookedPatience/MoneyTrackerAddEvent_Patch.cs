using HarmonyLib;
using Kitchen;

namespace OvercookedPatience {

    [HarmonyPatch(typeof(MoneyTracker), "AddEvent")]
    class MoneyTrackGetRecord {

        public static bool Prefix(int identifier, int amount) {
            Mod.Log("In money tracker add event. Adding " + amount + " for identifier " + identifier);
            return true;
        }
    }
}
