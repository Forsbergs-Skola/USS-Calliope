using UnityEngine;

public class BloodSampleGiver : MonoBehaviour
{
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;
    [SerializeField] private InventoryRuntimeData inventoryRuntimeData;




    public void TryGiveSample()
    {
        ProgressionData progData = progressionRuntimeData.Value;

        bool giveSample = progData.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_05_ID] == EnumObjectiveStatus.STARTED;
        if (!giveSample) return;

        InventoryData invData = inventoryRuntimeData.Value;
        invData.AddQuestItem(IDConstants.INFECTED_SAMPLE);
    }

}
