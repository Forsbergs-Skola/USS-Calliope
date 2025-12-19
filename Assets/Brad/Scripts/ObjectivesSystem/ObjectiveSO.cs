using UnityEngine;
using System.Collections.Generic;


public enum EnumProgressionField
{
    DEFEATED_ENEMIES_COUNT,
    DEFEATED_ENEMIES,
    FINISHED_OBJECTIVES
    // more as needed
}
public enum EnumValueComparison
{
    GREATER_OR_EQUAL,
    LESS_OR_EQUAL,
    EQUAL,
    CONTAINS
}
public enum EnumObjectiveStatus
{
    NOT_STARTED,
    STARTED,
    FINISHED
}
public enum EnumProgressionFieldType
{
    BOOL,
    INT,
    FLOAT,
    STRING,
    STRING_LIST

}

[System.Serializable]
public struct ObjectiveCriterion
{
    public EnumProgressionField progressionField;
    public EnumProgressionFieldType progressionFieldType;
    public EnumValueComparison valueComparison;

    //[Header("Target Value")]
    public bool negate;
    public bool boolTarget;
    public int intTarget;
    public float floatTarget;
    public string stringTarget;

    // if progressionField's valueComparison relation to the appropriate target value is TRUE
    // ...then the criterion is met
}

[CreateAssetMenu(fileName = "ObjectiveSO", menuName = "Objectives/ObjectiveSO")]
public class ObjectiveSO : ScriptableObject
{

    // Private //
    [SerializeField] private EnumObjective objectiveID = EnumObjective.NONE;
    [SerializeField] private string objectiveTitle = string.Empty;
    [TextArea][SerializeField] private string objectiveDescription = string.Empty;
    [SerializeField] private EnumObjectiveStatus defaultStatus = EnumObjectiveStatus.NOT_STARTED;
    [SerializeField] private List<ObjectiveCriterion> entryCriteria = new List<ObjectiveCriterion>();
    [SerializeField] private List<ObjectiveCriterion> completionCriteria= new List<ObjectiveCriterion>();

    // EnumInventoryItem is defined in the inventory system
    // "Item" is something that the inventory can contain no more than 1 of...
    // Example: "EnumInventoryItem.FLASHLIGHT"
    [SerializeField] private EnumInventoryItem itemReward = EnumInventoryItem.NONE;

    // EnumInventoryResource is defined in the inventory system
    // "Resource" is something that the inventory can contain any
    // natural number of...
    // Example: EnumInventoryResource.PISTOL_AMMO
    [SerializeField] private EnumInventoryResource resourceReward = EnumInventoryResource.NONE;
    [SerializeField] private int resourceRewardQuantity = 0;

    // XP is kept in PlayerData.XP
    [SerializeField] private int xpReward = 0;

    //private EnumObjectiveStatus currentStatus = EnumObjectiveStatus.NOT_STARTED;

    // Public Getters//
    public EnumObjective ObjectiveID { get => objectiveID; }
    public string ObjectiveTitle { get => objectiveTitle; }
    public string ObjectiveDescription { get => objectiveDescription; }
    public EnumObjectiveStatus DefaultStatus { get => defaultStatus; }
    public List<ObjectiveCriterion> EntryCriteria { get => entryCriteria; }
    public List<ObjectiveCriterion> CompletionCriteria { get => completionCriteria; }
    public EnumInventoryItem ItemReward { get => itemReward; }
    public EnumInventoryResource ResourceReward { get => resourceReward; }
    public int ResourceRewardQuantiny { get => resourceRewardQuantity; }
    public int XPReward { get => xpReward; }
    //public EnumObjectiveStatus CurrentStatus { }
}
