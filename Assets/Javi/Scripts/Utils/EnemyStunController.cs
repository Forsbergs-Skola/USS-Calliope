using System.Collections;
using UnityEngine;
public class EnemyStunController : MonoBehaviour
{
    [Header("Stun Settings")]
    [SerializeField] private float defaultStunTime = 8f;

    private SimpleMovementAgent movement;
    private EnemyAIStateController ai;
    private bool isStunned;
    private Coroutine stunRoutine;

    private void Awake()
    {
        movement = GetComponent<SimpleMovementAgent>();
        ai       = GetComponent<EnemyAIStateController>();
    }

    public void ApplyStun(float duration)
    {
        Debug.Log($"[{name}] Applying stun");
        // if (isStunned) return; // To avoid stunning effects again restarting the timer
        if (duration <= 0f)
            duration = defaultStunTime;

        if (stunRoutine != null)
            StopCoroutine(stunRoutine);

        stunRoutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;

        if (ai != null)
            ai.OnStunnedStart();

        if (movement != null)
            movement.SetMovementState(SimpleMovementAgent.MovementState.Idle);

        // TODO: animation

        yield return new WaitForSeconds(duration);

        isStunned = false;

        if (ai != null)
            ai.OnStunnedEnd();
    }
}