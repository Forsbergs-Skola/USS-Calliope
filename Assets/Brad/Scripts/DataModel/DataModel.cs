using UnityEngine;
using System.IO;
using System.Collections.Generic;


public enum EnumPlayerStatusEffect
{
    BLEEDING,
    POISON,
    IN_STEALTH
    // add/remove more as needed
}

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

    /////////////////
    // Data Fields //
    /////////////////

    // LIST FIELDS

    private List<EnumPlayerStatusEffect> _activeStatusEffects;
    public List<EnumPlayerStatusEffect> GetActiveStatusEffects()
    {
        return new List<EnumPlayerStatusEffect>(_activeStatusEffects);
    }
    public void AddActiveStatusEffect(EnumPlayerStatusEffect _effect)
    {
        if (_activeStatusEffects.Contains(_effect)) return;
        _activeStatusEffects.Add(_effect);
        DataTools.HandleOnDataChanged(this);
    }
    public void RemoveActiveStatusEffect(EnumPlayerStatusEffect _effect)
    {
        if (!_activeStatusEffects.Contains(_effect)) return;
        _activeStatusEffects.Remove(_effect);
        DataTools.HandleOnDataChanged(this);
    }
    public void ClearAllActiveStatusEffects()
    {
        _activeStatusEffects.Clear();
        _activeStatusEffects = new List<EnumPlayerStatusEffect>();
        DataTools.HandleOnDataChanged(this);
    }

    // ATOMIC FIELDS

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

    // TODO... Other data fields

    //////////////////
    // Constructors //
    //////////////////
    public PlayerData()
    {
        IsSandbox = false;
        Health = Constants.MAX_PLAYER_HEALTH;
        _activeStatusEffects = new List<EnumPlayerStatusEffect>();
    }
    public PlayerData(bool isSandbox)
    {
        IsSandbox = isSandbox;
        Health = Constants.MAX_PLAYER_HEALTH;
        _activeStatusEffects = new List<EnumPlayerStatusEffect>();
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
[System.Serializable]
public class ProgressionData : IRuntimeData
// All the save-worthy progression information
{
    public bool IsSandbox { get; private set; }

    /////////////////
    // Data Fields //
    /////////////////

    // TODO all the progression data fields

    // list fields
    private List<string> _defeatedEnemies;
    public void AddDefeatedEnemy(string enemyID)
    {
        if (_defeatedEnemies.Contains(enemyID)) return;
        _defeatedEnemies.Add(enemyID);
        DataTools.HandleOnDataChanged(this);
    }
    public void RemoveDefeatedEnemy(string enemyID)
    {
        if (!_defeatedEnemies.Contains(enemyID)) return;
        _defeatedEnemies.Remove(enemyID);
        DataTools.HandleOnDataChanged(this);
    }
    public void ResetDefeatedEnemies()
    {
        _defeatedEnemies.Clear();
        _defeatedEnemies = new List<string>();
    }
    public List<string> GetDefeatedEnemies()
    {
        return new List<string>(_defeatedEnemies);
    }

    // atomic fields
    private bool _talkedToBob;
    public bool TalkedToBob
    {
        get => _talkedToBob;
        set
        {
            _talkedToBob = value;
            DataTools.HandleOnDataChanged(this);
        }
    }

    private bool _talkedToAlice;
    public bool TalkedToAlice
    {
        get => _talkedToAlice;
        set
        {
            _talkedToAlice = value;
            DataTools.HandleOnDataChanged(this);
        }
    }


    //////////////////
    // Constructors //
    //////////////////
    public ProgressionData()
    {
        IsSandbox = false;
        TalkedToBob = false;
        TalkedToAlice = false;
        _defeatedEnemies = new List<string>();
    }
    public ProgressionData(bool isSandBox)
    {
        IsSandbox = isSandBox;
        TalkedToBob = false;
        TalkedToAlice = false;
        _defeatedEnemies = new List<string>();
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
[System.Serializable]
public class InventoryData : IRuntimeData
// all the save-worthy inventory information
{
    public bool IsSandbox { get; private set; }

    /////////////////
    // Data Fields //
    /////////////////

    // TODO all the inventory data fields

    //////////////////
    // Constructors //
    //////////////////
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

    public static StringListWrapper GetWrapperizedStringList(List<string> inList)
    {
        StringListWrapper wrappedStrings = new StringListWrapper();
        wrappedStrings.strings = new List<string>(inList);
        return wrappedStrings;
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
        ClearSave();
        SaveData newSD = GameDataToSaveData(playerData, inventoryData, progressionData);
        // TODO -- write newSD to disk
    }
    public static void Load()
    {
        SaveData savedData = new SaveData();

        // TODO -- read saved JSON into savedData
        
        GameData gameData = new GameData();
        PlayerData _playerData = GetPlayerDataFromSaveData(savedData);
        InventoryData _inventoryData = GetInventoryDataFromSaveData(savedData);
        ProgressionData _progressionData = GetProgressionDataFromSaveData(savedData);
        gameData.playerData = _playerData;
        gameData.inventoryData = _inventoryData;
        gameData.progressionData = _progressionData;
        EventRelay.Instance.GameEvents.SavedGameLoadedEvent.TriggerEvent(gameData);
    }

    //////////////////////
    // Conversion Tools //
    //////////////////////
    
    private static SaveData GameDataToSaveData(PlayerData playerData, InventoryData inventoryData, ProgressionData progressionData)
    {
        SaveData outData = new SaveData();
        // TODO...
        return outData;
    }
    public static void ClearSave()
    {
        if (File.Exists(saveFilePath)) { File.Delete(saveFilePath); }
    }
    private static PlayerData GetPlayerDataFromSaveData(SaveData saveData)
    {
        PlayerData playerData = new PlayerData();
        // TODO...
        return playerData;
    }
    private static InventoryData GetInventoryDataFromSaveData(SaveData saveData)
    {
        InventoryData inventoryData = new InventoryData();
        // TODO...
        return inventoryData;
    }
    private static ProgressionData GetProgressionDataFromSaveData(SaveData saveData)
    {
        ProgressionData progressionData = new ProgressionData();
        // TODO...
        return progressionData;
    }
}
