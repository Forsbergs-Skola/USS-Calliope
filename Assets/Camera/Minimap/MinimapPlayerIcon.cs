using UnityEngine;

public class MinimapPlayerIcon : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform icon;
    void LateUpdate()
    {
        if (!player) return;
        
        float playerYaw = player.eulerAngles.y;
        icon.localEulerAngles = new Vector3(0f, 0f, -playerYaw);
    }
}