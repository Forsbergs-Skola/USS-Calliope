using UnityEngine;

public static class NamedEnemyKilledHandler
{
    public static void AddNameToList(string enemyName)
    {
        if (DataController.Instance == null) return;
        DataController.Instance.ProgressionRuntimeData.Value.DefeatEnemy(enemyName);
    }
}
