using UnityEngine;

[CreateAssetMenu(fileName = "DataDeliveredCriteriaSO", menuName = "Objectives/Criteria/DataDelivered")]
public class DataDeliveredCriteriaSO : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        InventoryData invData = DataController.Instance.InventoryRuntimeData.Value;
        return invData.GetQuestItemIDs().Contains(IDConstants.STATION_DATA);
    }
}