using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObjectiveSO", menuName = "Objectives/ObjectiveSO")]
public class ObjectiveSO : ScriptableObject
{

    // Private //
    [SerializeField] private EnumObjective objectiveID;
    [SerializeField] private string objectiveTitle = string.Empty;
    [TextArea][SerializeField] private string objectiveDescription;
    [SerializeField] private EnumObjectiveStatus defaultStatus = EnumObjectiveStatus.NOT_STARTED;
    [SerializeField] private List<Structs.ObjectiveCriterion> entryCriteria;
    [SerializeField] private List<Structs.ObjectiveCriterion> completionCriteria;

    // entryCriteria:
    //  - Should be a list of conditions that must all be TRUE for the objective
    //    to move from NOT_STARTED to STARTED. These are stored in the Value
    //    field of the ProgressionRuntimeData class
    //
    //  - Whenever a value changes in ProgressionRuntimeData.Value, it fires an event that an objective tracker
    //    singleton can subscribe to, like so:
    /*
        private void HandleProgressionDataCHanged(IRuntimeData data)
        {
            if (!(data is ProgressionData)) return;
            ProgressionData currentProgressionData = data as ProgressionData;

            // example -- has the player talked to Alice yet?
            bool talkedToAlice = currentProgressionData.TalkedToAlice;
            //...
        }
        */

    // completionCriteria:
    //  - Should be a list of conditions that must all be true for the objective
    //    to move from STARTED to FINISHED. See above...



    // Public Getters//
    public EnumObjective ObjectiveID { get => objectiveID; }
    public string ObjectiveTitle { get => objectiveTitle; }
    public string ObjectiveDescription { get => objectiveDescription; }
    public EnumObjectiveStatus DefaultStatus { get => defaultStatus; }

    // also public getters for EntryCriteria and CompletionCriteria




}
