using UnityEngine;
using UnityEngine.UI;

public class InventoryTestButton : MonoBehaviour
{
    [SerializeField] private Button testButton;

    private void OnEnable()
    {
        testButton.onClick.AddListener(HandleOnPressed);
    }
    private void OnDisable()
    {
        testButton.onClick.RemoveAllListeners();
    }

    private void HandleOnPressed()
    {
        testButton.enabled = false;
        InventoryItemPickup pickup = GetComponent<InventoryItemPickup>();
        pickup.AddItemToInventory();
        gameObject.SetActive(false);
    }


}
