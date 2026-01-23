using UnityEngine;

[CreateAssetMenu(fileName = "EscapedCriteria", menuName = "Objectives/Criteria/EscapedCriteria")]
public class EscapedCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;

        return progData.Escaped;
    }
}
