using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SneakyEnemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    private bool isSeen;

    [SerializeField] private Transform player;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetSeen(bool seen)
    {
        isSeen = seen;

        if (seen)
        {
            rb.constraints =
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.constraints =
                RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotation;
        }
    }

    void FixedUpdate()
    {
        if (isSeen || player == null)
            return;

        Vector3 toPlayer = player.position - rb.position;
        toPlayer.y = 0f;

        Vector3 direction = toPlayer.normalized;

        Vector3 newPosition =
            rb.position + direction * (moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);
    }
}