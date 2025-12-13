using UnityEngine;
using System.IO;
using System.Collections.Generic;


public class Constants
{
    public const int MAX_PLAYER_HEALTH = 100;
}
public interface IRuntimeData
{
    public bool GetIsSandbox();
}

[System.Serializable]
// A utility class for serializing lists to JSON
public class StringListWrapper
{
    public List<string> strings;
}
[System.Serializable] 
public class SaveData
// JSON friendly data store
{
    // TODO
}
[System.Serializable]
public class PlayerData : IRuntimeData
// All the save-worthy player information
{
    public bool IsSandbox { get; private set; }
    private int _health;
    public int Health
    {
        get => _health;
        set
        {
            if (_health == value) return;
            _health = Mathf.Clamp(value, 0, Constants.MAX_PLAYER_HEALTH);
            DataTools.HandleOnDataChanged(this);
        }
    }
    public PlayerData()
    {
        IsSandbox = false;
        Health = Constants.MAX_PLAYER_HEALTH;
    }
    public PlayerData(bool isSandbox)
    {
        IsSandbox = isSandbox;
        Health = Constants.MAX_PLAYER_HEALTH;
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
[System.Serializable]
public class ProgressionData : IRuntimeData
// All the save-worthy progression information
{
    public bool IsSandbox { get; private set; }
    public ProgressionData()
    {
        IsSandbox = false;
    }
    public ProgressionData(bool isSandBox)
    {
        IsSandbox = isSandBox;
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
[System.Serializable]
public class InventoryData : IRuntimeData
// all the save-worthy inventory information
{
    public bool IsSandbox { get; private set; }
    public InventoryData()
    {
        IsSandbox = false;
    }
    public InventoryData(bool isSandbox)
    {
        IsSandbox = isSandbox;
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
[System.Serializable]
public class GameData
{
    public PlayerData playerData;
    public InventoryData inventoryData;
    public ProgressionData progressionData;

    public GameData() { }
    public GameData(PlayerData _playerData, InventoryData _inventoryData, ProgressionData _progressionData)
    {
        playerData = _playerData;
        inventoryData = _inventoryData;
        progressionData = _progressionData;
    }
}

public static class DataTools
{
    public static void HandleOnDataChanged(IRuntimeData data)
    {
        if (data.GetIsSandbox()) return;
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.DataUpdatedEvent.TriggerEvent();
    }
}

public static class SaveService
{
    private static string saveFilePath => Path.Combine(Application.persistentDataPath, "save.json");
    
    /////////
    // API //
    ///////// 
    
    public static bool SaveExists()
    {
        return File.Exists(saveFilePath);
    }
    public static void Save(PlayerData playerData, InventoryData inventoryData, ProgressionData progressionData)
    {
        // TODO
    }
    public static void Load()
    {
        GameData gameData = new GameData();
        // TODO
        EventRelay.Instance.GameEvents.SavedGameLoadedEvent.TriggerEvent(gameData);
    }

    //////////////////////
    // Conversion Tools //
    //////////////////////
    
    private static SaveData GameDataToSaveData(PlayerData playerData, InventoryData inventoryData, ProgressionData progressionData)
    {
        SaveData outData = new SaveData();
        // TODO
        return outData;
    }
    public static void ClearSave()
    {
        if (File.Exists(saveFilePath)) { File.Delete(saveFilePath); }
    }
    private static PlayerData GetPlayerDataFromSaveData(SaveData saveData)
    {
        PlayerData playerData = new PlayerData();
        // TODO
        return playerData;
    }
    private static InventoryData GetInventoryDataFromSaveData(SaveData saveData)
    {
        InventoryData inventoryData = new InventoryData();
        // TODO
        return inventoryData;
    }
    private static ProgressionData GetProgressionDataFromSaveData(SaveData saveData)
    {
        ProgressionData progressionData = new ProgressionData();
        // TODO
        return progressionData;
    }
    private static StringListWrapper GetWrapperizedStringList(List<string> inList)
    {
        StringListWrapper wrappedStrings = new StringListWrapper();
        wrappedStrings.strings = new List<string>(inList);
        return wrappedStrings;
    }
}
