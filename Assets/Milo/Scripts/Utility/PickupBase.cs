using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        OnPickup(other.gameObject);
        Destroy(gameObject);
    }

    public abstract void OnPickup(GameObject picker);
}