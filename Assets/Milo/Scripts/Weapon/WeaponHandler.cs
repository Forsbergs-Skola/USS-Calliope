using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private WeaponCooldown cooldown;
    [SerializeField] private HitscanWeaponFire weaponFire; 
    [SerializeField] private InputActionReference shootAction;

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

        weaponFire.SetWeapon(newWeapon); 
    }

    public void TryShoot()
    {
        if (currentWeapon == null) return;
        if (!cooldown.CanFire()) return;

        const int ammoPerShot = 1;
        if (!ammoModel.UseAmmo(ammoPerShot))
        {
            Debug.Log($"Click! {currentWeapon.WeaponId} out of ammo.");
            return;
        }

        weaponFire.Fire(); 
        cooldown.StartCooldown(currentWeapon.FireRate);
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
