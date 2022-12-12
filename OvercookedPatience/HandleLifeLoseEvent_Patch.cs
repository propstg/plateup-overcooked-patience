using HarmonyLib;
using Kitchen;
using Unity.Entities;
using UnityEngine;

namespace KitchenOvercookedPatience {

    [HarmonyPatch(typeof(HandleLifeLoseEvent), "OnUpdate")]
    class HandleLifeLoseEvent_Patch {

        private static void log(object message) {
            Debug.Log(Mod.MOD_ID + ": " + message);
        }

        public static bool Prefix(HandleLifeLoseEvent __instance) {
            int loseLifeEvents = __instance.GetQuery(new QueryHelper().All((ComponentType)typeof(CLoseLifeEvent))).CalculateEntityCount();

            if (loseLifeEvents > 0) {
                log("Intercepting lose life event...");

                if (isModTurnedOff()) {
                    log("Mod is turned off; passing control to handler.");
                    return true;
                }

                SMoney money = __instance.GetSingleton<SMoney>();
                log("Money available: " + money.Amount);

                int moneyToLose = getMoneyToLose(money);
                SMoney newMoney = money - moneyToLose;

                if (money <= 0 || newMoney < 0) {
                    log("Not enough money. Passing control to handler.");
                } else {
                    PatienceCooldownSystem.startPatienceCooldown();
                    log("Buying a life.");
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

        private static bool isModTurnedOff() {
            return OvercookedPatienceSettings.getLoseCoinsSelected() == 0;
        }

        private static SMoney getMoneyToLose(SMoney currentMoney) {
            int coinsToLose = OvercookedPatienceSettings.getLoseCoinsSelected();

            if (coinsToLose == -1) {
                log("Lose all coins is selected. Setting value to lose to current coin total");
                return currentMoney;
            }

            log($"Lose all coins is not selected. Setting value to lose to {coinsToLose}");
            return coinsToLose;
        }

        private static void playSound(EntityManager entityManager) {
            CSoundEvent.Create(entityManager, KitchenData.SoundEvent.MessCreated);
            CSoundEvent.Create(entityManager, KitchenData.SoundEvent.ItemDelivered);
        }
    }
}
