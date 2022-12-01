using BepInEx;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace OvercookedPatience {

    [BepInProcess("PlateUp.exe")]
    [BepInPlugin(MOD_ID, MOD_NAME, "0.4.7")]
    public class Mod : BaseUnityPlugin {

        public const string MOD_ID = "overcookedpatience";
        public const string MOD_NAME = "Overcooked Patience";
        public const string MOD_VERSION = "0.4.7";

        public static int patienceCooldownRemaining = 0;

        public void Awake() {
            Debug.LogWarning($"BepInEx mod loaded: {MOD_NAME} {MOD_VERSION}");
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
