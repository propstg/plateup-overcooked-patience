using HarmonyLib;
using Kitchen;
using System.Reflection;
using Unity.Entities;
using TMPro;

namespace KitchenOvercookedPatience {

    class MoneyPopup {

        public static void CreateMoneyPopup(EntityManager entityManager, GenericSystemBase genericSystemBase, int money) {
            MethodInfo field = genericSystemBase.GetType().GetMethod("GetCommandBuffer", BindingFlags.Instance | BindingFlags.NonPublic);
            EntityCommandBuffer buffer = (EntityCommandBuffer) field.Invoke(genericSystemBase, new object[] { ECB.End });
            Entity entity = buffer.CreateEntity();
            buffer.AddComponent<CMoneyPopup>(entity, new CMoneyPopup() { Change = money });
            buffer.AddComponent<CPosition>(entity, new CPosition(new UnityEngine.Vector3()));
            buffer.AddComponent<CLifetime>(entity, new CLifetime(1f));
            buffer.AddComponent<CRequiresView>(entity, new CRequiresView() { Type = ViewType.MoneyPopup });
            MoneyTracker.AddEvent(new EntityContext(entityManager, buffer), EndOfDayMoneyTrackerAppliance.ApplianceId, money);
        }
    }

    [HarmonyPatch(typeof(MoneyPopupView), "UpdateData")]
    class MoneyPopupView_DoubleNegativeFixerPatch {

        public static void Postfix(TextMeshPro ___Value) {
            if (___Value.text.StartsWith("--")) {
                ___Value.text = ___Value.text.Substring(1);
            }
        }
    }
}
