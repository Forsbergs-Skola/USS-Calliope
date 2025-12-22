using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private InventoryRuntimeData inventoryRuntimeData;
    private InventoryData inventoryData = null;


    private void OnEnable()
    {
        inventoryData = inventoryRuntimeData.Value;
    }

}
