using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace OvercookedPatience {

    [BepInProcess("PlateUp.exe")]
    [BepInPlugin(MOD_ID, MOD_NAME, "0.4.6")]
    public class Mod : BaseUnityPlugin {

        public const string MOD_ID = "overcookedpatience";
        public const string MOD_NAME = "Overcooked Patience";

        public static int patienceCooldownRemaining = 0;

        public void Awake() {
            Logger.LogInfo("Start");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MOD_ID);
        }

        public void Update() {
            if (patienceCooldownRemaining <= 0) {
                patienceCooldownRemaining = 0;
            } else {
                patienceCooldownRemaining--;
            }
        }

        public static void startPatienceCooldown() {
            if (OvercookedPatienceSettings.useCooldownOnPatienceLost) {
                patienceCooldownRemaining = 200;
            }
        }
    }
}
