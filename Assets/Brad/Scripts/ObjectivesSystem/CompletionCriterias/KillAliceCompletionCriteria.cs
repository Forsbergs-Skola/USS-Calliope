using UnityEngine;

[CreateAssetMenu(fileName = "KillAliceCompletionCriteria", menuName = "Objectives/Criteria/KillAlice")]
public class KillAliceCompletionCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        return progData.GetDefeatedEnemiesList().Contains("Alice");
    }
}
