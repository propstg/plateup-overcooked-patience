using HarmonyLib;
using Kitchen;

namespace OvercookedPatience {

    [HarmonyPatch(typeof(UpdateQueuePatience), "OnUpdate")]
    class UpdateQueuePatience_Patch {
        public static bool Prefix() {
            return Mod.patienceCooldownRemaining <= 0;
        }
    }

    [HarmonyPatch(typeof(UpdateCustomerImpatience), "OnUpdate")]
    class UpdateCustomerImpatience_Patch {
        public static bool Prefix() {
            return Mod.patienceCooldownRemaining <= 0;
        }
    }
}
