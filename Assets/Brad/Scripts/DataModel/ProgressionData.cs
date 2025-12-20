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
    private List<EnumObjective> _notStartedObjectives;
    private List<EnumObjective> _startedObjectives;
    private List<EnumObjective> _finishedObjectives;


    ///////////////////
    // Public Access //
    ///////////////////

    // Atomic Fields //

    public string SceneName
    {
        get => _sceneName;
        set
        {
            _sceneName = value;
            DataTools.HandleOnDataChanged(this);
        }
    }

    // Lists //

    // Objectives
    public List<EnumObjective> GetNotStartedObjectivesList()
    {
        if (_notStartedObjectives == null) return null;
        return new List<EnumObjective>(_notStartedObjectives);
    }
    public List<EnumObjective> GetStartedObjectivesList()
    {
        if (_startedObjectives == null) return null;
        return new List<EnumObjective>(_startedObjectives);
    }
    public List<EnumObjective> GetFinishedObjectivesList()
    {
        if (_finishedObjectives == null) return null;
        return new List<EnumObjective>(_finishedObjectives);
    }
    public void StartObjective(EnumObjective objective)
    {
        if (_finishedObjectives.Contains(objective)) return;
        if (_startedObjectives.Contains(objective)) return;
        if (!_notStartedObjectives.Contains(objective)) return;
        _notStartedObjectives.Remove(objective);
        _startedObjectives.Add(objective);
        DataTools.HandleOnDataChanged(this);
    }
    public void FinishObjective(EnumObjective objective)
    {
        if (_notStartedObjectives.Contains(objective)) return;
        if (_finishedObjectives.Contains(objective)) return;
        if (!_startedObjectives.Contains(objective)) return;
        _startedObjectives.Remove(objective);
        _finishedObjectives.Add(objective);
        DataTools.HandleOnDataChanged(this);
    }
    public void NotStartObjective(EnumObjective objective)
    {
        if (_startedObjectives.Contains(objective)) { _startedObjectives.Remove(objective); }
        if (_finishedObjectives.Contains(objective)) { _finishedObjectives.Remove(objective); }
        if (_notStartedObjectives.Contains(objective)) return;
        _notStartedObjectives.Add(objective);
        DataTools.HandleOnDataChanged(this);
    }
    public void ClearObjectivesData()
    {
        _startedObjectives = new List<EnumObjective>();
        _notStartedObjectives = new List<EnumObjective>();
        _finishedObjectives = new List<EnumObjective>();
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

    public void InitializeObjective(EnumObjective objective, EnumObjectiveStatus defaultStatus)
    {
        switch (defaultStatus)
        {
            case EnumObjectiveStatus.NOT_STARTED:
                if (!_notStartedObjectives.Contains(objective)) { _notStartedObjectives.Add(objective); }
                break;
            case EnumObjectiveStatus.STARTED:
                if (!_startedObjectives.Contains(objective)) { _startedObjectives.Add(objective); }
                break;
            case EnumObjectiveStatus.FINISHED:
                if(!_finishedObjectives.Contains(objective)) { _finishedObjectives.Add(objective); }
                break;
        }
        DataTools.HandleOnDataChanged(this);
    }



    //////////////////
    // Constructors //
    //////////////////
    public ProgressionData()
    {
        IsSandbox = false;
        SceneName = "Bootstrap";
        _notStartedObjectives = new List<EnumObjective>();
        _startedObjectives = new List<EnumObjective>();
        _finishedObjectives = new List<EnumObjective>();
        _defeatedEnemiesList = new List<string>();
    }
    public ProgressionData(bool isSandBox)
    {
        IsSandbox = isSandBox;
        SceneName = "Bootstrap";
        _notStartedObjectives = new List<EnumObjective>();
        _startedObjectives = new List<EnumObjective>();
        _finishedObjectives = new List<EnumObjective>();
        _defeatedEnemiesList = new List<string>();
    }

    public ProgressionData(ProgressionData inData)
    {
        IsSandbox = false;
        SceneName = inData.SceneName;
        _notStartedObjectives = inData.GetNotStartedObjectivesList();
        _startedObjectives = inData.GetStartedObjectivesList();
        _finishedObjectives = inData.GetFinishedObjectivesList();
        _defeatedEnemiesList = inData.GetDefeatedEnemiesList();
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
