using UnityEngine;
using System.Collections.Generic;

public class ObjectivesTracker : Singleton<ObjectivesTracker>
{
    [SerializeField] private List<ObjectiveSO> objectives;

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }

    private void HandleProgressionDataUpdate(IRuntimeData data)
    {
        if (!(data is ProgressionData)) return;
        ProgressionData progData = data as ProgressionData;


        foreach(ObjectiveSO obj in objectives)
        {
            //switch (ObjectiveSO.St)
        }


    }

    private bool GetIsCriteriaListMet(List<ObjectiveCriterion> criteriaList)
    {
        bool isMet = true;



        return isMet;
    }

}
