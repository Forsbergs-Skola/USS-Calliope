using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour
{
    private Animator anim;
    private EnemyAIStateController ai;
    private EnemyAttackController attack;
    private SimpleMovementAgent movement;

    private void Awake()
    {
        //anim = GetComponent<Animator>();
        anim = GetComponentInChildren<Animator>();
        ai = GetComponent<EnemyAIStateController>();
        attack = GetComponent<EnemyAttackController>();
        movement = GetComponent<SimpleMovementAgent>();
    }

    private void Update()
    {
        if (anim.GetBool("IsDead"))
            return;
                
        // movement (Idle / Walk / Run)
        float moveSpeed = movement != null ? movement.NormalizedSpeed : 0f;
        anim.SetFloat("MoveSpeed", moveSpeed);
        //Debug.Log($"[AnimController] MoveSpeed={moveSpeed}");

        // states
        anim.SetBool("IsDead", ai.IsDead);
        anim.SetBool("IsStunned", ai.IsStunned);

        // Attacks
        if (ai.CurrentState == EnemyAIStateController.State.Attacking &&
            attack.HasActiveAttack(out int attackType))
        {
            anim.SetBool("IsAttacking", true);
            anim.SetInteger("AttackType", attackType);
        }
        else
        {
            anim.SetBool("IsAttacking", false);
            anim.SetInteger("AttackType", 0);
        }
    }
}