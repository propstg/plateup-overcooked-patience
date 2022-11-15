using HarmonyLib;
using Kitchen;
using KitchenLib.Utils;
using Unity.Entities;

namespace OvercookedPatience {

    [HarmonyPatch(typeof(HandleLifeLoseEvent), "OnUpdate")]
    class HandleLifeLoseEvent_Patch {

        public static bool Prefix(HandleLifeLoseEvent __instance) {
            int loseLifeEvents = __instance.GetQuery(new QueryHelper().All((ComponentType)typeof(CLoseLifeEvent))).CalculateEntityCount();

            if (loseLifeEvents > 0) {
                Mod.Log("Intercepting lose life event...");

                bool loseAllCoins = isLoseAllCoinsSelected();
                bool lose10Coins = isLose10CoinsSelected();
                bool lose5Coins = isLose5CoinsSelected();

                if (areNoOptionsSelected(loseAllCoins, lose10Coins, lose5Coins)) {
                    Mod.Log("No settings are on. Passing control to handler.");
                    return true;
                }

                SMoney money = __instance.GetSingleton<SMoney>();
                Mod.Log("Money available: " + money.Amount);

                int moneyToLose = getMoneyToLose(loseAllCoins, lose10Coins, lose5Coins, money);
                SMoney newMoney = money - moneyToLose;

                if (money <= 0 || newMoney < 0) {
                    Mod.Log("Not enough money. Passing control to handler.");
                } else {
                    Mod.Log("Buying a life.");
                    playSound(__instance.EntityManager);

                    __instance.SetSingleton<SMoney>(newMoney);

                    SKitchenStatus status = __instance.GetSingleton<SKitchenStatus>();
                    status.RemainingLives += 1;
                    __instance.SetSingleton<SKitchenStatus>(status);

                    MoneyPopup.CreateMoneyPopup(__instance.EntityManager, __instance, -moneyToLose);
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

            Mod.Log(logMessage);
            return moneyToLose;
        }

        private static void playSound(EntityManager entityManager) {
            CSoundEvent.Create(entityManager, KitchenData.SoundEvent.MessCreated);
            CSoundEvent.Create(entityManager, KitchenData.SoundEvent.ItemDelivered);
        }
    }
}
