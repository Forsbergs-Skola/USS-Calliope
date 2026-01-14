using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    [SerializeField] private PatrolZone patrolZone;

    public PatrolZone PatrolZone => patrolZone;
    
    private void Awake()
    {
        if (PatrolPointRegistry.GetPointsForZone(patrolZone) == null)
        {
            PatrolPointRegistry.Initialize();
        }
    }
    
    private void Start()
    {
        PatrolPointRegistry.RegisterPoint(this);
    }

    private void OnDestroy()
    {
        PatrolPointRegistry.UnregisterPoint(this);
    }

    /*
    // Only for Unity not for builds
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = patrolZone != null ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, 0.3f);
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Name of the point
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"{name}\n[{patrolZone.name}]");
    }
    private void OnValidate()
    {
        if (patrolZone == null)
            Debug.LogWarning($"{name} has no PatrolZone assigned", this);
    }
#endif
    */
}