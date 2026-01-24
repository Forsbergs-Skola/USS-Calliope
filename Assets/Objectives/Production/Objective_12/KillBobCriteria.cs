using UnityEngine;

[CreateAssetMenu(fileName = "KillBobCriteria", menuName = "Objectives/Criteria/KillBobCriteria")]
public class KillBobCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        return progData.GetDefeatedEnemiesList().Contains("Bob");
    }
}
