using UnityEngine;

[CreateAssetMenu(fileName = "BobContactedCriteria", menuName = "Objectives/Criteria/BobContactedCriteria")]
public class BobContactedCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        return progData.BobContacted;
    }
}