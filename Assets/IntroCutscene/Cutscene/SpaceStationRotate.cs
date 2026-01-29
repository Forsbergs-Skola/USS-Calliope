using UnityEngine;

public class SpaceStationRotate : MonoBehaviour
{
    [SerializeField] float speed = 90f; 

    void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}
