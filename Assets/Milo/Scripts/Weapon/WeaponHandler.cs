using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections;

public class PlayerWeaponHandler : MonoBehaviour
{
   
    [SerializeField] private InputActionReference shootAction; 
    [SerializeField] private InputActionReference mousePositionAction; // <--- ADD THIS
    
    private AmmoModel ammoModel;
    private SO_WeaponType currentWeapon;
    private bool canShoot = true; 
    
    [Header("Raycasting")]
    [SerializeField] private float maxDistance = 100f; 
    [SerializeField] private LayerMask hitMask;        
    [SerializeField] private Transform firePoint;      
    
    [Header("Isometric Aiming")]
    [SerializeField] private LayerMask groundMask;
    private Camera mainCamera;

    void Awake()
    {
        ammoModel = new AmmoModel();
        ammoModel.AmmoChanged += OnAmmoChanged;
        ammoModel.OnError += OnAmmoError;

        if (shootAction != null && shootAction.action != null)
        {
            shootAction.action.Enable();
            shootAction.action.performed += OnShootInput; 
        }
        else
        {
            Debug.LogError("Shoot Action Reference is missing or invalid.");
        }
        
        // --- NEW: Enable the Mouse Position Action ---
        if (mousePositionAction != null && mousePositionAction.action != null)
        {
            mousePositionAction.action.Enable();
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("FATAL: Main Camera tag not found on any active Camera. Isometric aiming will fail.");
        }
    }

    void OnDestroy()
    {
        if (shootAction != null && shootAction.action != null)
        {
            shootAction.action.performed -= OnShootInput;
            shootAction.action.Disable();
        }
        if (mousePositionAction != null && mousePositionAction.action != null)
        {
             mousePositionAction.action.Disable();
        }
    }

    private void OnShootInput(InputAction.CallbackContext context)
    {
        if (canShoot)
        {
            TryShoot();
        }
    }
    
    public void EquipWeapon(SO_WeaponType weapon)
    {
        if (weapon == null || weapon == currentWeapon)
            return;

        currentWeapon = weapon;
        ammoModel.Initialize(weapon);
        Debug.Log($"Equipped: {weapon.WeaponId}");
    }

    public void TryShoot()
    {
        if (firePoint == null || mainCamera == null)
        {
            Debug.LogError("FATAL ERROR: FirePoint or Camera reference is missing. Check Inspector/Tags.");
            return;
        }

        if (currentWeapon == null)
        {
            Debug.Log("Click! No weapon equipped.");
            return;
        }
        
        const int ammoPerShot = 1; 

        if (ammoModel.UseAmmo(ammoPerShot))
        {
            FireWeapon();
            StartCoroutine(FireRateCooldown(currentWeapon.FireRate));
        }
        else
        {
            Debug.Log($"Click! {currentWeapon.WeaponId} out of ammo.");
        }
    }

    private void FireWeapon()
    {
        Vector3 origin = firePoint.position;
        Vector3 finalDirection; 

        if (GetMouseWorldPositionOnGround(out Vector3 targetPosition))
        {
            finalDirection = (targetPosition - origin).normalized;
        }
        else
        {
            Debug.LogWarning("Mouse not over ground plane. Using direct forward.");
            finalDirection = firePoint.forward; 
        }

        RaycastHit hit;

        if (Physics.Raycast(origin, finalDirection, out hit, maxDistance, hitMask))
        {
            Debug.Log($"Hit: {hit.collider.name} at {hit.point} | Dist: {hit.distance:F2}m");
            
            HandleHit(hit.collider.gameObject, hit.point);

            Debug.DrawLine(origin, hit.point, Color.red, 0.1f);
        }
        else
        {
            Debug.Log("Shot missed everything.");
            Debug.DrawLine(origin, origin + finalDirection * maxDistance, Color.yellow, 0.1f);
        }

        Debug.Log($"Fired {currentWeapon.WeaponId}. Ammo: {ammoModel.CurrentAmmo}");
    }

    private bool GetMouseWorldPositionOnGround(out Vector3 worldPosition)
    {
        // --- FIX: Read mouse position from the new Input System Action ---
        Vector2 mouseScreenPosition = mousePositionAction.action.ReadValue<Vector2>();
        
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance * 2, groundMask)) 
        {
            worldPosition = hit.point;
            return true;
        }

        worldPosition = Vector3.zero;
        return false;
    }
    
    private void HandleHit(GameObject hitObject, Vector3 hitPosition)
    {
        if (hitObject.TryGetComponent<EnemyHealthPC>(out var healthComponent))
        {
            healthComponent.TakeDamage(currentWeapon.WeaponDamage);
            Debug.Log($"Damage dealt: {currentWeapon.WeaponDamage} to {hitObject.name}");
        }    
    }

    private IEnumerator FireRateCooldown(float waitTime)
    {
        canShoot = false;
        yield return new WaitForSeconds(waitTime);
        canShoot = true;
    }

    private void OnAmmoChanged(int current, int max)
    {
        Debug.Log($"Ammo: {current}/{max}");
    }

    private void OnAmmoError(string message)
    {
        Debug.LogWarning(message);
    }

    public SO_WeaponType CurrentWeapon => currentWeapon;
    public AmmoModel AmmoModel => ammoModel;
}