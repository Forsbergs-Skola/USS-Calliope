using UnityEngine;

[CreateAssetMenu(fileName = "HasFlashlightCriteriaSO", menuName = "Objectives/Criteria/HasFlashlightCriteria")]
public class HasFlashlightCriteriaSO : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        InventoryData data = DataController.Instance.InventoryRuntimeData.Value;
        return data.GetQuestItemIDs().Contains("6d81adca-c9f4-44bd-8523-ce3f820faa19");
    }
}