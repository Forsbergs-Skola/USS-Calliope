using UnityEngine;

[CreateAssetMenu(fileName = "CrewQuartersUnlockedCriteria", menuName = "Objectives/Criteria/CrewQuartersUnlockedCriteria")]
public class CrewQuartersUnlockedCriteria : CompletionCriteriaSO
{
    public override bool IsCriteriaMet()
    {
        if (DataController.Instance == null) return false;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;
        return progData.CrewQuartersUnlocked;
    }
}