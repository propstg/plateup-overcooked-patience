using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using System.Collections.Generic;
using KitchenLib;
using KitchenLib.Utils;

namespace KitchenOvercookedPatience {

    public class OvercookedPatienceMenu<T> : KLMenu<T> {

        private static readonly List<int> loseCoinsOptionValues = new List<int> { 0, 5, 10, -1 };
        private static readonly List<string> loseCoinsOptionDisplay = new List<string> { "Off", "5", "10", "All coins" };

        public OvercookedPatienceMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            KitchenLib.IntPreference loseCoinsPreference = PreferenceUtils.Get<KitchenLib.IntPreference>(Mod.MOD_ID, OvercookedPatienceSettings.LOSE_COINS_KEY);

            AddLabel("How many coins would you like to lose?");
            Add(new Option<int>(loseCoinsOptionValues, loseCoinsPreference.Value, loseCoinsOptionDisplay))
                .OnChanged += delegate (object _, int value) {
                    loseCoinsPreference.Value = value;
                };

            AddLabel("Patience cooldown after losing a life?");
            BoolOption(PreferenceUtils.Get<KitchenLib.BoolPreference>(Mod.MOD_ID, OvercookedPatienceSettings.USE_COOLDOWN_KEY));
            New<SpacerElement>();
            New<SpacerElement>();

            AddButton(Localisation["MENU_APPLY_SETTINGS"], delegate {
                PreferenceUtils.Save();
                RequestPreviousMenu();
            });
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { PreferenceUtils.Save(); });
        }
    }
}
