using UnityEngine;
using UnityEngine.InputSystem;

public class DamageEnemyOnKey : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private EnemyHealth targetEnemy;

    [Header("Damage")]
    [SerializeField] private float damageAmount = 10f;

    [Header("Input")]
    [SerializeField] private InputAction damageAction;

    private void OnEnable()
    {
        damageAction.Enable();
        damageAction.performed += OnDamagePerformed;
    }

    private void OnDisable()
    {
        damageAction.performed -= OnDamagePerformed;
        damageAction.Disable();
    }

    private void OnDamagePerformed(InputAction.CallbackContext context)
    {
        if (targetEnemy != null)
        {
            targetEnemy.TakeDamage(damageAmount);
        }
    }
}