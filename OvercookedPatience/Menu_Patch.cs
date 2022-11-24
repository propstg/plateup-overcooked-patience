using Kitchen;
using HarmonyLib;
using System.Reflection;
using BepInEx.Logging;
using Kitchen.Modules;

namespace OvercookedPatience {

    [HarmonyPatch(typeof(MainMenu), "Setup")]
    class MainMenu_Patch {

        private static ManualLogSource log = Logger.CreateLogSource("OvercookedPatience MainMenu_Patch");

        public static bool Prefix(MainMenu __instance) {
            log.LogInfo("In main menu patch");
            MethodInfo addSubmenu = __instance.GetType().GetMethod("AddSubmenuButton", BindingFlags.NonPublic | BindingFlags.Instance);
            addSubmenu.Invoke(__instance, new object[] { Mod.MOD_NAME, typeof(OvercookedPatienceMenu), false });
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerPauseView), "SetupMenus")]
    class PauseMenu_Patch {

        private static ManualLogSource log = Logger.CreateLogSource("OvercookedPatience PauseMenu_Patch");

        public static bool Prefix(PlayerPauseView __instance) {
            log.LogInfo("In pause menu patch");
            ModuleList moduleList = (ModuleList)__instance.GetType().GetField("ModuleList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            MethodInfo addMenu = __instance.GetType().GetMethod("AddMenu", BindingFlags.NonPublic | BindingFlags.Instance);
            addMenu.Invoke(__instance, new object[] { typeof(OvercookedPatienceMenu), new OvercookedPatienceMenu(__instance.ButtonContainer, moduleList) });
            return true;
        }
    }
}
