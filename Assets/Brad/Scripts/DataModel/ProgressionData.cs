using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class ProgressionData : IRuntimeData
// All the save-worthy progression information
{
    public bool IsSandbox { get; private set; }

    private List<string> _defeatedEnemies;
    private string _sceneName;


    private bool _talkedToBob;
    private bool _talkedToAlice;

    /////////////////
    // Data Fields //
    /////////////////

    // TODO all the progression data fields

    // list fields

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
    
    public string SceneName
    {
        get => _sceneName;
        set
        {
            _sceneName = value;
            DataTools.HandleOnDataChanged(this);
        }
    }

    public bool TalkedToBob
    {
        get => _talkedToBob;
        set
        {
            _talkedToBob = value;
            DataTools.HandleOnDataChanged(this);
        }
    }

    
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

    public ProgressionData(ProgressionData inData)
    {
        IsSandbox = false;
        TalkedToBob = inData.TalkedToBob;
        TalkedToAlice = inData.TalkedToAlice;
        _defeatedEnemies = inData.GetDefeatedEnemies();
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
