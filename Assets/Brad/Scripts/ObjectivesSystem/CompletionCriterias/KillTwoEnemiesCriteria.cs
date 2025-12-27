using UnityEngine;
[CreateAssetMenu(fileName = "KillTwoEnemiesCriteria", menuName = "Objectives/Criteria/KillTwoEnemies")]
public class KillTwoEnemiesCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        return progData.GetDefeatedEnemiesList().Count >= 2;
    }
}
