using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float height;


    void LateUpdate()
    {
        if (!player) return;
        
        Vector3 position = player.position;
        transform.position = new Vector3(position.x, position.y + height, position.z);
        transform.rotation = Quaternion.Euler(90f, 90f, 0f);
    }
}
