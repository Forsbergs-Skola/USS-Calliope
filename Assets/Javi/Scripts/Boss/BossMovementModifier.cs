using UnityEngine;

public class BossMovementModifier : MonoBehaviour
{
    public float speedMultiplier = 1.5f;

    private EnemyFollowPlayer movement;

    private void Awake()
    {
        movement = GetComponent<EnemyFollowPlayer>();
        //movement.MoveSpeed *= speedMultiplier;
    }
}