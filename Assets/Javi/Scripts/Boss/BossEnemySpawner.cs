using UnityEngine;

public class BossEnemySpawner : MonoBehaviour
{
    public GameObject activeEnemyPrefab;
    public GameObject latentEnemyPrefab;

    public void SpawnEnemyFacingPlayer(
        GameObject prefab,
        Vector3 spawnPos,
        Vector3 playerPos
    )
    {
        if (prefab == null)
            return;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        Vector3 dir = playerPos - spawnPos;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.01f)
            enemy.transform.rotation = Quaternion.LookRotation(dir);
    }
}