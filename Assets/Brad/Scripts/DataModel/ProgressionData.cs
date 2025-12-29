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
    private Dictionary<string, EnumObjectiveStatus> _objectivesAndStatusesDict;

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


    //////////////////
    // Constructors //
    //////////////////
    public ProgressionData()
    {
        IsSandbox = false;
        SceneName = "Bootstrap";
        _defeatedEnemiesList = new List<string>();
        _objectivesAndStatusesDict = new Dictionary<string, EnumObjectiveStatus>();

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


        _dataDelivered = inData.DataDelivered;
        _centralCorridorDiscovered = inData.CentralCorridorDiscovered;
        _bobContacted = inData.BobContacted;
        _crewQuartersUnlocked = inData.CrewQuartersUnlocked;


        AliceAndBobFuneralHeld = inData.AliceAndBobFuneralHeld;
    }
    public bool GetIsSandbox() { return IsSandbox; }




}
