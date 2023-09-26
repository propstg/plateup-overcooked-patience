using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using System.Reflection;
using UnityEngine;

namespace KitchenOvercookedPatience {

    public class Mod : BaseMod {

        public const string MOD_ID = "blargle.overcookedpatience";
        public const string MOD_NAME = "Overcooked Patience";
        public const string MOD_VERSION = "0.6.4";
        public const float MOD_VERSION_AS_FLOAT = 0.6f;

        public Mod() : base(MOD_ID, MOD_NAME, "blargle", MOD_VERSION, ">=1.1.7", Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise() {
            Debug.Log($"[{MOD_ID}] v{MOD_VERSION} initialized");
            initPreferences();
            initPauseMenu();
            AddGameDataObject<EndOfDayMoneyTrackerAppliance>();
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
