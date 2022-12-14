using KitchenLib.Customs;
using KitchenData;

namespace KitchenOvercookedPatience {

    class EndOfDayMoneyTrackerAppliance : CustomAppliance {

        public static int ApplianceId;

        public override void OnRegister(GameDataObject gameDataObject) {
            ((Appliance)gameDataObject).Name = Mod.MOD_NAME;
            ApplianceId = gameDataObject.ID;
        }
    }
}
