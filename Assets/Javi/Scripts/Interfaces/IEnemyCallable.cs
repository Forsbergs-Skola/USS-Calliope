using UnityEngine;

public interface IEnemyCallable
{
    EnemyRank Rank { get; }
    void ReceiveCall(PatrolZone callZone, Vector3 callPosition);
}