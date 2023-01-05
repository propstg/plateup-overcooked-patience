using Kitchen;
using KitchenMods;
using System;
using Unity.Entities;
using UnityEngine;

namespace KitchenOvercookedPatience {

    [UpdateBefore(typeof(HandleLifeLoseEvent))]
    public class OvercookedPatienceSystem : RestaurantSystem, IModSystem {

        private EntityQuery loseLifeEventQuery;

        protected override void Initialise() {
            base.Initialise();
            loseLifeEventQuery = GetEntityQuery((ComponentType)typeof(CLoseLifeEvent));
        }

        protected override void OnUpdate() {
            int loseLifeEvents = loseLifeEventQuery.CalculateEntityCount();

            if (loseLifeEvents > 0) {
                log("Intercepting lose life event...");
                StrikeSystem.addStrike();

                switch (OvercookedPatienceSettings.getMode()) {
                    case OvercookedPatienceMode.OFF:
                        handleModTurnedOffMode();
                        break;
                    case OvercookedPatienceMode.LOSE_COINS:
                        handleLoseCoinsMode();
                        break;
                    case OvercookedPatienceMode.STRIKES:
                        handleStrikesMode();
                        break;
                }
            }
        }

        private void handleModTurnedOffMode() {
            log("Mod is turned off; passing control to handler.");
        }

        private void handleLoseCoinsMode() {
            log("Mod is in LOSE_COINS mode; attempting to buy life.");

            SMoney money = GetSingleton<SMoney>();
            log("Money available: " + money.Amount);

            int moneyToLose = getMoneyToLose(money);
            SMoney newMoney = money - moneyToLose;

            if (money <= 0 || newMoney < 0) {
                log("Not enough money. Passing control to handler.");
            } else {
                PatienceCooldownSystem.startPatienceCooldown();
                log("Buying a life.");
                playSound();
                SetSingleton<SMoney>(newMoney);
                MoneyPopup.CreateMoneyPopup(EntityManager, this, -moneyToLose);
                clearLoseLifeEvents();
            }
        }

        private SMoney getMoneyToLose(SMoney currentMoney) {
            int coinsToLose = OvercookedPatienceSettings.getLoseCoinsSelected();

            if (coinsToLose == OvercookedPatienceSettings.ALL_COINS) {
                log("Lose all coins is selected. Setting value to lose to current coin total");
                return currentMoney;
            } else if (coinsToLose == OvercookedPatienceSettings.PROGRESSIVE) {
                int strikes = StrikeSystem.getStrikes();
                log($"Progressive is selected. Setting value to lose to {strikes * 5} (current strikes ({strikes}) * 5).");
                return strikes * 5;
            } else if (coinsToLose == OvercookedPatienceSettings.EXPONENTIAL) {
                int strikes = StrikeSystem.getStrikes();
                log($"Exponential is selected. Setting value to lose to {5 * Math.Pow(2, strikes - 1)} (5 * current strikes 2^({strikes - 1})).");
                return (SMoney)(5 * Math.Pow(2, strikes - 1));
            }

            log($"Lose all coins/progressive not selected. Setting value to lose to {coinsToLose}");
            return coinsToLose;
        }

        private void handleStrikesMode() {
            log("Mod is in STRIKE mode.");

            if (StrikeSystem.getStrikes() < OvercookedPatienceSettings.MAX_STRIKES) {
                log($"Current strikes: {StrikeSystem.getStrikes()} less than {OvercookedPatienceSettings.MAX_STRIKES}; adding a life.");
                playSound();
                clearLoseLifeEvents();
            } else {
                log($"Current strikes: {StrikeSystem.getStrikes()} greater than/equal to {OvercookedPatienceSettings.MAX_STRIKES}; passing control to handler.");
            }
        }

        private void playSound() {
            CSoundEvent.Create(EntityManager, KitchenData.SoundEvent.MessCreated);
            CSoundEvent.Create(EntityManager, KitchenData.SoundEvent.ItemDelivered);
        }

        private void clearLoseLifeEvents() {
            EntityManager.DestroyEntity(loseLifeEventQuery);
        }

        private void log(object message) {
            Debug.Log($"[{Mod.MOD_ID}] {message}");
        }
    }
}
