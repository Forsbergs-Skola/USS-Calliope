using UnityEngine;

public class JunkScript : MonoBehaviour
{
    InventoryData invData
    {
        get
        {
            if (DataController.Instance == null) return null;
            else return DataController.Instance.InventoryRuntimeData.Value;
        }
    }

    private void Start()
    {
        if (invData == null)
        {
            Debug.Log("My inventory data reference is null");
            // use placeholder data because I'm in sandbox mode
        }
        else
        {
            Debug.Log("My inventory data reference is not null");
            // use the data in invData, which I know is not null 
            // because I explicitly asked it if it was null and
            // it said no...
        }
    }
}
