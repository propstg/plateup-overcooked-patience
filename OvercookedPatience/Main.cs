using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using System;
using System.Reflection;
using UnityEngine;

namespace KitchenOvercookedPatience {

    public class Mod : BaseMod {

        public const string MOD_ID = "blargle.overcookedpatience";
        public const string MOD_NAME = "Overcooked Patience";
        public const string MOD_VERSION = "0.4.8";

        public static int patienceCooldownRemaining = 0;

        public Mod() : base(MOD_NAME, MOD_VERSION, "1.1.2", Assembly.GetExecutingAssembly()) {
            Debug.Log($"Mod loaded: {MOD_ID} {MOD_VERSION}");
            initMainMenu();
            initPauseMenu();
        }

        protected override void OnUpdate() {
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
    }
}
