using UnityEngine;
using System.IO;
using System.Collections.Generic;

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
public class PlayerData
// All the save-worthy player information
{
    // TODO
}
[System.Serializable]
public class ProgressionData
// All the save-worthy progression information
{
    // TODO
}
[System.Serializable]
public class InventoryData
// all the save-worthy inventory information
{
    // TODO
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
