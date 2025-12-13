using UnityEngine;

public class DataService : Singleton<DataService>
{
    private PlayerData currentPlayerData;
    private InventoryData currentInventoryData;
    private ProgressionData currentProgressionData;

    public PlayerData CurrentPlayerData { get => currentPlayerData; }
    public InventoryData CurrentInventoryData { get => currentInventoryData; }
    public ProgressionData CurrentProgressionData { get => currentProgressionData; }
    public GameData CurrentAggregatedGameData { get => new GameData(currentPlayerData, currentInventoryData, currentProgressionData); }

    private void Start()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered += HandleNewGameStarted;
        EventRelay.Instance.GameEvents.SavedGameLoadedEvent.OnEventTriggered += HandleSavedGameLoaded;
    }
    private void OnDestroy()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered -= HandleNewGameStarted;
        EventRelay.Instance.GameEvents.SavedGameLoadedEvent.OnEventTriggered -= HandleSavedGameLoaded;
    }

    private void HandleNewGameStarted()
    {
        SaveService.ClearSave();
        currentPlayerData = InitializePlayerData();
        currentInventoryData = InitializeInventoryData();
        currentProgressionData = InitializeProgressionData();
        EventRelay.Instance.GameEvents.DataServiceUpdatedEvent.TriggerEvent();
    }

    private void HandleSavedGameLoaded(GameData _gameData)
    {
        currentPlayerData = _gameData.playerData;
        currentInventoryData = _gameData.inventoryData;
        currentProgressionData = _gameData.progressionData;
        EventRelay.Instance.GameEvents.DataServiceUpdatedEvent.TriggerEvent();
    }

    private PlayerData InitializePlayerData()
    {
        PlayerData newPlayerData = new PlayerData();
        return newPlayerData;
    }
    private InventoryData InitializeInventoryData()
    {
        InventoryData newInventoryData = new InventoryData();
        return newInventoryData;
    }
    private ProgressionData InitializeProgressionData()
    {
        ProgressionData newProgressionData = new ProgressionData();
        return newProgressionData;
    }
}
