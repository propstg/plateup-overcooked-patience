using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using System.Collections.Generic;
using KitchenLib;
using System;

namespace KitchenOvercookedPatience {

    public class OvercookedPatienceMenu<T> : KLMenu<T> {

        private static readonly List<int> modeValues = new List<int> { 0, 3, 1, 4, 5, 6, 2 };
        private static readonly List<string> modeLabels = new List<string> { "Off (Default Game)", "No Patience Penalty", "Lose Coins - Fixed", "Lose Coins - Progressive", "Lose Coins - Exponential", "Lose Coins - All", "3 Strikes" };
        private static readonly List<int> loseCoinsOptionValues = new List<int> { 5, 10, 15, 20 };
        private static readonly List<string> loseCoinsOptionDisplay = new List<string> { "5", "10", "15", "20" };
        private static readonly List<bool> useCooldownValues = new List<bool> { false, true };
        private static readonly List<string> useCooldownLabels = new List<string> { "Off", "On" };

        private SelectElement coinsSelect = null;
        private SelectElement cooldownSelect = null;
        InfoBoxElement modeInfo = null;

        public OvercookedPatienceMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            addModeSelect();
            coinsSelect = addCoinsSelect();
            cooldownSelect = addCooldownSection();
            addResetStrikesEachDaySection();
            addBackButton();
        }

        private void addModeSelect() {
            Option<int> option = new Option<int>(modeValues, Convert.ToInt32(OvercookedPatienceSettings.getMode()), modeLabels);

            AddLabel("Mode");
            AddSelect(option);
            modeInfo = AddInfo(getModeInfoLabel(Convert.ToInt32(OvercookedPatienceSettings.getMode())));

            option.OnChanged += delegate (object _, int value) {
                OvercookedPatienceSettings.setMode((OvercookedPatienceMode)value);
                modeInfo.SetLabel(getModeInfoLabel(value));

                coinsSelect?.SetSelectable(
                    (OvercookedPatienceMode)value == OvercookedPatienceMode.LOSE_COINS_FIXED ||
                    (OvercookedPatienceMode)value == OvercookedPatienceMode.LOSE_COINS_PROGRESSIVE ||
                    (OvercookedPatienceMode)value == OvercookedPatienceMode.LOSE_COINS_EXPONENTIAL);
                cooldownSelect?.SetSelectable((OvercookedPatienceMode)value != OvercookedPatienceMode.OFF);
            };
        }

        private string getModeInfoLabel(int value) {
            int loseCoins = OvercookedPatienceSettings.getLoseCoinsSelected();

            string label = "";
            switch (value) {
                case (int)OvercookedPatienceMode.OFF: label = "Game will operate normally, with no protection from the mod."; break;
                case (int)OvercookedPatienceMode.NO_PATIENCE_PENALTY: label = "No penalty will be applied when a customer loses patience."; break;
                case (int)OvercookedPatienceMode.LOSE_COINS_FIXED: label = $"Lose {loseCoins} coins each time a customer loses patience (change below). If you don't have enough, the game ends."; break;
                case (int)OvercookedPatienceMode.LOSE_COINS_PROGRESSIVE: label = $"Lose {loseCoins} coins the first time a customer loses patience, {loseCoins * 2} the 2nd, {loseCoins * 3} the 3rd, etc (change {loseCoins} below). If you don't have enough, the game ends."; break;
                case (int)OvercookedPatienceMode.LOSE_COINS_EXPONENTIAL: label = $"Lose {loseCoins} coins the first time a customer loses patience, {loseCoins * Math.Pow(2, 1)} the 2nd, {loseCoins * Math.Pow(2, 2)} the 3rd, etc (change {loseCoins} below). If you don't have enough, the game ends."; break;
                case (int)OvercookedPatienceMode.LOSE_COINS_ALL: label = $"Lose all of your current coin total each time a customer loses patience. If you have 0 coins, the game ends."; break;
                case (int)OvercookedPatienceMode.STRIKES: label = "Up to 3 customers per day/run can lose patience (change below). If a 4th loses patience, the game ends."; break;
            }
            return label;
        }

        private SelectElement addCoinsSelect() {
            Option<int> option = new Option<int>(loseCoinsOptionValues, OvercookedPatienceSettings.getLoseCoinsSelected(), loseCoinsOptionDisplay);

            AddLabel("Coins to lose");
            SelectElement selectElement = AddSelect(option);

            option.OnChanged += delegate (object _, int value) {
                OvercookedPatienceSettings.setLoseCoinsSelected(value);
                modeInfo.SetLabel(getModeInfoLabel((int)OvercookedPatienceSettings.getMode()));
            };

            return selectElement;
        }

        private SelectElement addCooldownSection() {
            Option<bool> option = new Option<bool>(useCooldownValues, OvercookedPatienceSettings.getUseCooldownOnPatienceLost(), useCooldownLabels);

            AddLabel("Patience cooldown");
            AddInfo("Pause patience timers briefly after a customer loses patience.");
            SelectElement selectElement = AddSelect(option);

            option.OnChanged += delegate (object _, bool value) {
                OvercookedPatienceSettings.setUseCooldownOnPatienceLost(value);
            };

            return selectElement;
        }

        private SelectElement addResetStrikesEachDaySection() {
            Option<bool> option = new Option<bool>(useCooldownValues, OvercookedPatienceSettings.getResetStrikesEachDay(), useCooldownLabels);

            AddLabel("Reset each day");
            AddInfo("Reset strikes/progressive/exponential mode each day. Turn this off to make things more difficult.");
            SelectElement selectElement = AddSelect(option);
            AddInfo("Note: Strikes will only reset on the first day, if disabled. Changing the mode mid-run might have unexpected results.");

            option.OnChanged += delegate (object _, bool value) {
                OvercookedPatienceSettings.setResetStrikesEachDay(value);
            };

            return selectElement;
        }

        private void addBackButton() {
            New<SpacerElement>();
            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }
}
