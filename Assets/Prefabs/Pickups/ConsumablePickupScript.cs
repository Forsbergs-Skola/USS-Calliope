using UnityEngine;

public class ConsumablePickupScript : MonoBehaviour
{
    [SerializeField] private string consumableID = string.Empty;
    [Range(1, 100)][SerializeField] private int qty = 1;
    private Collider myColl;

    private void Awake()
    {
        myColl = GetComponent<Collider>();
        if (consumableID == string.Empty)
        {
            myColl.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        myColl.enabled = false;

        Olle.Scripts.PlayerController pcontroller = other.GetComponent<Olle.Scripts.PlayerController>();
        pcontroller.HandleConsumablePickup(consumableID, qty);
        Destroy(gameObject);
    }


}
