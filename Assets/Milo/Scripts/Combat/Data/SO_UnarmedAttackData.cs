using UnityEngine;

[CreateAssetMenu(fileName = "SO_UnarmedAttackData", menuName = "Player/Player Combat/SO_UnarmedAttackData")]
public class SO_UnarmedAttackData : ScriptableObject
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float reach = 1.5f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private int pushForce = 5;

    public int Damage => damage;
    public float Reach => reach;
    public float Radius => radius;
    public float AttackCooldown => attackCooldown;
    public int PushForce => pushForce;
}
