using UnityEngine;
using System.Collections.Generic;

public class ObjectivesPanel : MonoBehaviour
{
    [SerializeField] private ProgressionRuntimeData progressionRuntimeData;
    private ProgressionData progressionData = null;

    private void OnEnable()
    {
        progressionData = progressionRuntimeData.Value;
    }

    

}
