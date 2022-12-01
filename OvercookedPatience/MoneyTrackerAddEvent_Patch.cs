using BepInEx.Logging;
using HarmonyLib;
using Kitchen;

namespace OvercookedPatience {

    [HarmonyPatch(typeof(MoneyTracker), "AddEvent")]
    class MoneyTrackGetRecord {

        private static ManualLogSource log = Logger.CreateLogSource($"{Mod.MOD_NAME} MoneyTracker");

        public static bool Prefix(int identifier, int amount) {
            log.LogInfo($"In money tracker add event. Adding {amount} for identifier {identifier}");
            return true;
        }
    }
}
