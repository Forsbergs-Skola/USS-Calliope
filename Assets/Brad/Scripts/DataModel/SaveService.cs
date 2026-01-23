using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;


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
        string json = JsonUtility.ToJson(newSD, true);
        File.WriteAllText(saveFilePath, json);
        EventRelay.Instance.GameEvents.GameSavedEvent.TriggerEvent();
    }
    public static void Load()
    {
        if (!SaveExists()) return;
        SaveData savedData = new SaveData();

        // TODO -- read saved JSON into savedData
        string json = File.ReadAllText(saveFilePath);
        savedData = JsonUtility.FromJson<SaveData>(json);
        
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


        float posX = playerData.LastPosition.x;
        float posY = playerData.LastPosition.y;
        float posZ = playerData.LastPosition.z;


        // write data to outData
        outData.PLAYER_ActiveStatusEffectsString = activeStatusEffectsString;
        outData.PLAYER_Health = playerData.Health;
        outData.PLAYER_MaxHealth = playerData.MaxHealth;
        outData.PLAYER_XP = playerData.XP;
        outData.PLAYER_Stamina = playerData.Stamina;
        outData.PLAYER_EquippedWeapon = playerData.EquippedWeapon.ToString();
        outData.PLAYER_PosX = posX;
        outData.PLAYER_PosY = posY;
        outData.PLAYER_PosZ = posZ;
        outData.PLAYER_CurrentAmmo = playerData.PlayerCurrentAmmo;


        ///////////////////
        // InventoryData //
        ///////////////////

        // wrapperize lists
        // TODO...

        List<string> questItemIDs = inventoryData.GetQuestItemIDs();
        StringListWrapper questItemsWrapper = DataTools.GetWrapperizedStringList(questItemIDs);
        string questItemsString = JsonUtility.ToJson(questItemsWrapper);

        List<string> weaponItemIDs = inventoryData.GetWeaponItemIDs();
        StringListWrapper weaponItemsWrapper = DataTools.GetWrapperizedStringList(weaponItemIDs);
        string weaponsString = JsonUtility.ToJson(weaponItemsWrapper);

        List<string> consumableItemIDs = inventoryData.GetConsumableIDsAndQuantities().Keys.ToList<string>();
        List<int> consumableItemValues = inventoryData.GetConsumableIDsAndQuantities().Values.ToList<int>();
        List<string> stringListConsumableItemValues = new();
        foreach(int i in consumableItemValues)
        {
            stringListConsumableItemValues.Add(i.ToString());
        }
        StringListWrapper consumableItemsWrapper = DataTools.GetWrapperizedStringList(consumableItemIDs);
        StringListWrapper consumableValuesWrapper = DataTools.GetWrapperizedStringList(stringListConsumableItemValues);
        string consumablesItemsString = JsonUtility.ToJson(consumableItemsWrapper);
        string consumablesValuesString = JsonUtility.ToJson(consumableValuesWrapper);

        List<string> exhasutedPickups = new List<string>(inventoryData.GetExhaustedPickups());
        StringListWrapper exhaustedPickupsWrapper = DataTools.GetWrapperizedStringList(exhasutedPickups);
        string exhaustedPickupsString = JsonUtility.ToJson(exhaustedPickupsWrapper);

        // write to outData
        // TODO...

        outData.INVENTORY_questItemsString = questItemsString;
        outData.INVENTORY_weaponsString = weaponsString;
        outData.INVENTORY_consumablesItemsString = consumablesItemsString;
        outData.INVENTORY_consumablesValuesString = consumablesValuesString;
        outData.INVENTORY_exhaustedPickups = exhaustedPickupsString;

        /////////////////////
        // ProgressionData //
        /////////////////////

        // Objectives...
        List<string> objectiveIDs = progressionData.ObjectivesAndStatusesDict.Keys.ToList<string>();
        List<string> objStatuses = new();
        foreach (string objID in objectiveIDs)
        {
            EnumObjectiveStatus status = progressionData.ObjectivesAndStatusesDict[objID];
            objStatuses.Add(status.ToString());
        }
        StringListWrapper objectiveIDsWrapper = DataTools.GetWrapperizedStringList(objectiveIDs);
        StringListWrapper statusesWrapper = DataTools.GetWrapperizedStringList(objStatuses);
        string objectiveIDsString = JsonUtility.ToJson(objectiveIDsWrapper);
        string objectiveStatusesString = JsonUtility.ToJson(statusesWrapper);

        // Breakables
        List<string> exhaustedBreakables = new List<string>(progressionData.GetExhaustedBreakablesList());
        StringListWrapper breakablesWrapper = DataTools.GetWrapperizedStringList(exhaustedBreakables);
        string breakablesString = JsonUtility.ToJson(breakablesWrapper);

        // UnlockedTerminalWorldIDs
        List<string> ids = new List<string>(progressionData.GetUnlockedTerminalWorldIDs());
        StringListWrapper terminalsWrapper = DataTools.GetWrapperizedStringList(ids);
        string terminalIdsString = JsonUtility.ToJson(terminalsWrapper);

        // other progression variables
        List<string> defeatedEnemies = new List<string>(progressionData.GetDefeatedEnemiesList());
        StringListWrapper enemiesDefeatedStringWrapper = DataTools.GetWrapperizedStringList(defeatedEnemies);
        string defeatedEnemiesString = JsonUtility.ToJson(enemiesDefeatedStringWrapper);




        // write to outData
        outData.PROGRESSION_ObjectiveIDs = objectiveIDsString;
        outData.PROGRESSION_ObjectiveStatuses = objectiveStatusesString;
        outData.PROGRESSION_ExhaustedBreakables = breakablesString;
        outData.PROGRESSION_EnemiesDefeated = defeatedEnemiesString;
        outData.PROGRESSION_AlicaAndBobFuneralHeld = progressionData.AliceAndBobFuneralHeld;
        outData.PROGRESSION_AlicaAndBobFuneralHeld = progressionData.BobContacted;
        outData.PROGRESSION_CentralCorridorDiscovered = progressionData.CentralCorridorDiscovered;
        outData.PROGRESSION_CrewQuartersUnlocked = progressionData.CrewQuartersUnlocked;
        outData.PROGRESSION_LinaxFound = progressionData.LinaxFound;
        outData.PROGRESSION_BridgeUnlocked = progressionData.BridgeUnlocked;
        outData.PROGRESSION_LiftAccessed = progressionData.LiftAccessed;
        outData.PROGRESSION_SceneName = progressionData.SceneName;
        outData.PROGRESSION_UnlockedTerminalWorldIDs = terminalIdsString;

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
        if (Enum.TryParse(saveData.PLAYER_EquippedWeapon, ignoreCase: true, out EnumWeaponType _theWeapon))
        {
            equippedWeapon = _theWeapon;
        }
        _playerData.EquippedWeapon = equippedWeapon;

        // do the easy stuff
        _playerData.Health = saveData.PLAYER_Health;
        _playerData.MaxHealth = saveData.PLAYER_MaxHealth;
        _playerData.XP = saveData.PLAYER_XP;
        _playerData.Stamina = saveData.PLAYER_Stamina;

        Vector3 lastPos = new Vector3(saveData.PLAYER_PosX, saveData.PLAYER_PosY, saveData.PLAYER_PosZ);
        _playerData.LastPosition = lastPos;

        _playerData.PlayerCurrentAmmo = saveData.PLAYER_CurrentAmmo;

        return new PlayerData(_playerData);
    }
    private static InventoryData GetInventoryDataFromSaveData(SaveData saveData)
    {
        InventoryData _inventoryData = new InventoryData(true);

        // TODO extract the data...
        // create list of items from saveData.INVENTORY_itemsString & add it to _inventoryData
        // create a dictionary of resources from INVENTORY_resourcesString & add it to _inventoryData
        
        List<string> weaponIDs = DataTools.GetStringListFromJson(saveData.INVENTORY_weaponsString);
        List<string> questItemIDs = DataTools.GetStringListFromJson(saveData.INVENTORY_questItemsString);
        List<string> consumablesIDs = DataTools.GetStringListFromJson(saveData.INVENTORY_consumablesItemsString);
        List<string> consumablesValuesStringList = DataTools.GetStringListFromJson(saveData.INVENTORY_consumablesValuesString);
        List<string> exhaustedPickups = DataTools.GetStringListFromJson(saveData.INVENTORY_exhaustedPickups);

        Dictionary<string, int> consumablesDict = new();
        for (int i = 0; i < consumablesIDs.Count; i++)
        {
            string key = consumablesIDs[i];
            if (int.TryParse(consumablesValuesStringList[i], out int _value))
            {
                // _value now holds the integerized value for the current ID
                consumablesDict[key] = _value;
            }
            else
            {
                Debug.LogError("Problem parsing the consumable values in saved data");
            }
        }

        _inventoryData.SetWeaponsList(weaponIDs);
        _inventoryData.SetQuestItemsList(questItemIDs);
        _inventoryData.SetConsumablesDict(consumablesDict);
        _inventoryData.SetExhaustedPickupsList(exhaustedPickups);

        return new InventoryData(_inventoryData);
    }
    private static ProgressionData GetProgressionDataFromSaveData(SaveData saveData)
    {

        ProgressionData _progressionData = new ProgressionData(true);
        
        // objectives

        List<string> idStrings = DataTools.GetStringListFromJson(saveData.PROGRESSION_ObjectiveIDs);
        List<string> statusStrings = DataTools.GetStringListFromJson(saveData.PROGRESSION_ObjectiveStatuses);
        

        Dictionary<string, EnumObjectiveStatus> statusDict = new();
        for (int i = 0; i< idStrings.Count; i++)
        {
            string key = idStrings[i];
            if (Enum.TryParse(statusStrings[i], ignoreCase: true, out EnumObjectiveStatus _status))
            {
                statusDict[key] = _status;
            }
            else
            {
                Debug.LogError("Problem parsing the objective statuses in saved data");
            }
        }
        _progressionData.UpdateObjectivesAndStatuses(statusDict);

        // Breakables
        List<string> breakablesList = DataTools.GetStringListFromJson(saveData.PROGRESSION_ExhaustedBreakables);
        _progressionData.ReplaceDefeatedEnemiesList(breakablesList);

        List<string> terminalsList = DataTools.GetStringListFromJson(saveData.PROGRESSION_UnlockedTerminalWorldIDs);
        _progressionData.ReplaceUnlockedTerminalWorldIDs(terminalsList);

        // other prog variables
        List<string> defeatedEnemiesList = DataTools.GetStringListFromJson(saveData.PROGRESSION_EnemiesDefeated);
        _progressionData.ReplaceDefeatedEnemiesList(defeatedEnemiesList);

        _progressionData.AliceAndBobFuneralHeld = saveData.PROGRESSION_AlicaAndBobFuneralHeld;
        _progressionData.BobContacted = saveData.PROGRESSION_BobContacted;
        _progressionData.CentralCorridorDiscovered = saveData.PROGRESSION_CentralCorridorDiscovered;
        _progressionData.CrewQuartersUnlocked = saveData.PROGRESSION_CrewQuartersUnlocked;
        _progressionData.DataDelivered = saveData.PROGRESSION_DataDelivered;
        _progressionData.LinaxFound = saveData.PROGRESSION_LinaxFound;
        _progressionData.BridgeUnlocked = saveData.PROGRESSION_BridgeUnlocked;
        _progressionData.LiftAccessed = saveData.PROGRESSION_LiftAccessed;
        _progressionData.SceneName = saveData.PROGRESSION_SceneName;

        // return the new progression data
        return new ProgressionData(_progressionData);
        
    }

    

}
