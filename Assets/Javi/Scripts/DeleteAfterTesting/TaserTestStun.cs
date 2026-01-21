using UnityEngine;
using UnityEngine.InputSystem;

public class TaserTestStun : MonoBehaviour
{
    /*[Header("Test Settings")]
    [SerializeField] private KeyCode stunKey = KeyCode.J;*/
    [SerializeField] private float stunRadius = 20f;
    [SerializeField] private LayerMask enemyLayer;   // Asigna la capa "Enemy" en el inspector
    [SerializeField] private float stunDurationOverride = 0f;
    // 0 = usar la duración por defecto del EnemyStunController
    
    private InputAction stunAction;

    private void Awake()
    {
        stunAction = new InputAction("TestStun", InputActionType.Button, "<Keyboard>/j");
    }

    private void OnEnable()
    {
        stunAction.Enable();
    }

    private void OnDisable()
    {
        stunAction.Disable();
    }

    private void Update()
    {
        if (stunAction.WasPressedThisFrame())
        {
            Debug.Log("[TaserTestStun] J pressed");
            TestStunNearbyEnemies();
        }
    }

    private void TestStunNearbyEnemies()
    {
        Vector3 playerPos = transform.position;

        // Buscar colliders en un radio alrededor del player solo en la capa de enemigos
        Collider[] hits = Physics.OverlapSphere(playerPos, stunRadius, enemyLayer);

        foreach (var hit in hits)
        {
            EnemyStunController stun = hit.GetComponentInParent<EnemyStunController>();
            if (stun != null)
            {
                // Si stunDurationOverride > 0, usamos esa duración; si no, la interna
                float duration = stunDurationOverride > 0f ? stunDurationOverride : -1f;
                stun.ApplyStun(duration);
            }
        }

        Debug.Log($"[TaserTestStun] Stunned {hits.Length} enemies within {stunRadius} meters");
    }

#if UNITY_EDITOR
    // Gizmo para ver el radio en la escena
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stunRadius);
    }
#endif
}