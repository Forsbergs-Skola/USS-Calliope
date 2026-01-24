using UnityEngine;


public class BobDisabler : MonoBehaviour
{

    private void Awake()
    {
        if (DataController.Instance == null) return;
        ProgressionData d = DataController.Instance.ProgressionRuntimeData.Value;
        gameObject.SetActive(d.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_07_ID] != EnumObjectiveStatus.FINISHED);
    }

    private void OnEnable()
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleDataUpdate;
    }

    private void OnDisable()
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= HandleDataUpdate;
    }

    private void HandleDataUpdate(IRuntimeData updatedData)
    {
        if (!(updatedData is ProgressionData)) return;
        HandleProgressionData(updatedData as ProgressionData);
    }


    private void HandleProgressionData(ProgressionData progData)
    {
        EnumObjectiveStatus linaxObjStatus = progData.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_07_ID];
        gameObject.SetActive(linaxObjStatus != EnumObjectiveStatus.FINISHED);
    }

}
