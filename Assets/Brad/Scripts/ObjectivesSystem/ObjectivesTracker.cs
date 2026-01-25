using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Events;

public enum EnumObjectiveStatus
{
    NOT_STARTED,
    STARTED,
    FINISHED
}

public class ObjectivesTracker : Singleton<ObjectivesTracker>
{

    [SerializeField] private ObjectivesCatalogSO objectivesCatalog;
    [SerializeField] private IRuntimeDataPayloadEvent runtimeDataUpdatedEvent;
    [SerializeField] private bool devMode = true;


    private void OnEnable()
    {
        runtimeDataUpdatedEvent.OnEventTriggered += IngestRuntimeDataUpdate;
    }
    private void OnDisable()
    {
        runtimeDataUpdatedEvent.OnEventTriggered -= IngestRuntimeDataUpdate;
    }

    public void ResetObjectives()
    {
        ProgressionData progressionData = DataController.Instance.ProgressionRuntimeData.Value;
        Dictionary<string, EnumObjectiveStatus> newStatusDict = new Dictionary<string, EnumObjectiveStatus>();

        foreach(ObjectiveSO objectiveData in objectivesCatalog.Objectives)
        {
            

            string objID = objectiveData.ObjectiveID;
            EnumObjectiveStatus defaultStatus = objectiveData.DefaultStatus;
            if (!newStatusDict.Keys.ToList<string>().Contains(objID))
            {
                //Debug.Log($"Adding {objectiveData.ObjectiveTitle}");
                newStatusDict[objID] = defaultStatus;
            }
        }
        progressionData.UpdateObjectivesAndStatuses(newStatusDict);
    }
    public ObjectiveSO GetObjectiveWithID(string objID)
    {
        return objectivesCatalog.Objectives.Find(obj => obj.ObjectiveID == objID);
    }

    public void StartObjectiveWithID(string objID)
    {
        ProgressionData progressionData = DataController.Instance.ProgressionRuntimeData.Value;
        Dictionary<string, EnumObjectiveStatus> statusDict = new Dictionary<string, EnumObjectiveStatus>(progressionData.ObjectivesAndStatusesDict);
        if (!statusDict.Keys.ToList<string>().Contains(objID)) { Debug.LogError($"Invalid objective id: {objID}"); return; }
        if (statusDict[objID] != EnumObjectiveStatus.NOT_STARTED) { Debug.LogWarning($"{objID} must be in the NOT_STARTED state to start"); return; }

        statusDict[objID] = EnumObjectiveStatus.STARTED;
        progressionData.UpdateObjectivesAndStatuses(statusDict);
    }
    public void FinishObjectiveWithID(string objID)
    {
        ProgressionData progressionData = DataController.Instance.ProgressionRuntimeData.Value;
        Dictionary<string, EnumObjectiveStatus> statusDict = new Dictionary<string, EnumObjectiveStatus>(progressionData.ObjectivesAndStatusesDict);
        if (!statusDict.Keys.ToList<string>().Contains(objID)) { Debug.LogError($"Invalid objective id: {objID}"); return; }
        if (statusDict[objID] != EnumObjectiveStatus.STARTED) { Debug.LogWarning($"{objID} must be in the STARTED state to finish"); return; }

        statusDict[objID] = EnumObjectiveStatus.FINISHED;
        progressionData.UpdateObjectivesAndStatuses(statusDict);
        GroomObjectives(objID);
    }

    private void GroomObjectives(string finishedObjectiveID)
    {
        ProgressionData progressionData = DataController.Instance.ProgressionRuntimeData.Value;
        Dictionary<string, EnumObjectiveStatus> statusDict = new Dictionary<string, EnumObjectiveStatus>(progressionData.ObjectivesAndStatusesDict);
        List<string> objIDs = statusDict.Keys.ToList<string>();
        List<ObjectiveSO> objectivesToEvaluate = new List<ObjectiveSO>();

        foreach(string _id in objIDs)
        {
            if (_id != finishedObjectiveID && statusDict[_id] == EnumObjectiveStatus.NOT_STARTED)
            {
                objectivesToEvaluate.Add(GetObjectiveWithID(_id));
            }
        }
        foreach(ObjectiveSO objective in objectivesToEvaluate)
        {
            bool goodToStart = true;
            List<string> prerequisiteIDs = objective.PrerequisiteObjectiveIDs;
            foreach(string prereqID in prerequisiteIDs)
            {
                if (statusDict[prereqID] != EnumObjectiveStatus.FINISHED) { goodToStart = false; break; }
            }
            if (goodToStart) { StartObjectiveWithID(objective.ObjectiveID); }
        }
    }

    private void IngestRuntimeDataUpdate(IRuntimeData _data)
    {
        ProgressionData progressionData = DataController.Instance.ProgressionRuntimeData.Value;
        if (progressionData == null) return;

        if (progressionData.ObjectivesAndStatusesDict == null) return;

        List<ObjectiveSO> startedObjectives = new List<ObjectiveSO>();
        Dictionary<string, EnumObjectiveStatus> statusDict = new Dictionary<string, EnumObjectiveStatus>(progressionData.ObjectivesAndStatusesDict);
        foreach (string objID in statusDict.Keys.ToList<string>())
        {
            if (statusDict[objID] == EnumObjectiveStatus.STARTED) { startedObjectives.Add(GetObjectiveWithID(objID)); }
        }

        if (startedObjectives.Count > 0)
        {
            foreach (ObjectiveSO startedObj in startedObjectives)
            {
                CompletionCriteriaSO criteria = startedObj.CompletionCriteria;
                if (criteria.IsCriteriaMet())
                {
                    //Debug.Log($"{startedObj.ObjectiveTitle} is complete!!!");
                    FinishObjectiveWithID(startedObj.ObjectiveID);
                }
            }
        }
        if (devMode) { DebugIncomingData(progressionData); }
        else { /*TODO: UI stuff...*/ }
    }


    private void DebugIncomingData(ProgressionData progData)
    {
        Dictionary<string, EnumObjectiveStatus> statusDict = new Dictionary<string, EnumObjectiveStatus>(progData.ObjectivesAndStatusesDict);
        foreach(string objID in statusDict.Keys.ToList<string>())
        {
            ObjectiveSO thisObj = GetObjectiveWithID(objID);
            Debug.Log($"{thisObj.ObjectiveTitle}: {statusDict[objID].ToString()}");
        }
    }


}
