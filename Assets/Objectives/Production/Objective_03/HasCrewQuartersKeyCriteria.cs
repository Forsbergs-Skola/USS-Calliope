using UnityEngine;

[CreateAssetMenu(fileName = "HasCrewQuartersKeyCriteria", menuName = "Objectives/Criteria/HasCrewQuartersKeyCriteria")]
public class HasCrewQuartersKeyCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        InventoryData invData = DataController.Instance.InventoryRuntimeData.Value;
        return invData.GetQuestItemIDs().Contains(IDConstants.CREW_QUARTERS_KEY);
    }
}