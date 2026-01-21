using UnityEngine;

public class BossEnemySpawner : MonoBehaviour
{
    public GameObject activeEnemyPrefab;
    public GameObject latentEnemyPrefab;

    public void Spawn(Vector3 playerPos, bool playerInvisible)
    {
        SpawnEnemy(activeEnemyPrefab, playerPos, playerInvisible);
        SpawnEnemy(latentEnemyPrefab, playerPos, playerInvisible);
        SpawnEnemy(latentEnemyPrefab, playerPos, playerInvisible);
    }

    private void SpawnEnemy(GameObject prefab, Vector3 lookAt, bool invisible)
    {
        var enemy = Instantiate(prefab, transform.position + Random.insideUnitSphere * 2f, Quaternion.identity);
        enemy.transform.LookAt(lookAt);

        if (invisible)
        {
            //enemy.GetComponent<EnemyAIStateController>()?.ForceWatchful();
        }
    }
}