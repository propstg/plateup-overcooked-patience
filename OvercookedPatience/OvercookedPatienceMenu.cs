using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using System.Collections.Generic;
using KitchenLib;
using System;

namespace KitchenOvercookedPatience {

    public class OvercookedPatienceMenu<T> : KLMenu<T> {

        private static readonly List<int> modeValues = new List<int> { 0, 1, 2 };
        private static readonly List<string> modeLabels = new List<string> { "Off", "Lose Coins", "3 Strikes" };
        private static readonly List<int> loseCoinsOptionValues = new List<int> { 0, 5, 10, -1, -2, -3 };
        private static readonly List<string> loseCoinsOptionDisplay = new List<string> { "0", "5", "10", "All coins", "Progressive", "Exponential" };
        private static readonly List<bool> useCooldownValues = new List<bool> { false, true };
        private static readonly List<string> useCooldownLabels = new List<string> { "Off", "On" };

        private SelectElement coinsSelect = null;
        private SelectElement cooldownSelect = null;

        public OvercookedPatienceMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            addModeSelect();
            coinsSelect = addCoinsSelect();
            cooldownSelect = addCooldownSection();
            addBackButton();
        }

        private void addModeSelect() {
            Option<int> option = new Option<int>(modeValues, Convert.ToInt32(OvercookedPatienceSettings.getMode()), modeLabels);

            AddLabel("Mode");
            AddSelect(option);
            InfoBoxElement modeInfo = AddInfo(getModeInfoLabel(Convert.ToInt32(OvercookedPatienceSettings.getMode())));

            option.OnChanged += delegate (object _, int value) {
                OvercookedPatienceSettings.setMode((OvercookedPatienceMode)value);
                modeInfo.SetLabel(getModeInfoLabel(value));

                coinsSelect?.SetSelectable((OvercookedPatienceMode)value == OvercookedPatienceMode.LOSE_COINS);
                cooldownSelect?.SetSelectable((OvercookedPatienceMode)value != OvercookedPatienceMode.OFF);
            };
        }

        private string getModeInfoLabel(int value) {
            string label = "";
            switch (value) {
                case 0: label = "Game will operate normally, with no protection from the mod."; break;
                case 1: label = "Lose coins each time a customer loses patience. If you don't have enough, the game ends."; break;
                case 2: label = "Up to 3 customers per day can lose patience. If a 4th loses patience, the game ends."; break;
            }
            return label;
        }

        private SelectElement addCoinsSelect() {
            Option<int> option = new Option<int>(loseCoinsOptionValues, OvercookedPatienceSettings.getLoseCoinsSelected(), loseCoinsOptionDisplay);

            AddLabel("Coins to lose");
            SelectElement selectElement = AddSelect(option);
            InfoBoxElement coinsInfo = AddInfo(getCoinInfoLabel(OvercookedPatienceSettings.getLoseCoinsSelected()));

            option.OnChanged += delegate (object _, int value) {
                OvercookedPatienceSettings.setLoseCoinsSelected(value);
                coinsInfo.SetLabel(getCoinInfoLabel(value));
            };

            return selectElement;
        }

        private string getCoinInfoLabel(int value) {
            string label = "";
            switch (value) {
                case 0: label = "Effectively turns patience off."; break;
                case 5: label = "Lose 5 coins when a customer loses patience."; break;
                case 10: label = "Lose 10 coins when a customer loses patience."; break;
                case -1: label = "Lose all of your coins when a customer loses patience."; break;
                case -2: label = "Lose 5 coins the first time a customer loses patience in a day, 10 coins the 2nd time, 15 the 3rd, etc."; break;
                case -3: label = "Lose 5 coins the first time a customer loses patience in a day, 10 coins the 2nd time, 20 the 3rd, etc."; break;
            }
            return label;
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

        private void addBackButton() {
            New<SpacerElement>();
            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }
}
