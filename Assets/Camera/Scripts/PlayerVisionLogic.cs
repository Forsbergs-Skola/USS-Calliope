using System.Collections.Generic;
using UnityEngine;

public class PlayerVisionLogic : MonoBehaviour
{
    public float viewDistance = 8f;
    public float viewAngle = 90f;
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;
    public float eyeHeight = 1.5f;
    
    private HashSet<Collider> trackedEnemies = new HashSet<Collider>(); //Like a list but doesn't allow duplicates and have fast lookup
    
    void Update()
    {
        Collider[] detectedEnemies = Physics.OverlapSphere(transform.position, viewDistance, enemyLayer);

        foreach (Collider enemy in trackedEnemies)
        {
            SetEnemyVisibility(enemy, false);
        }

        trackedEnemies.Clear();

        foreach (Collider enemy in detectedEnemies)
        {
            bool isVisible = CheckVisibility(enemy);

            SetEnemyVisibility(enemy, isVisible);

            trackedEnemies.Add(enemy);
        }
    }
    
    bool CheckVisibility(Collider enemy)
    {
        Vector3 directionToEnemy = enemy.transform.position - transform.position;
        directionToEnemy.y = 0f;

        Vector3 forward = transform.forward;
        forward.y = 0f;

        float angleToEnemy = Vector3.Angle(forward, directionToEnemy);

        if (angleToEnemy > viewAngle * 0.5f)
            return false;

        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
        Vector3 enemyTarget = enemy.bounds.center;

        Vector3 raycastDirection = enemyTarget - eyePosition;
        float raycastDistance = raycastDirection.magnitude;

        if (Physics.Raycast(eyePosition, raycastDirection.normalized, raycastDistance, obstacleLayer))
            return false;

        Debug.DrawLine(eyePosition, enemyTarget, Color.green);
        return true;
    }

    void SetEnemyVisibility(Collider enemy, bool visible)
    {
        if (!enemy) return;
        
        Renderer renderer = enemy.GetComponent<Renderer>();
        SneakyEnemy sneaky = enemy.GetComponent<SneakyEnemy>();

        if (renderer != null)
            renderer.enabled = visible;

        if (sneaky != null)
            sneaky.SetSeen(visible);
        
        //////
        Renderer[] renderers = enemy.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderers)
        {
            render.enabled = visible;
        }

        
    }
    
    
    
}