using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace OvercookedPatience {

    [BepInProcess("PlateUp.exe")]
    [BepInPlugin(MOD_ID, MOD_NAME, "0.4.5")]
    public class Mod : BaseUnityPlugin {

        public const string MOD_ID = "overcookedpatience";
        public const string MOD_NAME = "Overcooked Patience";

        public void Awake() {
            Logger.LogInfo("Start");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MOD_ID);
        }
    }
}
