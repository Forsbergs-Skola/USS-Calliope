using TMPro;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float verticalOffset = 2f;

    private IDamageEvents damageSource;

    private void Awake()
    {
        damageSource = GetComponent<IDamageEvents>();

        if (spawnPoint == null)
            spawnPoint = transform;
    }

    private void OnEnable()
    {
        if (damageSource != null)
            damageSource.OnDamaged += SpawnDamageText;
    }

    private void OnDisable()
    {
        if (damageSource != null)
            damageSource.OnDamaged -= SpawnDamageText;
    }

    private void SpawnDamageText(float damage)
    {
        Vector3 spawnPosition = spawnPoint.position + Vector3.up * verticalOffset;
        
        GameObject instance = Instantiate(
            damageTextPrefab,
            spawnPosition,
            Quaternion.identity
        );

        var text = instance.GetComponentInChildren<TextMeshPro>();
        text.SetText("-" + damage.ToString("0")+" Damage!");

        if (Camera.main != null)
        {
            instance.transform.forward = Camera.main.transform.forward;
        }
    }
}