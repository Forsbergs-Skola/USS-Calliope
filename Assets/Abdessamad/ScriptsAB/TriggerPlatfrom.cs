using UnityEngine;

public class TriggerPlatfrom : MonoBehaviour
{
    PlatformMoving platform;
    void Start()
    {
        platform = GetComponent<PlatformMoving>();
    }

    void OnTriggerEnter(Collider other)
    {
        platform.canMove = true;
    }
}
