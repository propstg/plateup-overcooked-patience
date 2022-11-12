using BepInEx;
using HarmonyLib;
using Kitchen;
using KitchenLib.Event;
using KitchenLib.Utils;
using System;
using System.Reflection;
using Unity.Entities;

namespace OvercookedPatience {

    [BepInProcess("PlateUp.exe")]
    [BepInPlugin(MOD_ID, MOD_NAME, "0.2.0")]
    public partial class Mod : KitchenLib.BaseMod {

        public const string MOD_ID = "overcookedpatience";
        public const string MOD_NAME = "Overcooked Patience";

        public Mod() : base(">=1.1.0", Assembly.GetCallingAssembly()) { }

        public void Start() {
            initHarmony();
            initSettings();
        }

        private void initHarmony() {
            Events.StartMainMenu_SetupEvent += (s, args) => {
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MOD_ID);
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

        [HarmonyPatch(typeof(HandleLifeLoseEvent), "OnUpdate")]
        class HandleLifeLoseEvent_Patch {

            public static bool Prefix(HandleLifeLoseEvent __instance) {
                int loseLifeEvents = __instance.GetQuery(new QueryHelper().All((ComponentType)typeof(CLoseLifeEvent))).CalculateEntityCount();

                if (loseLifeEvents > 0) {
                    Log("Intercepting lose life event...");

                    bool loseAllCoins = isLoseAllCoinsSelected();
                    bool lose10Coins = isLose10CoinsSelected();
                    bool lose5Coins = isLose5CoinsSelected();

                    if (areNoOptionsSelected(loseAllCoins, lose10Coins, lose5Coins)) {
                        Log("No settings are on. Passing control to handler.");
                        return true;
                    }

                    SMoney money = __instance.GetSingleton<SMoney>();
                    Log("Money available: " + money.Amount);

                    SMoney newMoney = money - getMoneyToLose(loseAllCoins, lose10Coins, lose5Coins, money);

                    if (money <= 0 || newMoney < 0) {
                        Log("Not enough money. Passing control to handler.");
                    } else {
                        Log("Buying a life.");

                        __instance.SetSingleton<SMoney>(newMoney);

                        SKitchenStatus status = __instance.GetSingleton<SKitchenStatus>();
                        status.RemainingLives += 1;
                        __instance.SetSingleton<SKitchenStatus>(status);
                    }
                }
                return true;
            }

            private static bool isLoseAllCoinsSelected() {
                return isPreferenceSelected(OvercookedPatienceConsts.LOSE_ALL_COINS);
            }

            private static bool isLose10CoinsSelected() {
                return isPreferenceSelected(OvercookedPatienceConsts.LOSE_10_COINS);
            }

            private static bool isLose5CoinsSelected() {
                return isPreferenceSelected(OvercookedPatienceConsts.LOSE_5_COINS);
            }

            private static bool isPreferenceSelected(string preference) {
                return PreferenceUtils.Get<KitchenLib.BoolPreference>(Mod.MOD_ID, preference).Value;
            }

            private static bool areNoOptionsSelected(bool loseAllCoins, bool lose10Coins, bool lose5Coins) {
                return !(loseAllCoins || lose10Coins || lose5Coins);
            }

            private static SMoney getMoneyToLose(bool loseAllCoins, bool lose10Coins, bool lose5Coins, SMoney currentMoney) {
                SMoney moneyToLose;
                string logMessage;

                if (loseAllCoins) {
                    logMessage = OvercookedPatienceConsts.LOSE_ALL_COINS + " is on. Setting value to lose to current coin total";
                    moneyToLose = currentMoney;
                } else if (lose10Coins) {
                    logMessage = OvercookedPatienceConsts.LOSE_10_COINS + " is on. Setting value to lose to " + OvercookedPatienceConsts.LOSE_10_COINS_VALUE;
                    moneyToLose = OvercookedPatienceConsts.LOSE_10_COINS_VALUE;
                } else {
                    logMessage = OvercookedPatienceConsts.LOSE_5_COINS + " is on. Setting value to lose to " + OvercookedPatienceConsts.LOSE_5_COINS_VALUE;
                    moneyToLose = OvercookedPatienceConsts.LOSE_5_COINS_VALUE;
                }

                Log(logMessage);
                return moneyToLose;
            }
        }
    }
}
