using HarmonyLib;
using Kitchen;

namespace KitchenOvercookedPatience {

    class PatienceCooldownSystem : GameSystemBase {

        public static int patienceCooldownRemaining = 0;

        public static void startPatienceCooldown() {
            if (OvercookedPatienceSettings.getUseCooldownOnPatienceLost()) {
                patienceCooldownRemaining = 200;
            }
        }

        protected override void OnUpdate() {
            if (patienceCooldownRemaining <= 0) {
                patienceCooldownRemaining = 0;
            } else {
                patienceCooldownRemaining--;
            }
        }
    }

    [HarmonyPatch(typeof(UpdateQueuePatience), "OnUpdate")]
    class UpdateQueuePatience_Patch {
        public static bool Prefix() {
            return PatienceCooldownSystem.patienceCooldownRemaining <= 0;
        }
    }

    [HarmonyPatch(typeof(UpdateCustomerImpatience), "OnUpdate")]
    class UpdateCustomerImpatience_Patch {
        public static bool Prefix() {
            return PatienceCooldownSystem.patienceCooldownRemaining <= 0;
        }
    }
}
