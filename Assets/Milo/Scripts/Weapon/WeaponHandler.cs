using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public SO_WeaponType CurrentWeaponData => currentWeaponData;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform firePoint;

    private AttackInput attackInput;
    private SO_WeaponType currentWeaponData;
    private bool isHoldingTrigger;
    private Coroutine firingCoroutine;
    private PerformAttack performAttack;
    private PlayerAimController aimController;
    private WeaponCooldown weaponCooldown;
    private GameObject currentWeaponPrefab;

    private int equippedWeaponIndex = -1;
    
    
    private InventoryData invData
    {
        get
        {
            if (DataController.Instance == null) return null;
            else { return DataController.Instance.InventoryRuntimeData.Value; }
        }
    }

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
    
    public void EquipWeapon(SO_WeaponType weaponData)
    {
        if (!weaponData)
            return;

        if (weaponData.WeaponModelPrefab && !firePoint)
        {
            Debug.LogError("WeaponHandler.cs: Tried to equip weapon but firePoint is missing");
            return;
        }

        // Maybe can be used later with inventory
        if (currentWeaponPrefab)
        {
            Destroy(currentWeaponPrefab);
        }
        
        currentWeaponData = weaponData;
        currentWeaponPrefab = weaponData.WeaponModelPrefab;

        AmmoModel.InitializeAmmo(weaponData);
        weaponCooldown.InitializeCooldown(weaponData.FireRate);
        performAttack.SetCurrentWeapon(weaponData);
        
        if (!currentWeaponPrefab)
            return;

        currentWeaponPrefab = Instantiate(weaponData.WeaponModelPrefab, firePoint.parent);
        currentWeaponPrefab.transform.SetParent(firePoint.parent, false);
        currentWeaponPrefab.transform.localPosition = Vector3.zero;
        currentWeaponPrefab.transform.localRotation = Quaternion.identity;
        currentWeaponPrefab.transform.localScale = Vector3.one;
    }

    private void EquipNextWeapon()
    {
        if (GetAvailableWeapons().Count > 0)
        {
            int numberOfWeapons = GetAvailableWeapons().Count;
            equippedWeaponIndex = (equippedWeaponIndex + 1) %  numberOfWeapons;

            string weaponId = GetAvailableWeapons()[equippedWeaponIndex];
            
            
        }
    }
    
    private void OnFireStarted()
    {
        if (!currentWeaponData) return;

        // Melee logic: Usually allowed even if not aiming
        if (currentWeaponData.AttackCategories == SO_WeaponType.AttackCategory.Melee)
        {
            TryMeleeAttack();
            return;
        }

        // Gun logic: Requires aiming
        if (!aimController.IsAiming) return;

        if (!currentWeaponData.IsSemiAutomatic)
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
        if (!currentWeaponData || !firePoint) return;
        if (!weaponCooldown.CanFire() || !currentWeaponData.HasAmmo) return;

        if (!AmmoModel.UseAmmo(1))
        {
            audioSource.PlayOneShot(currentWeaponData.DryFireSound);
            return;
        }

        performAttack.Execute();
        weaponCooldown.StartCooldown(currentWeaponData.FireRate);
    }
    
    private void TryMeleeAttack()
    {
        // Melee doesn't need to check for firePoint or Ammo
        if (!weaponCooldown.CanFire())
            return;

        Debug.Log("[WeaponHandler] Executing Melee Attack");
    
        performAttack.Execute();
    
        // Use the weapon's fireRate as the "swing speed" cooldown
        weaponCooldown.StartCooldown(currentWeaponData.FireRate);
    }

    private IEnumerator AutomaticFire()
    {
        while (isHoldingTrigger)
        {
            TryHitScanAttack();
            yield return new WaitForSeconds(currentWeaponData.FireRate);
        }
    }

    private List<string> GetAvailableWeapons()
    {
        if (invData == null) return IDConstants.GetAllWeapons();
        return invData.GetWeaponItemIDs();
    }
}
