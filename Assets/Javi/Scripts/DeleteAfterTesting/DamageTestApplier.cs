using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class DamageTestApplier : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private float damageAmount = 10f;

    [Header("Input")]
    [SerializeField] private InputActionReference damageAction;

    private IDamageable[] damageables;

    private void Awake()
    {
        damageables = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDamageable>()
            .ToArray();

        if (damageAction != null)
            damageAction.action.Enable();

        Debug.Log($"[DamageTest] Found {damageables.Length} IDamageable");
    }

    private void OnDestroy()
    {
        if (damageAction != null)
            damageAction.action.Disable();
    }

    private void Update()
    {
        if (damageAction != null && damageAction.action.WasPressedThisFrame())
        {
            ApplyDamageToAll();
        }
    }

    private void ApplyDamageToAll()
    {
        foreach (var dmg in damageables)
        {
            dmg.TakeDamage(damageAmount);
        }

        Debug.Log($"[DamageTest] Applied {damageAmount} damage");
    }
}