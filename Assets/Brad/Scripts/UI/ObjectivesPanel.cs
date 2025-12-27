using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ObjectivesPanel : MonoBehaviour
{
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;
    [SerializeField] private GameObject objectiveUIElementPrefab;
    [SerializeField] private Transform startedObjectivesHolder;
    [SerializeField] private Transform finishedObjectivesHolder;
    private ProgressionData progressionData = null;

    private void OnEnable()
    {
        if (ObjectivesTracker.Instance == null) return;
        FixObjectives();
    }
    private void OnDisable()
    {
        Resources.UnloadUnusedAssets();
    }

    private void FixObjectives()
    {
        progressionData = progressionRuntimeData.Value;
        if (progressionData == null) return;

        ClearChildren(startedObjectivesHolder);
        ClearChildren(finishedObjectivesHolder);
        
        List<string> startedObjectiveIDs = new List<string>();
        List<string> finishedObjectiveIDs = new List<string>();
        Dictionary<string, EnumObjectiveStatus> statusDict = new Dictionary<string, EnumObjectiveStatus>(progressionData.ObjectivesAndStatusesDict);
        foreach(string _id in statusDict.Keys.ToList<string>())
        {
            if (statusDict[_id] == EnumObjectiveStatus.STARTED) { startedObjectiveIDs.Add(_id); }
            if (statusDict[_id] == EnumObjectiveStatus.FINISHED) { finishedObjectiveIDs.Add(_id); }
        }

        foreach(string objID in startedObjectiveIDs)
        {
            ObjectiveSO obj = ObjectivesTracker.Instance.GetObjectiveWithID(objID);
            string title = obj.ObjectiveTitle;
            string desc = obj.ObjectiveDescription;
            GameObject uiObj = Instantiate(objectiveUIElementPrefab, startedObjectivesHolder);
            ObjectiveUIElement uiElement = uiObj.GetComponent<ObjectiveUIElement>();
            uiElement.Configure(title, desc, false);
        }

        foreach (string objID in finishedObjectiveIDs)
        {
            ObjectiveSO obj = ObjectivesTracker.Instance.GetObjectiveWithID(objID);
            string title = obj.ObjectiveTitle;
            string desc = obj.ObjectiveDescription;
            GameObject uiObj = Instantiate(objectiveUIElementPrefab, finishedObjectivesHolder);
            ObjectiveUIElement uiElement = uiObj.GetComponent<ObjectiveUIElement>();
            uiElement.Configure(title, desc, true);
        }
    }

    private void ClearChildren(Transform parentXform)
    {
        foreach(Transform child in parentXform)
        {
            Destroy(child.gameObject);
        }
    }


}
