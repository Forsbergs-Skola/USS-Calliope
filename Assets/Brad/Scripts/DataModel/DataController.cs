using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.sceneLoaded += HandleOnSceneLoaded;
    }
    private void OnDestroy()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered -= InitializeRuntimeData;
        EventRelay.Instance.GameEvents.SavedGameLoadedEvent.OnEventTriggered -= LoadSavedData;
        SceneManager.sceneLoaded -= HandleOnSceneLoaded;
    }

    private void InitializeRuntimeData()
    {
        if (ObjectivesTracker.Instance == null) return;

        Debug.Log("Initializing Game Data");


        SaveService.ClearSave();
        WipeData();
        playerRuntimeData.Value = new PlayerData();
        inventoryRuntimeData.Value = new InventoryData();
        progressionRuntimeData.Value = new ProgressionData();
        ObjectivesTracker.Instance.ResetObjectives();
        EventRelay.Instance.GameEvents.DataUpdatedEvent.TriggerEvent();
    }

    private void LoadSavedData(GameData _gameData)
    {
        WipeData();
        playerRuntimeData.Value = _gameData.playerData;
        inventoryRuntimeData.Value = _gameData.inventoryData;
        progressionRuntimeData.Value = _gameData.progressionData;
        EventRelay.Instance.GameEvents.DataUpdatedEvent.TriggerEvent();

        Debug.Log(progressionRuntimeData.Value.SceneName);
        SceneManager.LoadScene(progressionRuntimeData.Value.SceneName);


    }

    private void WipeData()
    {
        playerRuntimeData.Value = null;
        inventoryRuntimeData.Value = null;
        progressionRuntimeData.Value = null;
    }

    private void HandleOnSceneLoaded(Scene _scene, LoadSceneMode _loadMode)
    {
        if (_scene.name == "Bootstrap") { return; }
        progressionRuntimeData.Value.SceneName = _scene.name;
    }

}
