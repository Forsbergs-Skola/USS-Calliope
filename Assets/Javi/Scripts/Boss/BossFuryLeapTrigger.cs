using UnityEngine;

[RequireComponent(typeof(BossHealth))]
public class BossFuryLeapTrigger : MonoBehaviour
{
    [SerializeField] private float damageThreshold = 20f;

    private float accumulatedDamage;
    private bool leapAvailable;

    private BossHealth health;

    private void Awake()
    {
        health = GetComponent<BossHealth>();
        health.OnDamageTaken.AddListener(OnDamageTaken);
    }

    private void OnDamageTaken(float damage)
    {
        accumulatedDamage += damage;

        if (accumulatedDamage >= damageThreshold)
        {
            accumulatedDamage = 0;
            leapAvailable = true;
        }
    }

    public bool ConsumeLeapPermission()
    {
        if (!leapAvailable) return false;

        leapAvailable = false;
        return true;
    }
}