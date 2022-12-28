using HarmonyLib;
using Kitchen;
using System;
using Unity.Entities;
using UnityEngine;

namespace KitchenOvercookedPatience {

    [HarmonyPatch(typeof(HandleLifeLoseEvent), "OnUpdate")]
    class HandleLifeLoseEvent_Patch {

        private static readonly int MAX_STRIKES = 3;

        private static void log(object message) {
            Debug.Log(Mod.MOD_ID + ": " + message);
        }

        public static bool Prefix(HandleLifeLoseEvent __instance) {
            int loseLifeEvents = __instance.GetQuery(new QueryHelper().All((ComponentType)typeof(CLoseLifeEvent))).CalculateEntityCount();

            if (loseLifeEvents > 0) {
                log("Intercepting lose life event...");

                switch (OvercookedPatienceSettings.getMode()) {
                    case OvercookedPatienceMode.OFF:
                        handleModTurnedOffMode();
                        break;
                    case OvercookedPatienceMode.LOSE_COINS:
                        handleLoseCoinsMode(__instance);
                        break;
                    case OvercookedPatienceMode.STRIKES:
                        handleStrikesMode(__instance);
                        break;
                }
            }

            return true;
        }

        private static void handleModTurnedOffMode() {
            log("Mod is turned off; passing control to handler.");
        }

        private static void handleLoseCoinsMode(HandleLifeLoseEvent __instance) {
            log("Mod is in LOSE_COINS mode; attempting to buy life.");

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
                addLifeToKitchenStatus(__instance);
                MoneyPopup.CreateMoneyPopup(__instance.EntityManager, __instance, -moneyToLose);
            }
        }

        private static SMoney getMoneyToLose(SMoney currentMoney) {
            int coinsToLose = OvercookedPatienceSettings.getLoseCoinsSelected();

            if (coinsToLose == OvercookedPatienceSettings.ALL_COINS) {
                log("Lose all coins is selected. Setting value to lose to current coin total");
                return currentMoney;
            } else if (coinsToLose == OvercookedPatienceSettings.PROGRESSIVE) {
                StrikeSystem.addStrike();
                int strikes = StrikeSystem.getStrikes();
                log($"Progressive is selected. Setting value to lose to {strikes * 5} (current strikes ({strikes}) * 5).");
                return strikes * 5;
            } else if (coinsToLose == OvercookedPatienceSettings.EXPONENTIAL) {
                StrikeSystem.addStrike();
                int strikes = StrikeSystem.getStrikes();
                log($"Exponential is selected. Setting value to lose to {5 * Math.Pow(2, strikes - 1)} (5 * current strikes 2^({strikes - 1})).");
                return (SMoney)(5 * Math.Pow(2, strikes - 1));
            }

            log($"Lose all coins/progressive not selected. Setting value to lose to {coinsToLose}");
            return coinsToLose;
        }

        private static void handleStrikesMode(HandleLifeLoseEvent __instance) {
            log("Mod is in STRIKE mode; attempting to add a strike.");

            StrikeSystem.addStrike();

            if (StrikeSystem.getStrikes() < MAX_STRIKES) {
                log($"Current strikes: {StrikeSystem.getStrikes()} less than {MAX_STRIKES}; adding a life.");
                addLifeToKitchenStatus(__instance);
                playSound(__instance.EntityManager);
            } else {
                log($"Current strikes: {StrikeSystem.getStrikes()} greater than/equal to {MAX_STRIKES}; passing control to handler.");
            }
        }

        private static void addLifeToKitchenStatus(HandleLifeLoseEvent __instance) {
            SKitchenStatus status = __instance.GetSingleton<SKitchenStatus>();
            status.RemainingLives += 1;
            __instance.SetSingleton<SKitchenStatus>(status);
        }

        private static void playSound(EntityManager entityManager) {
            CSoundEvent.Create(entityManager, KitchenData.SoundEvent.MessCreated);
            CSoundEvent.Create(entityManager, KitchenData.SoundEvent.ItemDelivered);
        }
    }
}
