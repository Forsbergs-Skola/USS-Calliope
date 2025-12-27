using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Scratch : MonoBehaviour
{


    [SerializeField] private Button killAliceButton;
    [SerializeField] private Button killBobButton;
    [SerializeField] private Button killCharlieButton;
    [SerializeField] private Button haveFuneralButton;

    [SerializeField] private string killAliceObjectiveID;
    [SerializeField] private string killBobObjectiveID;
    [SerializeField] private string haveFuneralObjectiveID;


    private void OnEnable()
    {
        killAliceButton.onClick.AddListener(KillAlice);
        killBobButton.onClick.AddListener(KillBob);
        killCharlieButton.onClick.AddListener(KillCharlie);
        haveFuneralButton.onClick.AddListener(HaveFuneral);
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += IngestDataUpdate;
    }
    private void OnDisable()
    {
        killAliceButton.onClick.RemoveAllListeners();
        killBobButton.onClick.RemoveAllListeners();
        killCharlieButton.onClick.RemoveAllListeners();
        haveFuneralButton.onClick.RemoveAllListeners();
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= IngestDataUpdate;
    }
    private void Start()
    {
        FixButtons(DataController.Instance.ProgressionRuntimeData.Value);
    }

    private void KillAlice()
    {
        ProgressionData progData = GetCurrentProgressionData();
        progData.DefeatEnemy("Alice");
    }
    private void KillBob()
    {
        ProgressionData progData = GetCurrentProgressionData();
        progData.DefeatEnemy("Bob");
    }
    private void KillCharlie()
    {
        ProgressionData progData = GetCurrentProgressionData();
        progData.DefeatEnemy("Charlie");
    }
    private void HaveFuneral()
    {
        ProgressionData progData = GetCurrentProgressionData();
        progData.AliceAndBobFuneralHeld = true;
    }

    private ProgressionData GetCurrentProgressionData()
    {
        return DataController.Instance.ProgressionRuntimeData.Value;
    }

    private void IngestDataUpdate(IRuntimeData _data)
    {
        if (!(_data is ProgressionData)) return;
        FixButtons(_data as ProgressionData);
    }
    private void FixButtons(ProgressionData progData)
    {
        killAliceButton.gameObject.SetActive(progData.ObjectivesAndStatusesDict[killAliceObjectiveID] == EnumObjectiveStatus.STARTED);
        killBobButton.gameObject.SetActive(progData.ObjectivesAndStatusesDict[killBobObjectiveID] == EnumObjectiveStatus.STARTED);
        killCharlieButton.gameObject.SetActive(!progData.GetDefeatedEnemiesList().Contains("Charlie"));
        haveFuneralButton.gameObject.SetActive(progData.ObjectivesAndStatusesDict[haveFuneralObjectiveID] == EnumObjectiveStatus.STARTED);
    }



}
