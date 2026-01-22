using UnityEngine;
using Olle.Scripts;

public class BossPerception : MonoBehaviour, IPerceptionSystem
{
    private Transform player;
    private CrouchInvisibility invis;
    private EnemyVisionCone visionCone;

    private void Awake()
    {
        visionCone = GetComponent<EnemyVisionCone>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
            invis = player.GetComponent<CrouchInvisibility>();
    }

    public bool CanSeeTarget(Transform target)
    {
        if (target == null) return false;
        if (invis != null && invis.IsInvisible) return false;
        return visionCone.CanSeePlayer();
    }

    public Vector3 GetLastKnownZone()
    {
        return player.position;
    }
    
    public bool CanHearNoise(Vector3 position, float intensity)
    {
        return Vector3.Distance(transform.position, position) <= intensity;
    }

    public bool CanDetectBioelectric(Transform target)
    {
        // Future suit system can reduce this
        return Vector3.Distance(transform.position, target.position) < 6f;
    }
}