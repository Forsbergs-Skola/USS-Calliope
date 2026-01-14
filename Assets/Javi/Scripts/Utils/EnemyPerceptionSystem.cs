using UnityEngine;

public class EnemyPerceptionSystem : MonoBehaviour, IPerceptionSystem
{
    /*[SerializeField] private float viewDistance = 10f;
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private LayerMask obstacleMask;*/
    private EnemyVisionCone vision;
    
    void Awake()
    {
        vision = GetComponent<EnemyVisionCone>();
    }

    public bool CanSeeTarget(Transform target)
    {
        if (vision == null) return false;
        return vision.CanSeePlayer();
        
        /*Vector3 dir = target.position - transform.position;
        if (dir.magnitude > viewDistance) return false;
        if (Vector3.Angle(transform.forward, dir) > viewAngle) return false;

        if (Physics.Raycast(transform.position, dir.normalized, out RaycastHit hit, viewDistance))
        {
            return hit.transform == target;
        }
        return false;*/
    }

    public bool CanHearNoise(Vector3 position, float intensity)
    {
        return Vector3.Distance(transform.position, position) <= intensity;
    }

    public bool CanDetectBioelectric(Transform target)
    {
        // Future suit system can reduce this
        return Vector3.Distance(transform.position, target.position) < 6f;
    }
}