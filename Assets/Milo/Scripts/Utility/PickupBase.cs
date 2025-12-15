using UnityEngine;

public abstract class PickupBase : MonoBehaviour, IPickupable
{
    private void OnTriggerEnter(Collider other)
    {
        // Try to get the component that can pick up this item
        var ammoModel = other.GetComponent<AmmoModel>();
        if (ammoModel != null)
        {
            OnPickup(other.gameObject);  
            Destroy(gameObject);          
        }
    }

    public abstract void OnPickup(GameObject picker);
}