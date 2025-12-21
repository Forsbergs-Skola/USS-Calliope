using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerAimController aimController;
    [SerializeField] private HitscanWeaponFire weaponFire;
    [SerializeField] private WeaponCooldown cooldown;
    [SerializeField] private AudioSource audioSource;

    [Header("Input References")]
    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private InputActionReference aimAction;

    private AmmoModel ammoModel;
    private SO_WeaponType currentWeapon;

    private bool isAiming;

    public AmmoModel AmmoModel => ammoModel;

    private void Awake()
    {
        if (aimController == null)
            aimController = GetComponent<PlayerAimController>();

        ammoModel = new AmmoModel();
        ammoModel.AmmoChanged += OnAmmoChanged;
        ammoModel.OnError += OnAmmoError;

        // Aim Input
        if (aimAction?.action != null)
        {
            aimAction.action.Enable();
            aimAction.action.performed += OnAimStarted;
            aimAction.action.canceled += OnAimStopped;
        }

        // Shoot Input (ALWAYS enabled)
        if (shootAction?.action != null)
        {
            shootAction.action.Enable();
            shootAction.action.performed += OnShootInput;
        }
    }

    private void OnAimStarted(InputAction.CallbackContext context)
    {
        isAiming = true;
    }

    private void OnAimStopped(InputAction.CallbackContext context)
    {
        isAiming = false;
    }

    private void OnShootInput(InputAction.CallbackContext context)
    {
        if (!isAiming)
            return;

        TryShoot();
    }

    public void TryShoot()
    {
        if (currentWeapon == null)
            return;

        if (!cooldown.CanFire())
            return;

        if (!ammoModel.UseAmmo(1))
        {
            Debug.Log($"Click! {currentWeapon.WeaponId} out of ammo.");
            audioSource.PlayOneShot(currentWeapon.DryFireSounds);
            return;
        }

        weaponFire.Fire();
        cooldown.StartCooldown(currentWeapon.FireRate);
    }

    public void EquipWeapon(SO_WeaponType equippedWeapon)
    {
        if (equippedWeapon == null || equippedWeapon == currentWeapon)
            return;

        currentWeapon = equippedWeapon;

        ammoModel.Initialize(equippedWeapon);
        cooldown.InitializeCooldown(equippedWeapon.FireRate);
        weaponFire.SetWeapon(equippedWeapon);
    }

    private void OnDestroy()
    {
        if (aimAction?.action != null)
        {
            aimAction.action.performed -= OnAimStarted;
            aimAction.action.canceled -= OnAimStopped;
            aimAction.action.Disable();
        }

        if (shootAction?.action != null)
        {
            shootAction.action.performed -= OnShootInput;
            shootAction.action.Disable();
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
