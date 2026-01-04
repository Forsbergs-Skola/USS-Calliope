using System.Collections.Generic;
using UnityEngine;


public class AmmoPresenter : MonoBehaviour
{

    private InventoryData invData
    {
        get
        {
            if (DataController.Instance == null) return null;
            else { return DataController.Instance.InventoryRuntimeData.Value; }
        }
    }
}
