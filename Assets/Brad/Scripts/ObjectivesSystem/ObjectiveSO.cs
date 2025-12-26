using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObjectiveSO", menuName = "Objectives/ObjectiveSO")]
public class ObjectiveSO : ScriptableObject
{
    [SerializeField] private string objectiveID = string.Empty;
    [SerializeField] private string objectiveTitle = string.Empty;
    [TextArea][SerializeField] private string objectiveDescription = string.Empty;
    [SerializeField] private EnumObjectiveStatus defaultStatus = EnumObjectiveStatus.NOT_STARTED;
    [SerializeField] private int xpReward = 0;

    [SerializeField] private List<string> prerequisiteObjectiveIDs = new List<string>();


    // I expect to see something in the Inspector tab here, where I can drag the KillAliceCompletionCriteria script
    // from the project window, but there's nothing
    //[SerializeReference] private List<CompletionCriteria> completionCriteriaList = new List<CompletionCriteria>();
    [SerializeField] private CompletionCriteriaSO completionCriteria;

    public string ObjectiveID { get => objectiveID; }
    public string ObjectiveTitle { get => objectiveTitle; }
    public string ObjectiveDescription { get => objectiveDescription; }
    public EnumObjectiveStatus DefaultStatus { get => defaultStatus; }
    public int XPReward { get => xpReward; }
    public List<string> PrerequisiteObjectiveIDs { get => prerequisiteObjectiveIDs; }
    public CompletionCriteriaSO CompletionCriteria { get => completionCriteria; }



    private void OnValidate()
    {
        if (string.IsNullOrEmpty(objectiveID))
        {
            objectiveID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
