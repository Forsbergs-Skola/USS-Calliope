using UnityEngine;

public class BossEnemySpawner : MonoBehaviour
{
    public GameObject activeEnemyPrefab;
    public GameObject latentEnemyPrefab;

    public void Spawn(Vector3 playerPos, bool playerInvisible)
    {
        var points = PatrolPointRegistry.GetClosestPoints(transform.position, 3);

        if (points.Count < 3) return;
        
        SpawnEnemy(activeEnemyPrefab, points[0].position);
        SpawnEnemy(latentEnemyPrefab, points[1].position);
        SpawnEnemy(latentEnemyPrefab, points[2].position);
    }

    private void SpawnEnemy(GameObject prefab, Vector3 pos)
    {
        var enemy = Instantiate(prefab, pos, Quaternion.identity);
    }
}