using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Scratch : MonoBehaviour
{
    [SerializeField] private Button killAliceButton;
    [SerializeField] private Button killBobButton;
    [SerializeField] private Button killCharlieButton;
    [SerializeField] private Transform startedObjectives;
    [SerializeField] private Transform finishedObjectives;
    [SerializeField] private GameObject uiPrefab;


   private enum EnumFruit
    {
        APPLE,
        ORANGE,
        BANANA
    }

    private Dictionary<EnumFruit, int> fruitDict = new Dictionary<EnumFruit, int>();


    private void Start()
    {
        //InitializeFruitDict();

        if (DataController.Instance == null) return;
        ProgressionData progData = DataController.Instance.ProgressionRuntimeData.Value;

        killAliceButton.gameObject.SetActive(!progData.GetDefeatedEnemiesList().Contains("ALICE"));
        killBobButton.gameObject.SetActive(progData.GetStartedObjectivesList().Contains(EnumObjective.DEFEAT_BOB));
        killCharlieButton.gameObject.SetActive(false);

        RefreshUI(progData);
    }

    private void OnEnable()
    {
        killAliceButton.onClick.AddListener(KillAlice);
        killBobButton.onClick.AddListener(KillBob);
        killCharlieButton.onClick.AddListener(KillCharlie);
        if (EventRelay.Instance != null)
        {
            EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleProgressionUpdate;
        }
        
    }
    private void OnDisable()
    {
        killAliceButton.onClick.RemoveAllListeners();
        killBobButton.onClick.RemoveAllListeners();
        killCharlieButton.onClick.RemoveAllListeners();
        if (EventRelay.Instance != null)
        {
            EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= HandleProgressionUpdate;
        }
        
    }

    private void HandleProgressionUpdate(IRuntimeData data)
    {
        if (DataController.Instance == null) return;
        DataController dc = DataController.Instance;

        if (!(data is ProgressionData)) return;
        ProgressionData progData = data as ProgressionData;

        /*
        foreach(EnumObjective obj in dc.ProgressionRuntimeData.Value.GetNotStartedObjectivesList())
        {
            Debug.Log($"Not Started: {obj.ToString()}");
        }
        foreach (EnumObjective obj in dc.ProgressionRuntimeData.Value.GetStartedObjectivesList())
        {
            Debug.Log($"Started: {obj.ToString()}");
        }
        foreach (EnumObjective obj in dc.ProgressionRuntimeData.Value.GetFinishedObjectivesList())
        {
            Debug.Log($"Finished: {obj.ToString()}");
        }
        */

        RefreshUI(progData);

        killAliceButton.gameObject.SetActive(!progData.GetDefeatedEnemiesList().Contains("ALICE"));
        killBobButton.gameObject.SetActive(progData.GetStartedObjectivesList().Contains(EnumObjective.DEFEAT_BOB));

        bool showKillCharlie = (progData.GetFinishedObjectivesList().Contains(EnumObjective.DEFEAT_ALICE)) && (!progData.GetDefeatedEnemiesList().Contains("CHARLIE"));
        killCharlieButton.gameObject.SetActive(showKillCharlie);
    }

    private void KillAlice()
    {
        if (DataController.Instance == null) return;
        DataController.Instance.ProgressionRuntimeData.Value.DefeatEnemy("ALICE");
    }
    private void KillBob()
    {
        if (DataController.Instance == null) return;
        DataController.Instance.ProgressionRuntimeData.Value.DefeatEnemy("BOB");
    }
    private void KillCharlie()
    {
        if (DataController.Instance == null) return;
        DataController.Instance.ProgressionRuntimeData.Value.DefeatEnemy("CHARLIE");
    }

    private void InitializeFruitDict()
    {
        /*
        foreach(EnumFruit fruit in System.Enum.GetValues(typeof(EnumFruit)))
        {
            fruitDict[fruit] = 0;
        }

        Debug.Log(fruitDict[EnumFruit.APPLE].ToString());
        Debug.Log(fruitDict[EnumFruit.ORANGE].ToString());
        Debug.Log(fruitDict[EnumFruit.BANANA].ToString());
        */

        foreach (string fruitName in System.Enum.GetNames(typeof(EnumFruit)))
        {
            Debug.Log(fruitName);
        }

    }

    private void RefreshUI(ProgressionData progData)
    {
        if (ObjectivesTracker.Instance == null) return;
        ClearChildren(startedObjectives);
        ClearChildren(finishedObjectives);

        List<EnumObjective> startedIDs = progData.GetStartedObjectivesList();
        foreach (EnumObjective id in startedIDs)
        {
            ObjectiveSO? so = ObjectivesTracker.Instance.GetObjectiveSOByID(id);
            if (so != null)
            {
                string titleStr = so.ObjectiveTitle;
                string descStr = so.ObjectiveDescription;
                ObjectiveUIElement ui = Instantiate(uiPrefab, startedObjectives).GetComponent<ObjectiveUIElement>();
                ui.Configure(titleStr, descStr);
            }
        }

        List<EnumObjective> finishedIDs = progData.GetFinishedObjectivesList();
        foreach(EnumObjective id in finishedIDs)
        {
            ObjectiveSO? so = ObjectivesTracker.Instance.GetObjectiveSOByID(id);
            if (so != null)
            {
                string titleStr = so.ObjectiveTitle;
                string descStr = so.ObjectiveDescription;
                ObjectiveUIElement ui = Instantiate(uiPrefab, finishedObjectives).GetComponent<ObjectiveUIElement>();
                ui.Configure(titleStr, descStr, true);
            }
        }

    }

    private void ClearChildren(Transform parentXform)
    {
        int count = parentXform.childCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(parentXform.GetChild(i).gameObject);
        }
    }
}
