using UnityEngine;

[CreateAssetMenu(fileName = "InvestigateNoiseCriteria", menuName = "Objectives/Criteria/InvestigateNoiseCriteria")]
public class InvestigateNoiseCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        return progData.NoiseInvestigated;
    }
}
