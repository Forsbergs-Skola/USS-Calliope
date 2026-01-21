using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAIStateController))]
[RequireComponent(typeof(SimpleMovementAgent))]
public class BossWatchfulController : MonoBehaviour
{
    private EnemyAIStateController ai;
    private SimpleMovementAgent movement;
    private Transform player;
    private PatrolZone currentZone;

    private void Awake()
    {
        ai = GetComponent<EnemyAIStateController>();
        movement = GetComponent<SimpleMovementAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (ai.CurrentState != EnemyAIStateController.State.Watchful)
            return;

        if (player == null) return;

        PatrolZone zone = PatrolPointRegistry.GetClosestZone(player.position);
        if (zone == null) return;
        
        List<Transform> points = PatrolPointRegistry.GetPointsForZone(currentZone);

        if (points == null || points.Count == 0)
        {
            Debug.LogWarning($"{name}: No points found for zone {currentZone.name}");
            return;
        }

        Transform nextPoint = points[Random.Range(0, points.Count)];
        movement.MoveTo(nextPoint.position);
        //Vector3 targetPoint = zone.GetRandomPoint();
        //.MoveTo(nextPoint);
    }
}