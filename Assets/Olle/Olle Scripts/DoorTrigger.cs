using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public SlidingDoor door;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || door == null) return;
        
        if (door.lockType == DoorLockType.None)
        {
            door.SetOpen(true);
            return;
        }
        
        if (door.lockType == DoorLockType.Keycard)
        {
            var keycards = other.GetComponent<PlayerKeycards>();
            if (keycards != null)
            {
                keycards.TryUseKeycardForDoor(door);
            }
            return;
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || door == null) return;
        
        if (!door.isLocked)
        {
            door.SetOpen(false);
        }
    }
}