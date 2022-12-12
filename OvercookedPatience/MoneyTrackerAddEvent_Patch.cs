using HarmonyLib;
using Kitchen;
using UnityEngine;

namespace KitchenOvercookedPatience {

    [HarmonyPatch(typeof(MoneyTracker), "AddEvent")]
    class MoneyTrackGetRecord {

        public static bool Prefix(int identifier, int amount) {
            Debug.Log($"{Mod.MOD_ID}: In money tracker add event. Adding {amount} for identifier {identifier}");
            return true;
        }
    }
}
