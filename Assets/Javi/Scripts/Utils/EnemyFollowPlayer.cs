using UnityEngine;

public class EnemyFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform playerTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("State")]
    [SerializeField] private bool followPlayer = false;
    
    // Getter & Setter for the speed
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0, value);
    }

    // Getter & Setter to enable/disable tracking
    public bool FollowPlayer
    {
        get => followPlayer;
        set => followPlayer = value;
    }

    private void Update()
    {
        if (!followPlayer || playerTarget == null)
            return;
        
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Vector3 movement = direction * moveSpeed * Time.deltaTime;

        transform.position += movement;
    }

    // Allows you to assign the player from another script later on
    public void SetTarget(Transform target)
    {
        playerTarget = target;
    }
}