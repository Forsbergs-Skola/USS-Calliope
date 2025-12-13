using UnityEngine;

public class DataController : Singleton<DataController>
// the DontDestroyOnLoad and self reference checking is in the parent class
{
    [Header("Runtime Data Assets")]
    [SerializeField] private PlayerRuntimeData playerRuntimeData;
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;
    [SerializeField] private InventoryRuntimeData inventoryRuntimeData;


    public PlayerRuntimeData PlayerRuntimeData { get => playerRuntimeData; }
    public ProgressionRuntimeData ProgressionRuntimeData { get => progressionRuntimeData; }
    public InventoryRuntimeData InventoryRuntimeData { get => inventoryRuntimeData; }

    private void Start()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered += InitializeRuntimeData;
        EventRelay.Instance.GameEvents.SavedGameLoadedEvent.OnEventTriggered += LoadSavedData;
    }
    private void OnDestroy()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered -= InitializeRuntimeData;
        EventRelay.Instance.GameEvents.SavedGameLoadedEvent.OnEventTriggered -= LoadSavedData;
    }

    private void InitializeRuntimeData()
    {

        Debug.Log("Initializing Game Data");

        SaveService.ClearSave();
        WipeData();
        playerRuntimeData.Value = new PlayerData();
        inventoryRuntimeData.Value = new InventoryData();
        progressionRuntimeData.Value = new ProgressionData();
        EventRelay.Instance.GameEvents.DataUpdatedEvent.TriggerEvent();
    }

    private void LoadSavedData(GameData _gameData)
    {
        WipeData();
        playerRuntimeData.Value = _gameData.playerData;
        inventoryRuntimeData.Value = _gameData.inventoryData;
        progressionRuntimeData.Value = _gameData.progressionData;
        EventRelay.Instance.GameEvents.DataUpdatedEvent.TriggerEvent();
    }

    private void WipeData()
    {
        playerRuntimeData.Value = null;
        inventoryRuntimeData.Value = null;
        progressionRuntimeData.Value = null;
    }

}
