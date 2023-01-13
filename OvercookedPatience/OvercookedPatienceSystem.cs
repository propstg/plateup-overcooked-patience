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

                #if !DEBUG
                    SDay day = GetSingleton<SDay>();
                    if (day.Day < OvercookedPatienceSettings.ACTIVATE_ON_DAY) {
                        log($"Day {day.Day} is less than mod activation day ({OvercookedPatienceSettings.ACTIVATE_ON_DAY}). Passing control to handler.");
                        return;
                    }
                #endif

                SOvercookedStrikes strikes = GetOrDefault<SOvercookedStrikes>();
                strikes.Strikes++;
                Set(strikes);

                switch (OvercookedPatienceSettings.getMode()) {
                    case OvercookedPatienceMode.OFF:
                        handleModTurnedOffMode();
                        break;
                    case OvercookedPatienceMode.NO_PATIENCE_PENALTY:
                        handleNoPatiencePenaltyMode();
                        break;
                    case OvercookedPatienceMode.STRIKES:
                        handleStrikesMode(strikes.Strikes);
                        break;
                    default:
                        handleLoseCoinsMode(strikes.Strikes);
                        break;
                }
            }
        }

        private void handleModTurnedOffMode() {
            log("Mod is turned off; passing control to handler.");
        }

        private void handleNoPatiencePenaltyMode() {
            log("Mod is in NO_PATIENCE_PENALTY mode; clearing lose life events.");
            clearLoseLifeEvents();
        }

        private void handleLoseCoinsMode(int strikes) {
            log("Mod is in LOSE_COINS mode; attempting to buy life.");

            SMoney money = GetSingleton<SMoney>();
            log("Money available: " + money.Amount);

            int moneyToLose = getMoneyToLose(money, strikes);
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

        private SMoney getMoneyToLose(SMoney currentMoney, int strikes) {
            OvercookedPatienceMode mode = OvercookedPatienceSettings.getMode();
            int coinsToLose = OvercookedPatienceSettings.getLoseCoinsSelected();

            if (mode == OvercookedPatienceMode.LOSE_COINS_ALL) {
                log("Lose all coins is selected. Setting value to lose to current coin total");
                return currentMoney;
            } else if (mode == OvercookedPatienceMode.LOSE_COINS_PROGRESSIVE) {
                log($"Progressive is selected, with {coinsToLose} set as base. Setting value to lose to {strikes * coinsToLose} (current strikes ({strikes}) * {coinsToLose}).");
                return strikes * coinsToLose;
            } else if (mode == OvercookedPatienceMode.LOSE_COINS_EXPONENTIAL) {
                log($"Exponential is selected, with {coinsToLose} set as base. Setting value to lose to {coinsToLose * Math.Pow(2, strikes - 1)} ({coinsToLose} * current strikes 2^({strikes - 1})).");
                return (SMoney)(coinsToLose * Math.Pow(2, strikes - 1));
            }

            log($"Fixed is selected. Setting value to lose to {coinsToLose}");
            return coinsToLose;
        }

        private void handleStrikesMode(int strikes) {
            log("Mod is in STRIKE mode.");

            if (strikes < OvercookedPatienceSettings.MAX_STRIKES) {
                log($"Current strikes: {strikes} less than {OvercookedPatienceSettings.MAX_STRIKES}; adding a life.");
                playSound();
                clearLoseLifeEvents();
            } else {
                log($"Current strikes: {strikes} greater than/equal to {OvercookedPatienceSettings.MAX_STRIKES}; passing control to handler.");
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
