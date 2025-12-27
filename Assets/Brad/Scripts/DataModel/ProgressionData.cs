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
    private bool _aliceAndBobFuneralHeld = false;

   

    ///////////////////
    // Public Access //
    ///////////////////



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
        AliceAndBobFuneralHeld = false;
    }
    public ProgressionData(bool isSandBox)
    {
        IsSandbox = isSandBox;
        SceneName = "Bootstrap";
        _defeatedEnemiesList = new List<string>();
        _objectivesAndStatusesDict = new Dictionary<string, EnumObjectiveStatus>();
        AliceAndBobFuneralHeld = false;
    }
    
    public ProgressionData(ProgressionData inData)
    // use this constructor when loading a saved game
    {
        IsSandbox = false;
        SceneName = inData.SceneName;
        _defeatedEnemiesList = new List<string>(inData.GetDefeatedEnemiesList());
        _objectivesAndStatusesDict = new Dictionary<string, EnumObjectiveStatus>(inData.ObjectivesAndStatusesDict);
        AliceAndBobFuneralHeld = inData.AliceAndBobFuneralHeld;
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
