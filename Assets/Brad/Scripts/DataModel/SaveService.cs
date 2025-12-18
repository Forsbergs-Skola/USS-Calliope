using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;


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
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.TriggerEvent(data);
    }

    public static StringListWrapper GetWrapperizedStringList(List<string> inList)
    {
        StringListWrapper wrappedStrings = new StringListWrapper();
        wrappedStrings.strings = new List<string>(inList);
        return wrappedStrings;
    }

    public static List<string> GetStringListFromJson(string inString)
    {
        List<string> outList = new List<string>();
        if (string.IsNullOrEmpty(inString)) { return outList; }
        StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(inString);
        outList = wrapper.strings;
        return outList;
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
        GameData gData = new GameData();
        gData.playerData = playerData;
        gData.inventoryData = inventoryData;
        gData.progressionData = progressionData;
        SaveData newSD = GameDataToSaveData(gData);
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

    public static void ClearSave()
    {
        if (File.Exists(saveFilePath)) { File.Delete(saveFilePath); }
    }

    //////////////////////
    // Conversion Tools //
    //////////////////////

    private static SaveData GameDataToSaveData(GameData gameData)
    {

        PlayerData playerData = gameData.playerData;
        InventoryData inventoryData = gameData.inventoryData;
        ProgressionData progressionData = gameData.progressionData;
        SaveData outData = new SaveData();

        ////////////////
        // PlayerData //
        ////////////////
        
        // wrapperize lists
        List<string> statusEffects = new List<string>();
        foreach(EnumPlayerStatusEffect effect in playerData.GetActiveStatusEffects())
        {
            string effectStr = effect.ToString();
            if (!statusEffects.Contains(effectStr)) { statusEffects.Add(effectStr); }
        }
        StringListWrapper statusEffectsWrapper = DataTools.GetWrapperizedStringList(statusEffects);
        string activeStatusEffectsString = JsonUtility.ToJson(statusEffectsWrapper);

        // write data to outData
        outData.PLAYER_ActiveStatusEffectsString = activeStatusEffectsString;
        outData.PLAYER_Health = playerData.Health;
        outData.PLAYER_XP = playerData.XP;
        outData.PLAYER_Stamina = playerData.Stamina;
        outData.PLAYER_EquippedWeapon = playerData.EquippedWeapon.ToString();


        ///////////////////
        // InventoryData //
        ///////////////////
        
        // wrapperize lists
        // TODO...

        // write to outData
        // TODO...


        /////////////////////
        // ProgressionData //
        /////////////////////

        // wrapperize lists
        List<string> defeatedEnemies = new List<string>();
        foreach(string enemy in progressionData.GetDefeatedEnemies())
        {
            if (!defeatedEnemies.Contains(enemy)) { defeatedEnemies.Add(enemy); }
        }
        StringListWrapper defeatedEnemiesWrapper = DataTools.GetWrapperizedStringList(defeatedEnemies);
        string defeatedEnemiesString = JsonUtility.ToJson(defeatedEnemiesWrapper);

        // write to outData
        outData.PROGRESSION_defeatedEnemiesString = defeatedEnemiesString;
        outData.PROGRESSION_sceneName = progressionData.SceneName;
        outData.PROGRESSION_TalkedToBob = progressionData.TalkedToBob;
        outData.PROGRESSION_TalkedToAlice = progressionData.TalkedToAlice;


        // return outData
        return outData;
    }
    
    private static PlayerData GetPlayerDataFromSaveData(SaveData saveData)
    {
        
        PlayerData _playerData = new PlayerData(true); // use a sandbox instance to construct

        // extract status effect list data
        List<EnumPlayerStatusEffect> activeStatusEffects = new List<EnumPlayerStatusEffect>();
        List<string> statusEffectsStringList = DataTools.GetStringListFromJson(saveData.PLAYER_ActiveStatusEffectsString);
        foreach (string effectString in statusEffectsStringList)
        {
            if (Enum.TryParse(effectString, ignoreCase: true, out EnumPlayerStatusEffect theEffect))
            {
                activeStatusEffects.Add(theEffect);
            }
        }
        foreach (EnumPlayerStatusEffect effect in activeStatusEffects)
        {
            _playerData.AddActiveStatusEffect(effect);
        }

        // parse the equpiied weapon string
        EnumWeaponType equippedWeapon = EnumWeaponType.NONE;

        // convert equipped weapon string to EnumWeaponType
        if (Enum.TryParse(saveData.PLAYER_EquippedWeapon, ignoreCase: true, out EnumWeaponType theWeapon))
        {
            equippedWeapon = theWeapon;
        }
        _playerData.EquippedWeapon = equippedWeapon;

        // do the easy stuff
        _playerData.Health = saveData.PLAYER_Health;
        _playerData.XP = saveData.PLAYER_XP;
        _playerData.Stamina = saveData.PLAYER_Stamina;

        return new PlayerData(_playerData);
    }
    private static InventoryData GetInventoryDataFromSaveData(SaveData saveData)
    {
        InventoryData _inventoryData = new InventoryData(true);

        // TODO extract the data...
        // create list of items from saveData.INVENTORY_itemsString & add it to _inventoryData
        // create a dictionary of resources from INVENTORY_resourcesString & add it to _inventoryData

        return new InventoryData(_inventoryData);
    }
    private static ProgressionData GetProgressionDataFromSaveData(SaveData saveData)
    {
        ProgressionData _progressionData = new ProgressionData(true);

        // construct defeated enemies list
        List<string> savedDefeatedEnemies = DataTools.GetStringListFromJson(saveData.PROGRESSION_defeatedEnemiesString);
        foreach(string enemy in savedDefeatedEnemies)
        {
            _progressionData.AddDefeatedEnemy(enemy);
        }
        _progressionData.SceneName = saveData.PROGRESSION_sceneName;

        return new ProgressionData(_progressionData);
    }

    

}
