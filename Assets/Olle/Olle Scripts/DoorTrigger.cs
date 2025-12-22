using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public SlidingDoor door;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (door != null)
                door.SetOpen(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (door != null)
                door.SetOpen(false);
        }
    }
}