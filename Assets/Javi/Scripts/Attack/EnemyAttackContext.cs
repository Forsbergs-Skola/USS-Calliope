using UnityEngine;

public class EnemyAttackContext
{
    public Transform enemy;
    public Transform player;
    public float infectionPercentage;
    public EnemyFollowPlayer movement;
    public MonoBehaviour coroutineRunner;
    public Transform firePoint;
}