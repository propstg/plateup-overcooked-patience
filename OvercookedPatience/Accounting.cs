using HarmonyLib;
using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenOvercookedPatience {

    [HarmonyPatch(typeof(MoneyTracker), "AddEvent")]
    class MoneyTrackGetRecord {

        public static bool Prefix(int identifier, int amount, MoneyTracker __instance) {
            if (amount != 0) {
                Debug.Log($"[{Mod.MOD_ID}] In money tracker add event. Adding {amount} for identifier {identifier}");
            }
            return true;
        }
    }

    public class StartOfDayMoneyTracker : StartOfDaySystem, IModSystem {
        protected override void OnUpdate() {
            SMoney money = GetSingleton<SMoney>();
            SDay day = GetSingleton<SDay>();
            Debug.Log($"[{Mod.MOD_ID}] start of day money tracking: day = {day.Day}, money = {(int)money}");
        }
    }

    [UpdateInGroup(typeof(EndOfDayProgressionGroup))]
    [UpdateBefore(typeof(CreateEndOfDayPopup))]
    public class EndOfDayBonusSystem : StartOfNightSystem, IModSystem {
        private EntityQuery players;

        protected override void Initialise() {
            base.Initialise();
            players = GetEntityQuery((ComponentType)typeof(CPlayer));
        }

        protected override void OnUpdate() {
            if (HasSingleton<SIsRestartedDay>() || GetSingleton<SDay>().Day == 0) {
                log("Day was restarted or day is 0. Skipping.");
                return;
            }

            SMoney money = GetSingleton<SMoney>();
            SMoneyEarningsTracker earningsTracker = GetSingleton<SMoneyEarningsTracker>();
            int oldAmount = earningsTracker.OldAmount;
            int newAmount = money - oldAmount;
            float rewardModifier = DifficultyHelpers.MoneyRewardPlayerModifier(players.CalculateEntityCount());
            int bonus = Mathf.CeilToInt((float)newAmount * (rewardModifier - 1f));

            log($"current money = {(int)money}, old amount from tracker = {oldAmount}, money earned this round = {newAmount}, difficulty bonus = {rewardModifier}, player bonus = {bonus}");

            if (bonus < 0) {
                log($"Bonus will be less than zero, which will unexpectedly reduce player money. Attempting to zero out player bonus...");
                earningsTracker.OldAmount = money;
                SetSingleton<SMoneyEarningsTracker>(earningsTracker);
            }
        }

        protected void log(string message) {
            Debug.Log($"[{Mod.MOD_ID}] end of day money tracking: {message}");
        }
    }
}
