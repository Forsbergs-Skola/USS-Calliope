using UnityEngine;

public class MiniMapRoomTrigger : MonoBehaviour
{
    public MinimapRoomLightUp mapNode;
    static MinimapRoomLightUp currentRoom;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (currentRoom == mapNode) return;
        
        if (currentRoom != null) currentRoom.SetActive(false);
        
        currentRoom = mapNode;
        if (currentRoom != null) currentRoom.SetActive(true);
    }
}