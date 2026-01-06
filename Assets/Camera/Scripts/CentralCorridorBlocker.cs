using System;
using UnityEngine;
using UnityEngine.XR;

public class CentralCorridorBlocker : MonoBehaviour
{
    private EventRelay eventRelay = EventRelay.Instance;
    
    private void Start()
    {
        HandleDataUpdate();
    }

    void OnEnable()
    {
        if (EventRelay.Instance == null) return;
        eventRelay.GameEvents.DataUpdatedEvent.OnEventTriggered += HandleDataUpdate;
    }

    void OnDisable()
    {
        if (EventRelay.Instance == null) return;
        eventRelay.GameEvents.DataUpdatedEvent.OnEventTriggered -= HandleDataUpdate;
    }

    private void HandleDataUpdate()
    {
        if (DataController.Instance == null) return;
        bool objectiveOneFinished = DataController.Instance.ProgressionRuntimeData.Value.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_01_ID] == EnumObjectiveStatus.FINISHED;
        gameObject.SetActive(!objectiveOneFinished);
    }
}
