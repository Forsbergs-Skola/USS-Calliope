using UnityEngine;

[CreateAssetMenu(fileName = "HasFlashlightCriteriaSO", menuName = "Objectives/Criteria/HasFlashlightCriteria")]
public class HasFlashlightCriteriaSO : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        InventoryData data = DataController.Instance.InventoryRuntimeData.Value;
        return data.GetQuestItemIDs().Contains(IDConstants.FLASHLIGHT_INV_ID);
    }
}