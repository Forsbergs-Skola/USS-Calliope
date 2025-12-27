using UnityEngine;

[CreateAssetMenu(fileName = "KillBobCompletionCriteria", menuName = "Objectives/Criteria/KillBob")]
public class KillBobCompletionCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        return progData.GetDefeatedEnemiesList().Contains("Bob");
    }
}
