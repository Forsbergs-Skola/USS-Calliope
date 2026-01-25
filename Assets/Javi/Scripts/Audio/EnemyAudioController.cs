using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyAIStateController))]
public class EnemyAudioController : MonoBehaviour
{
    [Header("Idle Clips")]
    [SerializeField] private AudioClip[] wanderingIdleClips;
    [SerializeField] private AudioClip[] watchfulIdleClips;

    [Header("Idle Timing")]
    [SerializeField] private float idleMinDelay = 10f;
    [SerializeField] private float idleMaxDelay = 30f;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource idleSource;
    [SerializeField] private AudioSource watchfulSource;
    [SerializeField] private AudioSource battleCrySource;

    [Header("Battle Cry")]
    [SerializeField] private AudioClip battleCryClip;

    private EnemyAIStateController ai;
    private Coroutine idleRoutine;
    private EnemyAIStateController.State lastState;

    private void Awake()
    {
        ai = GetComponent<EnemyAIStateController>();
    }

    private void OnEnable()
    {
        lastState = ai.CurrentState;
        StartIdleForState(lastState);
    }

    private void Update()
    {
        if (ai.CurrentState == lastState)
            return;

        OnStateChanged(ai.CurrentState);
        lastState = ai.CurrentState;
    }

    private void OnStateChanged(EnemyAIStateController.State newState)
    {
        StopIdle();

        switch (newState)
        {
            case EnemyAIStateController.State.Wandering:
                StartIdleLoop(idleSource, wanderingIdleClips);
                break;

            case EnemyAIStateController.State.Watchful:
                StartIdleLoop(watchfulSource, watchfulIdleClips);
                break;

            case EnemyAIStateController.State.Attacking:
            case EnemyAIStateController.State.Dead:
                // no idle
                break;
        }
    }

    private void StartIdleForState(EnemyAIStateController.State state)
    {
        OnStateChanged(state);
    }

    private void StartIdleLoop(AudioSource source, AudioClip[] clips)
    {
        if (source == null || clips == null || clips.Length == 0)
            return;

        idleRoutine = StartCoroutine(IdleLoop(source, clips));
    }

    private IEnumerator IdleLoop(AudioSource source, AudioClip[] clips)
    {
        while (true)
        {
            float delay = Random.Range(idleMinDelay, idleMaxDelay);
            yield return new WaitForSeconds(delay);

            if (ai.IsDead) yield break;
            if (ai.CurrentState != lastState) yield break;

            AudioClip clip = clips[Random.Range(0, clips.Length)];
            source.PlayOneShot(clip);
        }
    }

    private void StopIdle()
    {
        if (idleRoutine != null)
        {
            StopCoroutine(idleRoutine);
            idleRoutine = null;
        }

        if (idleSource != null) idleSource.Stop();
        if (watchfulSource != null) watchfulSource.Stop();
    }

    // Calling enemies
    public void PlayBattleCry()
    {
        if (battleCrySource == null || battleCryClip == null)
            return;

        if (ai.IsDead)
            return;

        battleCrySource.PlayOneShot(battleCryClip);
    }

    public void StopAllAudio()
    {
        StopIdle();
        if (battleCrySource != null)
            battleCrySource.Stop();
    }
}
