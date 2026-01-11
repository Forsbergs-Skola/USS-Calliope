using UnityEngine;

public class LevelPatrolInitializer : MonoBehaviour
{
    private void Start()
    {
        PatrolPointRegistry.Initialize();
        
        // find the Points for patrolling
        PatrolPoint[] allPoints = FindObjectsOfType<PatrolPoint>(true);
        foreach (var point in allPoints)
        {
            PatrolPointRegistry.RegisterPoint(point);
        }
        
        Debug.Log($"Patrol system initialized with {allPoints.Length} points");
    }

    private void OnDestroy()
    {
        PatrolPointRegistry.ClearRegistry();
    }
}