using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using System.Collections.Generic;

namespace OvercookedPatience {
    public class OvercookedPatienceMenu : Menu<PauseMenuAction> {

        private static readonly List<int> loseCoinsOptionValues = new List<int> { 0, 5, 10, -1 };
        private static readonly List<string> loseCoinsOptionDisplay = new List<string> { "Off", "5", "10", "All coins" };

        public OvercookedPatienceMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            AddLabel("How many coins would you like to lose?");
            Add(new Option<int>(loseCoinsOptionValues, OvercookedPatienceSettings.loseCoinsSelected, loseCoinsOptionDisplay))
                .OnChanged += delegate (object _, int value) {
                    OvercookedPatienceSettings.loseCoinsSelected = value;
                };

            New<SpacerElement>();
            New<SpacerElement>();

            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }
}
