using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BossHealth))]
public class BossSpawnOnLowHealth : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private BossEnemySpawner spawner;
    [SerializeField] private float triggerHealth = 35f;

    [Header("Battle Cry")]
    [SerializeField] private int minBattleCries = 3;
    [SerializeField] private float cryInterval = 1.2f;

    private BossHealth health;
    private EnemyAudioController audioController;
    private Transform player;

    private bool hasTriggered = false;

    private void Awake()
    {
        health = GetComponent<BossHealth>();
        audioController = GetComponent<EnemyAudioController>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        health.OnHealthChanged.AddListener(OnHealthChanged);
    }

    private void OnHealthChanged(float current, float max)
    {
        if (hasTriggered)
            return;

        if (current <= triggerHealth)
        {
            hasTriggered = true;
            ExecuteSummon();
        }
    }

    private void ExecuteSummon()
    {
        if (spawner == null || player == null)
        {
            Debug.LogWarning("[BossSummonOnLowHealth] Missing references");
            return;
        }

        SpawnEnemiesFacingPlayer();
        StartCoroutine(BattleCryRoutine());
    }

    private void SpawnEnemiesFacingPlayer()
    {
        var points = PatrolPointRegistry.GetClosestPoints(transform.position, 3);

        if (points.Count < 3)
        {
            Debug.LogWarning("[BossSummonOnLowHealth] Not enough patrol points");
            return;
        }

        spawner.SpawnEnemyFacingPlayer(
            spawner.activeEnemyPrefab,
            points[0].position,
            player.position
        );

        spawner.SpawnEnemyFacingPlayer(
            spawner.latentEnemyPrefab,
            points[1].position,
            player.position
        );

        spawner.SpawnEnemyFacingPlayer(
            spawner.latentEnemyPrefab,
            points[2].position,
            player.position
        );
    }

    private IEnumerator BattleCryRoutine()
    {
        if (audioController == null)
            yield break;

        for (int i = 0; i < minBattleCries; i++)
        {
            audioController.PlayBattleCry();
            yield return new WaitForSeconds(cryInterval);
        }
    }
}