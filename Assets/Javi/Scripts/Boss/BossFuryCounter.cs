using UnityEngine;

[RequireComponent(typeof(BossHealth))]
public class BossFuryCounter : MonoBehaviour
{
    [SerializeField] private float damageThreshold = 20f;

    private float accumulatedDamage;
    private bool leapReady;
    public bool IsLeapReady => leapReady;

    private void Awake()
    {
        GetComponent<BossHealth>().OnDamageTaken.AddListener(OnDamageTaken);
    }

    private void OnDamageTaken(float dmg)
    {
        accumulatedDamage += dmg;

        if (accumulatedDamage >= damageThreshold)
        {
            accumulatedDamage = 0f;
            leapReady = true;
        }
    }

    public bool ConsumeLeap()
    {
        if (!leapReady) return false;

        leapReady = false;
        return true;
    }
}