using UnityEngine;

[CreateAssetMenu(fileName = "DataDeliveredCriteriaSO", menuName = "Objectives/Criteria/DataDelivered")]
public class DataDeliveredCriteriaSO : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        return progData.DataDelivered;
    }
}