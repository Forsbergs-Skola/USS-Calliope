using System.Collections;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public SO_WeaponType CurrentWeapon => currentWeapon;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform firePoint;

    private AttackInput attackInput;
    private SO_WeaponType currentWeapon;
    private bool isHoldingTrigger;
    private Coroutine firingCoroutine;
    private PerformAttack performAttack;
    private PlayerAimController aimController;
    private WeaponCooldown weaponCooldown;
    private GameObject currentWeaponPrefab;

    public AmmoModel AmmoModel { get; private set; }
    
    private void Awake()
    {
        weaponCooldown = GetComponent<WeaponCooldown>();
        attackInput = GetComponent<AttackInput>();
        aimController = GetComponent<PlayerAimController>();
        performAttack = GetComponent<PerformAttack>();

        AmmoModel = new AmmoModel();
    }

    private void Start()
    {
        attackInput.FireStarted += OnFireStarted;
        attackInput.FireStopped += OnFireStopped;
        attackInput.AimStarted += OnAimStarted;
        attackInput.AimStopped += OnAimStopped;
    }

    private void OnDisable()
    {
        attackInput.FireStarted -= OnFireStarted;
        attackInput.FireStopped -= OnFireStopped;
        attackInput.AimStarted -= OnAimStarted;
        attackInput.AimStopped -= OnAimStopped;
    }
    
    public void EquipWeapon(SO_WeaponType equippedWeapon, GameObject weaponPrefab = null)
    {
        if (!equippedWeapon)
            return;

        if (weaponPrefab && !firePoint)
        {
            Debug.LogError("WeaponHandler.cs: Tried to equip weapon but firePoint is missing");
            return;
        }

        if (currentWeaponPrefab)
            Destroy(currentWeaponPrefab);
        
        currentWeapon = equippedWeapon;
        currentWeaponPrefab = weaponPrefab;

        AmmoModel.InitializeAmmo(equippedWeapon);
        weaponCooldown.InitializeCooldown(equippedWeapon.FireRate);
        performAttack.SetCurrentWeapon(equippedWeapon);
        
        if (!currentWeaponPrefab)
            return;

        currentWeaponPrefab.transform.SetParent(firePoint.parent, false);
        currentWeaponPrefab.transform.localPosition = Vector3.zero;
        currentWeaponPrefab.transform.localRotation = Quaternion.identity;
        currentWeaponPrefab.transform.localScale = Vector3.one;
    }
    
    private void OnFireStarted()
    {
        if (!currentWeapon) return;

        // Melee logic: Usually allowed even if not aiming
        if (currentWeapon.AttackCategories == SO_WeaponType.AttackCategory.Melee)
        {
            TryMeleeAttack();
            return;
        }

        // Gun logic: Requires aiming
        if (!aimController.IsAiming) return;

        if (!currentWeapon.IsSemiAutomatic)
        {
            if (firingCoroutine != null) return;
            isHoldingTrigger = true;
            firingCoroutine = StartCoroutine(AutomaticFire());
        }
        else
        {
            TryHitScanAttack();
        }
    }

    private void OnFireStopped()
    {
        isHoldingTrigger = false;
        if (firingCoroutine == null) return;
        StopCoroutine(firingCoroutine);
        firingCoroutine = null;
    }

    private void OnAimStarted()
    {
    }

    private void OnAimStopped()
    {
    }

    private void TryHitScanAttack()
    {
        if (!currentWeapon || !firePoint) return;
        if (!weaponCooldown.CanFire() || !currentWeapon.HasAmmo) return;

        if (!AmmoModel.UseAmmo(1))
        {
            audioSource.PlayOneShot(currentWeapon.DryFireSound);
            return;
        }

        performAttack.Execute();
        weaponCooldown.StartCooldown(currentWeapon.FireRate);
    }
    
    private void TryMeleeAttack()
    {
        // Melee doesn't need to check for firePoint or Ammo
        if (!weaponCooldown.CanFire())
            return;

        Debug.Log("[WeaponHandler] Executing Melee Attack");
    
        performAttack.Execute();
    
        // Use the weapon's fireRate as the "swing speed" cooldown
        weaponCooldown.StartCooldown(currentWeapon.FireRate);
    }

    private IEnumerator AutomaticFire()
    {
        while (isHoldingTrigger)
        {
            TryHitScanAttack();
            yield return new WaitForSeconds(currentWeapon.FireRate);
        }
    }
}
