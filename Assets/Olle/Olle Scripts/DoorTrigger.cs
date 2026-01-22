using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public SlidingDoor door;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || door == null) return;
        
        if (door.isLocked)
        {
            door.TryUnlock();
        }
        
        door.SetPlayerInTrigger(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || door == null) return;
        door.SetPlayerInTrigger(false);
    }
}