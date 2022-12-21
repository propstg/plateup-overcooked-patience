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
        private static readonly List<int> loseCoinsOptionValues = new List<int> { 0, 5, 10, -1 };
        private static readonly List<string> loseCoinsOptionDisplay = new List<string> { "0", "5", "10", "All coins" };
        private static readonly List<bool> useCooldownValues = new List<bool> { false, true };
        private static readonly List<string> useCooldownLabels = new List<string> { "Off", "On" };

        public OvercookedPatienceMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            AddLabel("Mode");
            Add(new Option<int>(modeValues, Convert.ToInt32(OvercookedPatienceSettings.getMode()), modeLabels))
                .OnChanged += delegate (object _, int value) {
                    OvercookedPatienceSettings.setMode((OvercookedPatienceMode)value);
                };

            AddLabel("Coins to lose");
            Add(new Option<int>(loseCoinsOptionValues, OvercookedPatienceSettings.getLoseCoinsSelected(), loseCoinsOptionDisplay))
                .OnChanged += delegate (object _, int value) {
                    OvercookedPatienceSettings.setLoseCoinsSelected(value);
                };

            AddLabel("Patience cooldown");
            Add(new Option<bool>(useCooldownValues, OvercookedPatienceSettings.getUseCooldownOnPatienceLost(), useCooldownLabels))
                .OnChanged += delegate (object _, bool value) {
                    OvercookedPatienceSettings.setUseCooldownOnPatienceLost(value);
                };
            New<SpacerElement>();
            New<SpacerElement>();

            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }
}
