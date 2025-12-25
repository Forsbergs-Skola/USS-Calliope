using UnityEngine;

public class InventoryItemPickup : MonoBehaviour
{
    [SerializeField] private string inventoryCatalogID;
    public void AddItemToInventory()
    {
        if (InventoryController.Instance == null) return;
        InventoryController.Instance.AddItemToInventory(inventoryCatalogID);
    }
}
