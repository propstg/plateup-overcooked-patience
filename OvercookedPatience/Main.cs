using BepInEx;
using System.Reflection;
using KitchenLib.Event;
using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace OvercookedPatience {

    [BepInProcess("PlateUp.exe")]
    [BepInPlugin("overcookedpatience", "Overcooked patience", "0.0.1")]
    public class Mod : KitchenLib.BaseMod {

        public Mod() : base(">=1.1.0", Assembly.GetCallingAssembly()) { }

        public void Start() {
            Events.StartMainMenu_SetupEvent += (s, args) => {
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "overcookedpatience");
            };
        }

        [HarmonyPatch(typeof(HandleLifeLoseEvent), "OnUpdate")]
        class HandleLifeLoseEvent_Patch {

            public static bool Prefix(HandleLifeLoseEvent __instance) {
                int loseLifeEvents = __instance.GetQuery(new QueryHelper().All((ComponentType)typeof(CLoseLifeEvent))).CalculateEntityCount();

                if (loseLifeEvents > 0) {
                    Log("Intercepting lose life event...");

                    SMoney money = __instance.GetSingleton<SMoney>();
                    Log("Money available: " + money);

                    if (money <= 0) {
                        Log("Not enough money. Passing control to handler.");
                    } else {
                        Log("Buying a life.");
                        SMoney newMoney = 0;
                        __instance.SetSingleton<SMoney>(newMoney);

                        SKitchenStatus status = __instance.GetSingleton<SKitchenStatus>();
                        status.RemainingLives += 1;
                        __instance.SetSingleton<SKitchenStatus>(status);
                    }
                }
                return true;
            }
        }
    }
}
