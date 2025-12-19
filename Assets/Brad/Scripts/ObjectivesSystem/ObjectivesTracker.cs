using UnityEngine;
using System.Collections.Generic;

public enum EnumObjective
{
    NONE,
    ENTER_THE_LAB,
    TALK_TO_ALICE,
    DEFEAT_ALICE,
    DEFEAT_FOUR_ENEMIES,
    DO_A_LITTLE_DANCE,
    MAKE_A_LITTLE_LOVE,
    GET_DOWN_TONIGHT
    // more as needed
}
public class ObjectivesTracker : Singleton<ObjectivesTracker>
{
    [SerializeField] private List<ObjectiveSO> objectives;

    private void OnEnable()
    {
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleProgressionDataUpdate;
    }
    private void OnDisable()
    {
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= HandleProgressionDataUpdate;
    }

    public void ResetObjectives()
    {
        
    }

    private void HandleProgressionDataUpdate(IRuntimeData data)
    {
        if (!(data is ProgressionData)) return;
        ProgressionData progData = data as ProgressionData;

    }

    private bool GetIsCriteriaListMet(List<ObjectiveCriterion> criteriaList)
    {
        bool isMet = true;



        return isMet;
    }

}
