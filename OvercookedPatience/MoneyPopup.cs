using HarmonyLib;
using Kitchen;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using TMPro;

namespace KitchenOvercookedPatience {

    class MoneyPopup {

        public static void CreateMoneyPopup(EntityManager entityManager, GenericSystemBase genericSystemBase, int money) {
            FieldInfo field = genericSystemBase.GetType().GetField("ECBs", BindingFlags.Instance | BindingFlags.NonPublic);
            Dictionary<ECB, EntityCommandBufferSystem> ecbs = (Dictionary<ECB, EntityCommandBufferSystem>)field.GetValue(genericSystemBase);

            EntityCommandBuffer buffer = ecbs[ECB.End].CreateCommandBuffer();
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
