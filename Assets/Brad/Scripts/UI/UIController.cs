using UnityEngine;
using Events;
using UnityEngine.SceneManagement;
public class UIController : Singleton<UIController>
{

    [Header("Event Channels")]
    [SerializeField] private IRuntimeDataPayloadEvent runtimeDataUpdatedEvent;
    [SerializeField] private EmptyPayloadEvent newGamePressedEvent;

    [Header("Prefabs")]
    [SerializeField] private GameObject mainMenuPrefab;
    [SerializeField] private GameObject hudPrefab;

    private void Start()
    {
        // show logo splash
        // then show main menu

        GameObject mainObj = Instantiate(mainMenuPrefab);

    }


    private void OnEnable()
    {
        runtimeDataUpdatedEvent.OnEventTriggered += HandleOnRuntimeDataUpdated;
        newGamePressedEvent.OnEventTriggered += HandleNewGamePressedEvent;

        SceneManager.sceneLoaded += HandleOnSceneLoaded;
    }

    private void OnDisable()
    {
        runtimeDataUpdatedEvent.OnEventTriggered -= HandleOnRuntimeDataUpdated;
        newGamePressedEvent.OnEventTriggered -= HandleNewGamePressedEvent;

        SceneManager.sceneLoaded += HandleOnSceneLoaded;
    }

    

    private void HandleOnRuntimeDataUpdated(IRuntimeData data)
    {
        switch (data)
        {
            case PlayerData:
                IngestNewPlayerData(data as PlayerData);
                break;
            case InventoryData:
                IngestNewInventoryData(data as InventoryData);
                break;
            case ProgressionData:
                IngestNewProgressionData(data as ProgressionData);
                break;
        }
    }

    private void IngestNewPlayerData(PlayerData playerData)
    {
        int health = playerData.Health;
        int xp = playerData.XP;
        Debug.Log($"HEALTH: {health} / XP: {xp}");
        // refresh the HUD
    }
    private void IngestNewInventoryData(InventoryData inventoryData)
    {

    }
    private void IngestNewProgressionData(ProgressionData progressionData)
    {
        //string sceneName = progressionData.SceneName;
        //if (sceneName != null)
        //{
        //    Debug.Log(sceneName);
        //}
    }

    private void HandleNewGamePressedEvent()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.TriggerEvent(); //Data controller initializes game data
        SceneManager.LoadScene(Bootstrapper.Instance.DefaultGameSceneName);
    }

    private void HandleLoadGamePressedEvent()
    {
        SaveService.Load(); // Save service triggers SavedGameLoadedEvent, Datacontroller ingests it
        SceneManager.LoadScene(DataController.Instance.ProgressionRuntimeData.Value.SceneName);
    }

    private void HandleOnSceneLoaded(Scene _scene, LoadSceneMode _loadMode)
    {
        
        if (_scene.name != "Bootstrap")
        {
            GameObject hudObj = Instantiate(hudPrefab);
        }
        
        
    }


}
