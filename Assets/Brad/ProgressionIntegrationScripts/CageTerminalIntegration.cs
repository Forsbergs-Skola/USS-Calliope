using UnityEngine;
using Events;

public class CageTerminalIntegration : MonoBehaviour
{
    [SerializeField] private ProgressionRuntimeData progressionData;
    private Collider myCollider;


    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {

        ProgressionData progData = progressionData.Value;
        bool beOn = progData.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_04_ID] == EnumObjectiveStatus.FINISHED;
        myCollider.enabled = beOn;



        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleRuntimeDataUpdate;
    }
    private void OnDisable()
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= HandleRuntimeDataUpdate;
    }
    private void HandleRuntimeDataUpdate(IRuntimeData _data)
    {
        if (!(_data is ProgressionData)) return;

        //Debug.Log("FOO");


        ProgressionData progData = progressionData.Value;
        bool beOn = progData.ObjectivesAndStatusesDict[IDConstants.OBJECTIVE_04_ID] == EnumObjectiveStatus.FINISHED;
        myCollider.enabled = beOn;

    }
}
