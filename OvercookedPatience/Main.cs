using BepInEx;
using HarmonyLib;
using Kitchen;
using KitchenLib.Event;
using KitchenLib.Utils;
using System;
using System.Reflection;

namespace OvercookedPatience {

    [BepInProcess("PlateUp.exe")]
    [BepInPlugin(MOD_ID, MOD_NAME, "0.4.4")]
    public partial class Mod : KitchenLib.BaseMod {

        public const string MOD_ID = "overcookedpatience";
        public const string MOD_NAME = "Overcooked Patience";

        private bool registered = false;

        public Mod() : base(">=1.1.0", Assembly.GetCallingAssembly()) { }

        public void Start() {
            Mod.Log("Start");

            initHarmony();
            initSettings();
        }

        private void initHarmony() {
            Log("initHarmony()");

            Events.StartMainMenu_SetupEvent += (s, args) => {
                if (registered) {
                    return;
                }

                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MOD_ID);
                Log("StartMainMenu_SetupEvent()");

                registered = true;
            };
        }

        private void initSettings() {
            initPreferences();
            initMainMenu();
            initPauseMenu();
        }

        private void initPreferences() {
            KitchenLib.BoolPreference loseAllMoney = PreferenceUtils
                .Register<KitchenLib.BoolPreference>(MOD_ID, OvercookedPatienceConsts.LOSE_ALL_COINS, OvercookedPatienceConsts.LOSE_ALL_COINS_DISPLAY);
            KitchenLib.BoolPreference lose25Money = PreferenceUtils
                .Register<KitchenLib.BoolPreference>(MOD_ID, OvercookedPatienceConsts.LOSE_5_COINS, OvercookedPatienceConsts.LOSE_5_COINS_DISPLAY);
            KitchenLib.BoolPreference lose50Money = PreferenceUtils
                .Register<KitchenLib.BoolPreference>(MOD_ID, OvercookedPatienceConsts.LOSE_10_COINS, OvercookedPatienceConsts.LOSE_10_COINS_DISPLAY);

            loseAllMoney.Value = true;
            lose25Money.Value = false;
            lose50Money.Value = false;
            PreferenceUtils.Load();
        }

        private void initMainMenu() {
            Events.PreferenceMenu_MainMenu_SetupEvent += (s, args) => {
                Type type = args.instance.GetType().GetGenericArguments()[0];
                args.mInfo.Invoke(args.instance, new object[] { MOD_NAME, typeof(OvercookedPatienceSettings<>).MakeGenericType(type), false });
            };
            Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(OvercookedPatienceSettings<MainMenuAction>), new OvercookedPatienceSettings<MainMenuAction>(args.Container, args.Module_list));
            };
        }

        private void initPauseMenu() {
            Events.PreferenceMenu_PauseMenu_SetupEvent += (s, args) => {
                Type type = args.instance.GetType().GetGenericArguments()[0];
                args.mInfo.Invoke(args.instance, new object[] { MOD_NAME, typeof(OvercookedPatienceSettings<>).MakeGenericType(type), false });
            };
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(OvercookedPatienceSettings<PauseMenuAction>), new OvercookedPatienceSettings<PauseMenuAction>(args.Container, args.Module_list));
            };
        }
    }
}
