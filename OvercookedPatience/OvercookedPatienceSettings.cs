using UnityEngine;
using Kitchen.Modules;
using KitchenLib.Utils;

namespace OvercookedPatience {

    public class OvercookedPatienceConsts {

        public const string LOSE_ALL_COINS = "loseallcoins";
        public const string LOSE_5_COINS = "lose5coins";
        public const string LOSE_10_COINS = "lose10coins";
        public const string LOSE_ALL_COINS_DISPLAY = "Lose all coins";
        public const string LOSE_5_COINS_DISPLAY = "Lose 5 coins";
        public const string LOSE_10_COINS_DISPLAY = "Lose 10 coins";
        public const int LOSE_5_COINS_VALUE = 5;
        public const int LOSE_10_COINS_VALUE = 10;
    }

    public class OvercookedPatienceSettings<T> : KitchenLib.KLMenu<T> {

        public OvercookedPatienceSettings(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            AddLabel(OvercookedPatienceConsts.LOSE_ALL_COINS_DISPLAY);
            BoolOption(PreferenceUtils.Get<KitchenLib.BoolPreference>(Mod.MOD_ID, OvercookedPatienceConsts.LOSE_ALL_COINS));
            AddLabel(OvercookedPatienceConsts.LOSE_10_COINS_DISPLAY);
            BoolOption(PreferenceUtils.Get<KitchenLib.BoolPreference>(Mod.MOD_ID, OvercookedPatienceConsts.LOSE_10_COINS));
            AddLabel(OvercookedPatienceConsts.LOSE_5_COINS_DISPLAY);
            BoolOption(PreferenceUtils.Get<KitchenLib.BoolPreference>(Mod.MOD_ID, OvercookedPatienceConsts.LOSE_5_COINS));

            New<SpacerElement>();
            AddInfo("The first \"On\" setting will take priority.");
            New<SpacerElement>();

            AddButton(Localisation["MENU_APPLY_SETTINGS"], delegate {
                PreferenceUtils.Save();
                RequestPreviousMenu();
            });
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }
}
