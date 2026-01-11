using UnityEngine;

public interface IEnemyChaseController
{
    void StartChase(Transform target);
    void StopChase();
}