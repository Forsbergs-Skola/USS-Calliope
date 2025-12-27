using UnityEngine;
using UnityEngine.UI;
using Events;
using System.Collections.Generic;

public class WalkThrough : MonoBehaviour
{

    [SerializeField] private Button flashlightButton;
    [SerializeField] private Button discoverCentralCorridorButton;
    [SerializeField] private Button crewQuartersKeyButton;





    private Dictionary<string, EnumObjectiveStatus> statusDict
    {
        get => new Dictionary<string, EnumObjectiveStatus>(DataController.Instance.ProgressionRuntimeData.Value.ObjectivesAndStatusesDict);
    }
    private ProgressionData progData
    {
        get => DataController.Instance.ProgressionRuntimeData.Value;
    }
    private InventoryData invData = DataController.Instance.InventoryRuntimeData.Value;


    private void Awake()
    {
        if (DataController.Instance == null) Destroy(gameObject);
    }

    private void OnEnable()
    {
        EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered += FixButtons;


        flashlightButton.onClick.AddListener(GetFlashlight);
        discoverCentralCorridorButton.onClick.AddListener(EnterCentralCorridor);
        crewQuartersKeyButton.onClick.AddListener(GetCrewQuartersKey);
        
        FixButtons();
    }
    private void OnDisable()
    {
        EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered -= FixButtons;


        flashlightButton.onClick.RemoveAllListeners();
        discoverCentralCorridorButton.onClick.RemoveAllListeners();
        crewQuartersKeyButton.onClick.RemoveAllListeners();
    }






    private void GetFlashlight()
    {
       invData.AddQuestItem(IDConstants.FLASHLIGHT_INV_ID);
    }
    private void EnterCentralCorridor()
    {
        progData.CentralCorridorDiscovered = true;
    }
    private void GetCrewQuartersKey()
    {
        invData.AddQuestItem(IDConstants.CREW_QUARTERS_KEY);
    }


    private void FixButtons()
    {
        flashlightButton.gameObject.SetActive(statusDict[IDConstants.OBJECTIVE_01_ID] == EnumObjectiveStatus.STARTED);
        discoverCentralCorridorButton.gameObject.SetActive(statusDict[IDConstants.OBJECTIVE_02_ID] == EnumObjectiveStatus.STARTED);
        crewQuartersKeyButton.gameObject.SetActive(statusDict[IDConstants.OBJECTIVE_03_ID] == EnumObjectiveStatus.STARTED);
    }

    /*
    private void CompleteQuest(string questID)
    {
        Dictionary<string, EnumObjectiveStatus> newDict = new Dictionary<string, EnumObjectiveStatus>(statusDict);
        if (newDict[questID] != EnumObjectiveStatus.STARTED) { Debug.LogError($"{questID} not started!"); }
        newDict[questID] = EnumObjectiveStatus.FINISHED;
        progData.UpdateObjectivesAndStatuses(newDict);
    }
    */





}
