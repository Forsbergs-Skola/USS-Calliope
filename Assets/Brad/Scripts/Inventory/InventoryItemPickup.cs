using UnityEngine;

public class InventoryItemPickup : MonoBehaviour
{
    [SerializeField] private string inventoryCatalogID;



    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        AddItemToInventory();
    }





    public void AddItemToInventory()
    {
        if (InventoryController.Instance == null) return;
        InventoryController.Instance.AddItemToInventory(inventoryCatalogID);
        Destroy(gameObject);
    }
}
