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
        public const string MOD_VERSION = "0.6.1";
        public const float MOD_VERSION_AS_FLOAT = 0.6f;
        private static bool isRegistered = false;

        public Mod() : base(MOD_ID, MOD_NAME, "blargle", MOD_VERSION, "1.1.2", Assembly.GetExecutingAssembly()) { }

        protected override void Initialise() {
            base.Initialise();
            if (!isRegistered) {
                Debug.Log($"[{MOD_ID}] v{MOD_VERSION} initialized");
                initPreferences();
                initPauseMenu();
                AddGameDataObject<EndOfDayMoneyTrackerAppliance>();
                isRegistered = true;
            } else {
                Debug.Log($"[{MOD_ID}] v{MOD_VERSION} skipping re-creating menus and preferences");
            }
        }

        private void initPauseMenu() {
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(OvercookedPatienceMenu<PauseMenuAction>), typeof(PauseMenuAction));
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(OvercookedPatienceMenu<PauseMenuAction>), new OvercookedPatienceMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
        }

        private void initPreferences() {
            OvercookedPatienceSettings.registerPreferences(MOD_VERSION_AS_FLOAT);
        }
    }
}
