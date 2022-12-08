using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using System.Collections.Generic;
using KitchenLib;

namespace KitchenOvercookedPatience {

    public class OvercookedPatienceMenu<T> : KLMenu<T> {

        private static readonly List<int> loseCoinsOptionValues = new List<int> { 0, 5, 10, -1 };
        private static readonly List<string> loseCoinsOptionDisplay = new List<string> { "Off", "5", "10", "All coins" };
        private static readonly List<bool> useCooldownValues = new List<bool> { false, true };
        private static readonly List<string> useCooldownLabels = new List<string> { "Off", "On" };

        public OvercookedPatienceMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            AddLabel("How many coins would you like to lose?");
            Add(new Option<int>(loseCoinsOptionValues, OvercookedPatienceSettings.loseCoinsSelected, loseCoinsOptionDisplay))
                .OnChanged += delegate (object _, int value) {
                    OvercookedPatienceSettings.loseCoinsSelected = value;
                };

            AddLabel("Patience cooldown after losing a life?");
            Add(new Option<bool>(useCooldownValues, OvercookedPatienceSettings.useCooldownOnPatienceLost, useCooldownLabels))
                .OnChanged += delegate (object _, bool value) {
                    OvercookedPatienceSettings.useCooldownOnPatienceLost = value;
                };
            New<SpacerElement>();
            New<SpacerElement>();

            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }
}
