using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class ProgressionData : IRuntimeData
// All the save-worthy progression information
{
    public bool IsSandbox { get; private set; }
    
    private string _sceneName;
    //private List<string> _defeatedEnemiesList;
    private List<EnumObjective> notStartedObjectives;
    private List<EnumObjective> startedObjectives;
    private List<EnumObjective> finishedObjectives;


    

    public string SceneName
    {
        get => _sceneName;
        set
        {
            _sceneName = value;
            DataTools.HandleOnDataChanged(this);
        }
    }



    //////////////////
    // Constructors //
    //////////////////
    public ProgressionData()
    {
        IsSandbox = false;
        SceneName = "Bootstrap";
    }
    public ProgressionData(bool isSandBox)
    {
        IsSandbox = isSandBox;
        SceneName = "Bootstrap";
    }

    public ProgressionData(ProgressionData inData)
    {
        IsSandbox = false;
        SceneName = inData.SceneName;
    }
    public bool GetIsSandbox() { return IsSandbox; }
}
