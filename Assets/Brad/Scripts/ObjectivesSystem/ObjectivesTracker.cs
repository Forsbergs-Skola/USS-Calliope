using UnityEngine;
using System.Collections.Generic;

public enum EnumObjective
{
    NONE,
    //ENTER_THE_LAB,
    //TALK_TO_ALICE,
    DEFEAT_ALICE,
    DEFEAT_BOB,
    KILL_TWO_ENEMIES,
    //DEFEAT_FOUR_ENEMIES,
    DO_A_LITTLE_DANCE,
    MAKE_A_LITTLE_LOVE,
    GET_DOWN_TONIGHT
    // more as needed
}


public static class CriteriaEvaluator
{
    public static bool EvaluateDefeatedEnemies(ObjectiveCriterion criterion)
    {
        if (DataController.Instance == null) return false;
        List<string> defeatedEnemies = DataController.Instance.ProgressionRuntimeData.Value.GetDefeatedEnemiesList();
        string enemyString = criterion.stringTarget;
        return (defeatedEnemies.Contains(enemyString));
    }

    public static bool EvaluateFinishedObjectives(ObjectiveCriterion criterion)
    {
        if (DataController.Instance == null) return false;
        List<EnumObjective> finishedObjectives = DataController.Instance.ProgressionRuntimeData.Value.GetFinishedObjectivesList();
        EnumObjective targetObj = criterion.finishedObjective;
        return (finishedObjectives.Contains(targetObj));
    }

    public static bool EvaluateDefeatedEnemiesCount(ObjectiveCriterion criterion)
    {
        if (DataController.Instance == null) return false;
        int defeatedEnemies = DataController.Instance.ProgressionRuntimeData.Value.GetDefeatedEnemiesList().Count;
        int targetInt = criterion.intTarget;
        
        switch (criterion.valueComparison)
        {
            case EnumValueComparison.GREATER_OR_EQUAL:
                return defeatedEnemies >= targetInt;
            //...and so on
            default: return false;
        }
    }
}

public class ObjectivesTracker : Singleton<ObjectivesTracker>
{
    [SerializeField] private List<ObjectiveSO> objectives;


    private void Start()
    {
        if (EventRelay.Instance != null)
        {
            EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleProgressionDataUpdate;
        }
    }
    private void OnDestroy()
    {
        if (EventRelay.Instance != null)
        {
            EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= HandleProgressionDataUpdate;
        }
    }

    public void ResetObjectives()
    {
        DataController dc = DataController.Instance;
        dc.ProgressionRuntimeData.Value.ClearObjectivesData();
        foreach (ObjectiveSO objective in objectives)
        {
            switch (objective.DefaultStatus)
            {
                case EnumObjectiveStatus.NOT_STARTED:
                    dc.ProgressionRuntimeData.Value.InitializeObjective(objective.ObjectiveID, EnumObjectiveStatus.NOT_STARTED);
                    break;
                case EnumObjectiveStatus.STARTED:
                    dc.ProgressionRuntimeData.Value.InitializeObjective(objective.ObjectiveID, EnumObjectiveStatus.STARTED);
                    break;
                case EnumObjectiveStatus.FINISHED:
                    dc.ProgressionRuntimeData.Value.InitializeObjective(objective.ObjectiveID, EnumObjectiveStatus.FINISHED);
                    break;
            }
        }
    }

    private void HandleProgressionDataUpdate(IRuntimeData data)
    {
        if (!(data is ProgressionData)) return;
        ProgressionData progData = data as ProgressionData;
        GroomObjectives(progData);
        /*
        if (progData.GetNotStartedObjectivesList() != null)
        {
            

            PromoteNewFinishedObjectives(progData);
            PromoteNewStartedObjectives(progData);
        }
        */
    }

    private void GroomObjectives(ProgressionData progData)
    {
        if (progData.GetStartedObjectivesList() != null)
        {
            List<ObjectiveSO> startedObjSOs = GetObjectiveSOsByStatus(EnumObjectiveStatus.STARTED);
            if (startedObjSOs.Count > 0)
            {
                foreach(ObjectiveSO startedObjSO in startedObjSOs)
                {
                    bool isMet = IsCriteriaMet(startedObjSO.CompletionCriteria);
                    if (isMet) { progData.FinishObjective(startedObjSO.ObjectiveID); }
                }
            }
        }
        if (progData.GetNotStartedObjectivesList() != null)
        {
            List<ObjectiveSO> notStartedObjSOs = GetObjectiveSOsByStatus(EnumObjectiveStatus.NOT_STARTED);
            if (notStartedObjSOs.Count > 0)
            {
                foreach(ObjectiveSO notStartedObjSO in notStartedObjSOs)
                {
                    bool isMet = IsCriteriaMet(notStartedObjSO.EntryCriteria);
                    if (isMet) { progData.StartObjective(notStartedObjSO.ObjectiveID); }
                }
            }
        }
        
    }

    private List<ObjectiveSO> GetObjectiveSOsByStatus(EnumObjectiveStatus status)
    {
        
        List<ObjectiveSO> objList = new List<ObjectiveSO>();
        if (DataController.Instance == null) return objList;

        List<EnumObjective> compareList = new List<EnumObjective>();

        switch (status)
        {
            case EnumObjectiveStatus.NOT_STARTED:
                compareList = DataController.Instance.ProgressionRuntimeData.Value.GetNotStartedObjectivesList();
                break;
            case EnumObjectiveStatus.STARTED:
                compareList = DataController.Instance.ProgressionRuntimeData.Value.GetStartedObjectivesList();
                break;
            case EnumObjectiveStatus.FINISHED:
                compareList = DataController.Instance.ProgressionRuntimeData.Value.GetFinishedObjectivesList();
                break;
        }
        if (compareList.Count <= 0) return objList;
        foreach (ObjectiveSO obj in objectives)
        {
            if ( (compareList.Contains(obj.ObjectiveID)) && (!objList.Contains(obj))) { objList.Add(obj); }
        }

        return objList;
    }

    /*
    private void PromoteNewFinishedObjectives(ProgressionData progData)
    {
        foreach (ObjectiveSO obj in objectives)
        {
            if (progData.GetStartedObjectivesList().Contains(obj.ObjectiveID))
            {
                bool isMet = IsCriteriaMet(obj.CompletionCriteria);
                if (isMet) { progData.StartObjective(obj.ObjectiveID); }
            }
        }
    }

    private void PromoteNewStartedObjectives(ProgressionData progData)
    {
        foreach (ObjectiveSO obj in objectives)
        {
            if (progData.GetNotStartedObjectivesList().Contains(obj.ObjectiveID))
            {
                bool isMet = IsCriteriaMet(obj.EntryCriteria);
                if (isMet) { progData.StartObjective(obj.ObjectiveID); }
            }
        }
    }
    */
    

    private bool IsCriteriaMet(List<ObjectiveCriterion> completionCriteria)
    {

        bool isMet = true;

        foreach(ObjectiveCriterion criterion in completionCriteria)
        {
            EnumProgressionField field = criterion.progressionField;
            EnumProgressionFieldType fieldType = criterion.progressionFieldType;
            EnumValueComparison comparison = criterion.valueComparison;

            switch (field)
            {
                case EnumProgressionField.FINISHED_OBJECTIVES:
                    if (!CriteriaEvaluator.EvaluateFinishedObjectives(criterion)) { return false; }
                    break;
                case EnumProgressionField.DEFEATED_ENEMIES:
                    if (!CriteriaEvaluator.EvaluateDefeatedEnemies(criterion)) { return false; }
                    break;
                case EnumProgressionField.DEFEATED_ENEMIES_COUNT:
                    if (!CriteriaEvaluator.EvaluateDefeatedEnemiesCount(criterion)) { return false; }
                    break;
            }
        }
        return true;
    }

    public ObjectiveSO? GetObjectiveSOByID(EnumObjective objID)
    {
        foreach(ObjectiveSO obj in objectives)
        {
            if (obj.ObjectiveID == objID) { return obj; }
        }

        return null;
    }

    /*
    private bool GetIsCriteriaListMet(List<ObjectiveCriterion> criteriaList)
    {
        bool isMet = true;



        return isMet;
    }
    */

}
