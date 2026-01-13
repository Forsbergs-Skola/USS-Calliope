using UnityEngine;

public class ConsumablePrefab : MonoBehaviour
{
    [SerializeField] private string pickupName;
    [Range(1,100)][SerializeField] private int qty = 1;
    [SerializeField] private string worldID;
    [SerializeField] private ConsumableItemSO itemSO;

    private Collider myColl;

    private void Awake()
    {
        myColl = GetComponent<Collider>();
    }

    private void Start()
    {
        if (DataController.Instance == null) return;
        InventoryData invData = DataController.Instance.InventoryRuntimeData.Value;
        if (invData.GetExhaustedPickups().Contains(worldID))
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        myColl.enabled = false;
        Olle.Scripts.PlayerController pc = other.GetComponent<Olle.Scripts.PlayerController>();
        pc.HandleConsumablePickup(worldID, itemSO.ItemID, qty);
        Destroy(gameObject);

    }


}
