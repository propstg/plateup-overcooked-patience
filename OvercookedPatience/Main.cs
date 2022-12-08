using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Utils;
using System;
using System.Reflection;
using UnityEngine;

namespace KitchenOvercookedPatience {

    public class Mod : BaseMod {

        public const string MOD_ID = "blargle.overcookedpatience";
        public const string MOD_NAME = "Overcooked Patience";
        public const string MOD_VERSION = "0.4.8";

        public Mod() : base(MOD_ID, MOD_NAME, "blarglebottoms", MOD_VERSION, "1.1.2", Assembly.GetExecutingAssembly()) {
            Debug.Log($"Mod loaded: {MOD_ID} {MOD_VERSION}");
            initMainMenu();
            initPauseMenu();
            initPreferences();
        }

        private void initMainMenu() {
            Events.PreferenceMenu_MainMenu_SetupEvent += (s, args) => {
                Type type = args.instance.GetType().GetGenericArguments()[0];
                args.mInfo.Invoke(args.instance, new object[] { MOD_NAME, typeof(OvercookedPatienceMenu<>).MakeGenericType(type), false });
            };
            Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(OvercookedPatienceMenu<MainMenuAction>), new OvercookedPatienceMenu<MainMenuAction>(args.Container, args.Module_list));
            };
        }

        private void initPauseMenu() {
            Events.PreferenceMenu_PauseMenu_SetupEvent += (s, args) => {
                Type type = args.instance.GetType().GetGenericArguments()[0];
                args.mInfo.Invoke(args.instance, new object[] { MOD_NAME, typeof(OvercookedPatienceMenu<>).MakeGenericType(type), false });
            };
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(OvercookedPatienceMenu<PauseMenuAction>), new OvercookedPatienceMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
        }

        private void initPreferences() {
            KitchenLib.IntPreference loseCoins = PreferenceUtils
                .Register<KitchenLib.IntPreference>(MOD_ID, OvercookedPatienceSettings.LOSE_COINS_KEY, "How many coins to lose when customers lose patience");
            KitchenLib.BoolPreference useCooldown = PreferenceUtils
                .Register<KitchenLib.BoolPreference>(MOD_ID, OvercookedPatienceSettings.USE_COOLDOWN_KEY, "Use cooldown after losing patience");

            loseCoins.Value = -1;
            useCooldown.Value = false;
            PreferenceUtils.Load();
        }
    }
}
