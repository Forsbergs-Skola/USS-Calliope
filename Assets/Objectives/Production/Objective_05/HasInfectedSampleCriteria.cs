using UnityEngine;

[CreateAssetMenu(fileName = "HasInfectedSampleCriteria", menuName = "Objectives/Criteria/HasInfectedSampleCriteria")]
public class HasInfectedSampleCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        InventoryData invData = DataController.Instance.InventoryRuntimeData.Value;
        return invData.GetQuestItemIDs().Contains(IDConstants.INFECTED_SAMPLE);
    }
}