using UnityEngine;

public class ConsumablePrefab : MonoBehaviour
{
    [SerializeField] private string pickupName;
    [Range(1,100)][SerializeField] private int qty = 1;
    [SerializeField] private string worldID;
    [SerializeField] private ConsumableItemSO itemSO;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Olle.Scripts.PlayerController pc = other.GetComponent<Olle.Scripts.PlayerController>();
        pc.HandleConsumablePickup(worldID, itemSO.ItemID, qty);
    }


}
