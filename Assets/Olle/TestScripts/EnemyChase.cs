using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Visual")]
    public Renderer bodyRenderer;
    public Color idleColor = Color.green;
    public Color alertColor = Color.red;

    [Header("Target")]
    public Transform player;

    [Header("Vision")]
    public float viewDistance = 10f;
    public float viewAngle = 60f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float returnSpeed = 3f;

    [Header("Memory")]
    public float loseSightDelay = 0.3f;

    [Header("Attack")]
    public float attackDistance = 1.2f;   // how close before push
    public float pushForce = 6f;          // strength of push

    Vector3 startPos;
    Quaternion startRot;
    bool chasing;
    float lastTimeSeenPlayer = -999f;

    Rigidbody playerRb;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        if (bodyRenderer == null)
            bodyRenderer = GetComponentInChildren<Renderer>();

        if (bodyRenderer != null)
            bodyRenderer.material.color = idleColor;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
            playerRb = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        bool canSee = CanSeePlayer();

        if (canSee)
            lastTimeSeenPlayer = Time.time;

        bool shouldChase = (Time.time - lastTimeSeenPlayer) <= loseSightDelay;

        if (shouldChase && !chasing)
        {
            chasing = true;
            if (bodyRenderer != null)
                bodyRenderer.material.color = alertColor;
        }
        else if (!shouldChase && chasing)
        {
            chasing = false;
            if (bodyRenderer != null)
                bodyRenderer.material.color = idleColor;
        }

        if (chasing)
            ChasePlayer();
        else
            ReturnToStart();
    }

    bool CanSeePlayer()
    {
        if (player == null)
            return false;

        Vector3 eyePos = transform.position + Vector3.up * 0.7f;
        Vector3 toPlayer = player.position - eyePos;
        float distance = toPlayer.magnitude;

        if (distance > viewDistance)
            return false;

        float angle = Vector3.Angle(transform.forward, toPlayer);
        if (angle > viewAngle)
            return false;

        Vector3 dir = toPlayer.normalized;

        if (Physics.Raycast(eyePos, dir, out RaycastHit hit, viewDistance, ~0))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    void ChasePlayer()
    {
        if (player == null)
            return;

        Vector3 target = player.position;
        target.y = transform.position.y;

        Vector3 toPlayer = target - transform.position;
        float dist = toPlayer.magnitude;

        if (dist > attackDistance)
        {
            // move toward player
            transform.position = Vector3.MoveTowards(transform.position,
                                                     target,
                                                     moveSpeed * Time.deltaTime);
        }
        else
        {
            // close enough: push player backwards
            if (playerRb != null)
            {
                Vector3 pushDir = toPlayer.normalized;
                pushDir.y = 0f;
                playerRb.AddForce(pushDir * pushForce, ForceMode.VelocityChange);
            }
        }

        transform.LookAt(target);
    }

    void ReturnToStart()
    {
        Vector3 targetPos = startPos;
        targetPos.y = transform.position.y;

        transform.position = Vector3.MoveTowards(transform.position,
                                                 targetPos,
                                                 returnSpeed * Time.deltaTime);

        if ((transform.position - startPos).sqrMagnitude > 0.001f)
        {
            Vector3 lookPos = startPos;
            lookPos.y = transform.position.y;

            Quaternion lookRot = Quaternion.LookRotation(lookPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  lookRot,
                                                  5f * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  startRot,
                                                  5f * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 eyePos = transform.position + Vector3.up * 1f;

        Vector3 leftDir  = Quaternion.Euler(0f, -viewAngle, 0f) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0f,  viewAngle, 0f) * transform.forward;

        Gizmos.DrawLine(eyePos, eyePos + leftDir  * viewDistance);
        Gizmos.DrawLine(eyePos, eyePos + rightDir * viewDistance);
        Gizmos.DrawLine(eyePos, eyePos + transform.forward * viewDistance);
    }

    public void HeardPlayer(Vector3 noisePosition)
    {
        lastTimeSeenPlayer = Time.time;
        chasing = true;
        if (bodyRenderer != null)
            bodyRenderer.material.color = alertColor;
    }
}
