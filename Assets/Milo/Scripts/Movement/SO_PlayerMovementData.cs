using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerMovementData", menuName = "Player/Movement/SO_PlayerMovementData")]
public class SO_PlayerMovementData : ScriptableObject
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runMoveSpeed;
    [SerializeField] private float crouchMoveSpeed;

    public float MoveSpeed => moveSpeed;
    public float RunMoveSpeed => runMoveSpeed;
    public float CrouchMoveSpeed => crouchMoveSpeed;
}
