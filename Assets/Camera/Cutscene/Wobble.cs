using UnityEngine;

public class CameraWobble : MonoBehaviour
{
    [Header("Wobble Amount")]
    [SerializeField] private float pitchAmount = 0.25f;
    [SerializeField] private float yawAmount = 0.15f;
    [SerializeField] private float rollAmount = 0.1f;

    [Header("Speed")]
    [SerializeField] private float pitchSpeed = 1.2f;
    [SerializeField] private float yawSpeed = 0.9f;
    [SerializeField] private float rollSpeed = 1.5f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        float pitch = Mathf.Sin(Time.time * pitchSpeed) * pitchAmount;
        float yaw   = Mathf.Sin(Time.time * yawSpeed)   * yawAmount;
        float roll  = Mathf.Sin(Time.time * rollSpeed)  * rollAmount;

        Quaternion wobble = Quaternion.Euler(pitch, yaw, roll);

        transform.localRotation = startRotation * wobble;
    }
}