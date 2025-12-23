using UnityEngine;

public interface IPerceptionSystem
{
    bool CanSeeTarget(Transform target);
    bool CanHearNoise(Vector3 position, float intensity);
    bool CanDetectBioelectric(Transform target);
}