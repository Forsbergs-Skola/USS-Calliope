using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private PlayerAimController aimController;
    [SerializeField] private WeaponCooldown cooldown;
    [SerializeField] private ImpactProcessor impactProcessor;
    [SerializeField] private Transform firePoint;

    [Header("Input")] [SerializeField] private InputActionReference shootAction;

    private AmmoModel ammoModel;
    private SO_WeaponType currentWeapon;

    public AmmoModel AmmoModel => ammoModel;

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
    }

    void OnDestroy()
    {
        if (shootAction != null && shootAction.action != null)
        {
            shootAction.action.performed -= OnShootInput;
            shootAction.action.Disable();
        }
    }

    private void OnShootInput(InputAction.CallbackContext context)
    {
        TryShoot();
    }

    public void EquipWeapon(SO_WeaponType newWeapon)
    {
        if (newWeapon == null)
        {
            Debug.LogWarning("Attempted to equip null weapon.");
            return;
        }

        if (newWeapon == currentWeapon)
            return;

        currentWeapon = newWeapon;
        Debug.Log($"Equipped: {newWeapon.WeaponId}");


        ammoModel.Initialize(newWeapon);

        cooldown.InitializeCooldown(newWeapon.FireRate);

        impactProcessor.InitializeProcessor(newWeapon);
    }

    public void TryShoot()
    {
        if (currentWeapon == null || firePoint == null) return;

        if (!cooldown.CanFire()) return;

        const int ammoPerShot = 1;
        if (!ammoModel.UseAmmo(ammoPerShot))
        {
            Debug.Log($"Click! {currentWeapon.WeaponId} out of ammo.");
            return;
        }

        FireWeapon();
        cooldown.StartCooldown(currentWeapon.FireRate);
    }

    private void FireWeapon()
    {
        Vector3 origin = firePoint.position;
        Vector3 finalDirection;

        if (aimController.TryGetAimDirection(origin, out finalDirection))
        {
            finalDirection = BallisticsUtility.GetGaussianSpread(finalDirection,currentWeapon.SpreadStandardDeviation);
            
            RaycastHit hit;

            if (Physics.Raycast(origin, finalDirection, out hit, currentWeapon.ImpactRange, impactProcessor.HitMask))
            {
                impactProcessor.ProcessHit(hit);
                Debug.DrawLine(origin, hit.point, Color.red, 0.1f);
            }
            else
            {
                Debug.Log("Shot missed everything.");
                Debug.DrawLine(origin, origin + finalDirection * currentWeapon.ImpactRange, Color.yellow, 0.1f);
            }
        }
    }

    private void OnAmmoChanged(int current, int max)
    {
        Debug.Log($"Ammo: {current}/{max}");
    }

    private void OnAmmoError(string message)
    {
        Debug.LogWarning(message);
    }
}