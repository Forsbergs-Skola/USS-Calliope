using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapePodTrigger : MonoBehaviour
{
    private Collider myCollider;
    
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;

    private void OnEnable()
    {
        myCollider = GetComponent<Collider>();
        myCollider.enabled = progressionRuntimeData.Value.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_13_ID] == EnumObjectiveStatus.STARTED;
        
        if (EventRelay.Instance == null) return;

        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleRuntimeUpdate;
    }

    private void OnDisable()
    {
        if (EventRelay.Instance == null) return;
        
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= HandleRuntimeUpdate;
    }
    

    private void HandleRuntimeUpdate(IRuntimeData runtimeData)
    {
        //Debug.Log("Runtime Data Updated");
        
        if (!(runtimeData is ProgressionData)) return;
        
        HandleProgressionData();
    }


    private void HandleProgressionData()
    {
        //Debug.Log("Progression Updated");
        
        myCollider.enabled = progressionRuntimeData.Value.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_13_ID] == EnumObjectiveStatus.STARTED;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SceneManager.LoadScene("OutroCutscene");
    }
}
