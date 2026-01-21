using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class ProgressionData : IRuntimeData
// All the save-worthy progression information
{
    public bool IsSandbox { get; private set; }
    
    ////////////////////
    // Backing Fields //
    ////////////////////

    private string _sceneName;

    private List<string> _defeatedEnemiesList;
    private List<string> _exhaustedBrakables;
    private Dictionary<string, EnumObjectiveStatus> _objectivesAndStatusesDict;
    private List<string> _unlockedTerminalWorldIDs;

    private bool _dataDelivered = false;
    private bool _centralCorridorDiscovered;
    private bool _bobContacted = false;
    private bool _crewQuartersUnlocked = false;
    private bool _aliceAndBobFuneralHeld = false;



   

    ///////////////////
    // Public Access //
    ///////////////////

    public bool DataDelivered
    {
        get => _dataDelivered;
        set
        {
            if (value == _dataDelivered) return;
            _dataDelivered = value;
            DataTools.HandleOnDataChanged(this);
        }
    }
    public bool CentralCorridorDiscovered
    {
        get => _centralCorridorDiscovered;
        set
        {
            if (value == _centralCorridorDiscovered) return;
            _centralCorridorDiscovered = value;
            DataTools.HandleOnDataChanged(this);
        }
    }
    public bool BobContacted
    {
        get => _bobContacted;
        set
        {
            if (value == _bobContacted) return;
            _bobContacted = value;
            DataTools.HandleOnDataChanged(this);
        }
    }
    public bool CrewQuartersUnlocked
    {
        get => _crewQuartersUnlocked;
        set
        {
            if (value == _crewQuartersUnlocked) return;
            _crewQuartersUnlocked = value;
            DataTools.HandleOnDataChanged(this);
        }
    }

    public string SceneName
    {
        get => _sceneName;
        set
        {
            _sceneName = value;
            DataTools.HandleOnDataChanged(this);
        }
    }
    public bool AliceAndBobFuneralHeld
    {
        get => _aliceAndBobFuneralHeld;
        set
        {
            if (value == _aliceAndBobFuneralHeld) return;
            _aliceAndBobFuneralHeld = value;
            DataTools.HandleOnDataChanged(this);
        }
    }

    public IReadOnlyDictionary<string, EnumObjectiveStatus> ObjectivesAndStatusesDict
    {
        get
        {
            return _objectivesAndStatusesDict;
        }
    }

    

    // Objectives
    public void UpdateObjectivesAndStatuses(Dictionary<string, EnumObjectiveStatus> inDict)
    {
        _objectivesAndStatusesDict = new Dictionary<string, EnumObjectiveStatus>(inDict);
        DataTools.HandleOnDataChanged(this);
    }

    // Defeated Enemies
    public List<string> GetDefeatedEnemiesList()
    {
        return new List<string>(_defeatedEnemiesList);
    }
    public void DefeatEnemy(string enemyName)
    {
        if (_defeatedEnemiesList.Contains(enemyName)) return;
        _defeatedEnemiesList.Add(enemyName);
        DataTools.HandleOnDataChanged(this);
    }
    public void ReplaceDefeatedEnemiesList(List<string> inList)
    {
        _defeatedEnemiesList = new List<string>(inList);
    }

    // BREAKABLES

    public List<string> GetExhaustedBreakablesList()
    {
        return new List<string>(_exhaustedBrakables);
    }
    public void AddExhaustedBreakable(string worldID)
    {
        if (_exhaustedBrakables.Contains(worldID)) return;
        _exhaustedBrakables.Add(worldID);
        DataTools.HandleOnDataChanged(this);
    }
    public void ReplaceExhaustedBreakablesList(List<string> inList)
    {
        _exhaustedBrakables = new List<string>(inList);
    }

    // TERMINALS
    public List<string> GetUnlockedTerminalWorldIDs()
    {
        return new List<string>(_unlockedTerminalWorldIDs);
    }
    public void AddUnlockedTerminalWorldID(string worldID)
    {
        Debug.Log("Adding unlocked terminal world ID: " + worldID);
        if (_unlockedTerminalWorldIDs.Contains(worldID)) return;
        _unlockedTerminalWorldIDs.Add(worldID);
        DataTools.HandleOnDataChanged(this);
    }
    public void ReplaceUnlockedTerminalWorldIDs(List<string> ids)
    {
        _unlockedTerminalWorldIDs = new List<string>(ids);
    }


    //////////////////
    // Constructors //
    //////////////////
    public ProgressionData()
    {
        IsSandbox = false;
        SceneName = "Bootstrap";
        _defeatedEnemiesList = new List<string>();
        _objectivesAndStatusesDict = new Dictionary<string, EnumObjectiveStatus>();
        _exhaustedBrakables = new List<string>();
        _unlockedTerminalWorldIDs = new List<string>();

        _dataDelivered = false;
        _centralCorridorDiscovered = false;
        _bobContacted = false;
        _crewQuartersUnlocked = false;


        AliceAndBobFuneralHeld = false;
    }
    public ProgressionData(bool isSandBox)
    {
        IsSandbox = isSandBox;
        SceneName = "Bootstrap";
        _defeatedEnemiesList = new List<string>();
        _objectivesAndStatusesDict = new Dictionary<string, EnumObjectiveStatus>();
        _exhaustedBrakables = new List<string>();
        _unlockedTerminalWorldIDs = new List<string>();

        _dataDelivered = false;
        _centralCorridorDiscovered = false;
        _bobContacted = false;
        _crewQuartersUnlocked = false;

        AliceAndBobFuneralHeld = false;
    }
    
    public ProgressionData(ProgressionData inData)
    // use this constructor when loading a saved game
    {
        IsSandbox = false;
        SceneName = inData.SceneName;
        _defeatedEnemiesList = new List<string>(inData.GetDefeatedEnemiesList());
        _objectivesAndStatusesDict = new Dictionary<string, EnumObjectiveStatus>(inData.ObjectivesAndStatusesDict);
        _exhaustedBrakables = inData.GetExhaustedBreakablesList();

        _dataDelivered = inData.DataDelivered;
        _centralCorridorDiscovered = inData.CentralCorridorDiscovered;
        _bobContacted = inData.BobContacted;
        _crewQuartersUnlocked = inData.CrewQuartersUnlocked;
        _unlockedTerminalWorldIDs = inData.GetUnlockedTerminalWorldIDs();


        AliceAndBobFuneralHeld = inData.AliceAndBobFuneralHeld;
    }
    public bool GetIsSandbox() { return IsSandbox; }




}
