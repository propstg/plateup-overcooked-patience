using BepInEx.Logging;
using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace OvercookedPatience {

    [HarmonyPatch(typeof(HandleLifeLoseEvent), "OnUpdate")]
    class HandleLifeLoseEvent_Patch {

        private static ManualLogSource log = Logger.CreateLogSource($"{Mod.MOD_NAME} HandleLifeLoseEvent");

        public static bool Prefix(HandleLifeLoseEvent __instance) {
            int loseLifeEvents = __instance.GetQuery(new QueryHelper().All((ComponentType)typeof(CLoseLifeEvent))).CalculateEntityCount();

            if (loseLifeEvents > 0) {
                log.LogInfo("Intercepting lose life event...");

                if (isModTurnedOff()) {
                    log.LogInfo("Mod is turned off; passing control to handler.");
                    return true;
                }

                SMoney money = __instance.GetSingleton<SMoney>();
                log.LogInfo("Money available: " + money.Amount);

                int moneyToLose = getMoneyToLose(money);
                SMoney newMoney = money - moneyToLose;

                if (money <= 0 || newMoney < 0) {
                    log.LogInfo("Not enough money. Passing control to handler.");
                } else {
                    Mod.startPatienceCooldown();
                    log.LogInfo("Buying a life.");
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
            return OvercookedPatienceSettings.loseCoinsSelected == 0;
        }

        private static SMoney getMoneyToLose(SMoney currentMoney) {
            if (OvercookedPatienceSettings.loseCoinsSelected == -1) {
                log.LogInfo("Lose all coins is selected. Setting value to lose to current coin total");
                return currentMoney;
            }

            log.LogInfo($"Lose all coins is not selected. Setting value to lose to {OvercookedPatienceSettings.loseCoinsSelected}");
            return OvercookedPatienceSettings.loseCoinsSelected;
        }

        private static void playSound(EntityManager entityManager) {
            CSoundEvent.Create(entityManager, KitchenData.SoundEvent.MessCreated);
            CSoundEvent.Create(entityManager, KitchenData.SoundEvent.ItemDelivered);
        }
    }
}
