using UnityEngine;

public class PlayerVisionLogic : MonoBehaviour
{
    public float viewDistance = 8f;
    public float viewAngle = 90f;
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;
    public float eyeHeight = 1.5f;
    
    
    void Update()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, viewDistance, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            Renderer enemyRenderer = enemy.GetComponent<Renderer>();
            SneakyEnemy sneakyEnemy = enemy.GetComponent<SneakyEnemy>();
            
            bool isVisible = false;

            Vector3 directionToEnemy = enemy.transform.position - transform.position;
            directionToEnemy.y = 0f;

            Vector3 forward = transform.forward;
            forward.y = 0f;

            float angleToEnemy = Vector3.Angle(forward, directionToEnemy);
            
            if (angleToEnemy <= viewAngle * 0.5f) //Check if the enemy is in FOV
            {
                Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
                Vector3 enemyTarget = enemy.bounds.center;

                Vector3 raycastDirection = enemyTarget - eyePosition;
                float raycastDistance = raycastDirection.magnitude;
                
                if (!Physics.Raycast(eyePosition, raycastDirection.normalized, raycastDistance, obstacleLayer)) //If no wall blocks enemy
                {
                    isVisible = true;
                    Debug.DrawLine(eyePosition, enemyTarget, Color.green);
                }
            }

            if (enemyRenderer != null)
                enemyRenderer.enabled = isVisible;
            
            if (sneakyEnemy != null)
                sneakyEnemy.SetSeen(isVisible);
        }
    }
}