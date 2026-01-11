using UnityEngine;

public interface IEnemyPatrol
{
    void Initialize();
    void UpdatePatrol();
    void SetAlarmZone(PatrolZone zone);
    void ClearAlarmZone();
    void SetPlayerVisible(bool visible, Transform player = null);
}