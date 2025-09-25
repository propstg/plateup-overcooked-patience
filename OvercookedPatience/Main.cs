using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using System.Reflection;
using UnityEngine;

namespace KitchenOvercookedPatience {

    public class Mod : BaseMod {

        public const string MOD_ID = "blargle.overcookedpatience";
        public const string MOD_NAME = "Overcooked Patience";
        public const string MOD_VERSION = "0.6.7";
        public const float MOD_VERSION_AS_FLOAT = 0.6f;

        public Mod() : base(MOD_ID, MOD_NAME, "blargle", MOD_VERSION, ">=1.2.0", Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise() {
            Debug.Log($"[{MOD_ID}] v{MOD_VERSION} initialized");
            initPreferences();
            initPauseMenu();
            AddGameDataObject<EndOfDayMoneyTrackerAppliance>();
        }

        private void initPauseMenu() {
            ModsPreferencesMenu<MenuAction>.RegisterMenu(MOD_NAME, typeof(OvercookedPatienceMenu<MenuAction>), typeof(MenuAction));
            Events.PlayerPauseView_SetupMenusEvent += (s, args) => {
                args.addMenu.Invoke(args.instance, new object[] { typeof(OvercookedPatienceMenu<MenuAction>), new OvercookedPatienceMenu<MenuAction>(args.instance.ButtonContainer, args.module_list) } );
            };
        }

        private void initPreferences() {
            OvercookedPatienceSettings.registerPreferences(MOD_VERSION_AS_FLOAT);
        }
    }
}
