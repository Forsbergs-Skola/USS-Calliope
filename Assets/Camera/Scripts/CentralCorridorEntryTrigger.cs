using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class CentralCorridorEntryTrigger : MonoBehaviour
{
    private EventRelay eventRelay = EventRelay.Instance;
    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

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
        bool objectiveTwoStarted = DataController.Instance.ProgressionRuntimeData.Value.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_02_ID] == EnumObjectiveStatus.STARTED;
        myCollider.enabled = objectiveTwoStarted;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DataController.Instance == null) return;
            DataController.Instance.ProgressionRuntimeData.Value.CentralCorridorDiscovered = true;
            Debug.Log("Triggered Corridor");
            myCollider.enabled = false;
        }
    }
}