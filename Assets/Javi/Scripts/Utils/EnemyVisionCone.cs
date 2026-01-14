using UnityEngine;

[RequireComponent(typeof(InfectionController))]
public class EnemyVisionCone : MonoBehaviour
{
    [Header("Vision Origin")]
    [SerializeField] private Transform visionOrigin;

    [Header("Layers")]
    [SerializeField] private LayerMask obstacleMask;

    [Header("Debug (Editor Only)")]
    [SerializeField] private bool drawGizmos = true;

    float viewDistance = 10f;
    float viewAngle = 60f;

    Transform player;
    Olle.Scripts.CrouchInvisibility playerInvisibility;

    void Awake()
    {
        if (visionOrigin == null)
            visionOrigin = transform;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
            playerInvisibility = player.GetComponent<Olle.Scripts.CrouchInvisibility>();
    }

    public void SetPhaseData(SO_InfectionPhaseData data)
    {
        viewDistance = data.viewDistance;
        viewAngle = data.viewAngle;
    }

    public bool CanSeePlayer()
    {
        if (player == null)
            return false;

        // Invisibility check
        if (playerInvisibility != null && playerInvisibility.IsInvisible)
            return false;

        Vector3 origin = visionOrigin.position;
        Vector3 dirToPlayer = player.position - origin;

        float distance = dirToPlayer.magnitude;
        if (distance > viewDistance)
            return false;

        float angle = Vector3.Angle(visionOrigin.forward, dirToPlayer);
        if (angle > viewAngle * 0.5f)
            return false;

        // Line of sight check
        if (Physics.Raycast(origin, dirToPlayer.normalized, out RaycastHit hit, viewDistance, obstacleMask))
        {
            if (hit.transform != player)
                return false;
        }

        return true;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Transform origin = visionOrigin != null ? visionOrigin : transform;

        Gizmos.color = Color.yellow;

        Vector3 leftDir = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * origin.forward;
        Vector3 rightDir = Quaternion.Euler(0, viewAngle * 0.5f, 0) * origin.forward;

        Gizmos.DrawLine(origin.position, origin.position + leftDir * viewDistance);
        Gizmos.DrawLine(origin.position, origin.position + rightDir * viewDistance);

        int segments = 20;
        float step = viewAngle / segments;

        Vector3 prevPoint = origin.position + leftDir * viewDistance;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -viewAngle * 0.5f + step * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * origin.forward;
            Vector3 nextPoint = origin.position + dir * viewDistance;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        Gizmos.DrawLine(origin.position, prevPoint);
    }
#endif
}
