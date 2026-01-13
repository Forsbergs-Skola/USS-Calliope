using System.Collections.Generic;
using UnityEngine;

public class UnarmedAimHelp : MonoBehaviour
{
    [SerializeField] private PlayerVisionLogic visionLogic;
    [SerializeField] private LayerMask enemyMask;

    public PlayerVisionLogic VisionLogic { get => visionLogic; set => visionLogic = value; }
    public LayerMask EnemyMask { get => enemyMask; set => enemyMask = value; }

    /// <summary>
    /// Calculates the distance to the closest visible enemy target within the player's vision.
    /// </summary>

    private Transform GetNearestVisibleEnemy(Transform player, out float distance)
    {
        Collider[] enemiesInView = Physics.OverlapSphere(
            player.position,
            VisionLogic.viewDistance,
            EnemyMask
        );

        Transform bestTarget = null;
        float closestSqrDistance = float.MaxValue;

        foreach (var enemy in enemiesInView)
        {
            if (!VisionLogic.CheckVisibility(enemy))
                continue;

            Vector3 toEnemy = enemy.bounds.center - player.position;
            float sqrDistance = toEnemy.sqrMagnitude;

            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                bestTarget = enemy.transform;
            }
        }

        if (bestTarget != null)
            distance = Mathf.Sqrt(closestSqrDistance);
        else
            distance = -1f;

        return bestTarget;
    }


    /// <summary>
    /// Rotates the player to the target and locks the rotation
    /// </summary>
    private Transform LockOnNearestEnemy(Transform player)
    {
        float distance;
        Transform target = GetNearestVisibleEnemy(player, out distance);

        if (target == null) return null;

        Vector3 direction = target.position - player.position;
        direction.y = 0; 

        if (direction.sqrMagnitude < 0.001f) return null;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        player.rotation = Quaternion.Slerp(player.rotation, targetRotation, Time.deltaTime * 10f);

        float angle = Vector3.Angle(player.forward, direction);
        if (angle > 2f) 
            return null; 

        return target; 
    }
}
