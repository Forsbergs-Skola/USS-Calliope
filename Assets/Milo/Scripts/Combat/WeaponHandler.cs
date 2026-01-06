using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public SO_WeaponType CurrentWeaponData => currentWeaponData;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SO_WeaponList weaponDatabase;

    private AttackInput attackInput;
    private SO_WeaponType currentWeaponData;
    private bool isHoldingTrigger;
    private Coroutine firingCoroutine;
    private PerformAttack performAttack;
    private PlayerAimController aimController;
    private WeaponCooldown weaponCooldown;
    private GameObject currentWeaponPrefab;
    private Coroutine reloadCoroutine;

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
        attackInput.SwitchWeaponTriggered += EquipNextWeapon;
        attackInput.ReloadTriggered += TryReload;

    }

    private void OnDisable()
    {
        attackInput.FireStarted -= OnFireStarted;
        attackInput.FireStopped -= OnFireStopped;
        attackInput.AimStarted -= OnAimStarted;
        attackInput.AimStopped -= OnAimStopped;
        attackInput.SwitchWeaponTriggered -= EquipNextWeapon;
        attackInput.ReloadTriggered -= TryReload;

    }

    private void EquipNextWeapon()
    {
       
        if (currentWeaponData != null && AmmoModel.CurrentAmmo > 0)
        {
            Debug.Log("Returning ammo to inventory");
            invData.ReplenishConsumable(currentWeaponData.AmmoType.AmmoID, AmmoModel.CurrentAmmo);
        }
        
        var availableWeapons = GetAvailableWeapons();
        if (availableWeapons.Count <= 0) return;

        equippedWeaponIndex = (equippedWeaponIndex + 1) % availableWeapons.Count;
        var weaponId = availableWeapons[equippedWeaponIndex];

        if (weaponDatabase == null)
        {
            Debug.LogWarning("[WeaponHandler] WeaponDatabase is missing!");
            return;
        }

        SO_WeaponType weaponData = weaponDatabase.GetWeapon(weaponId);
        if (weaponData == null) return;

        currentWeaponData = weaponData;

        if (currentWeaponPrefab != null)
        {
            Destroy(currentWeaponPrefab);
            currentWeaponPrefab = null;
        }

        AmmoModel.InitializeAmmo(weaponData);
        weaponCooldown.InitializeCooldown(weaponData.FireRate);
        performAttack.SetCurrentWeapon(weaponData);

        if (weaponData.WeaponModelPrefab == null || firePoint == null) return;
        currentWeaponPrefab = Instantiate(weaponData.WeaponModelPrefab, firePoint);
        currentWeaponPrefab.transform.localPosition = Vector3.zero;
        currentWeaponPrefab.transform.localRotation = Quaternion.identity;
        currentWeaponPrefab.transform.localScale = Vector3.one;

        Debug.Log("[WeaponHandler] Weapon spawned: " + currentWeaponPrefab.name);
    }

    
    private void OnFireStarted()
    {
        if (!currentWeaponData) return;

        if (currentWeaponData.AttackCategories == SO_WeaponType.AttackCategory.Melee)
        {
            TryMeleeAttack();
            return;
        }

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
        if (!weaponCooldown.CanFire())
            return;

        Debug.Log("[WeaponHandler] Executing Melee Attack");
    
        performAttack.Execute();
    
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

    private void TryReload()
    {
        if (!currentWeaponData || !currentWeaponData.HasAmmo) return;
        if (reloadCoroutine != null) return; // already reloading

        
        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }
    
    private IEnumerator ReloadRoutine()
    {
        Debug.Log("[WeaponHandler] Reloading " + currentWeaponData.DisplayName);

        yield return new WaitForSeconds(currentWeaponData.ReloadTime);

        string ammoID = currentWeaponData.AmmoType.AmmoID;

        if (invData.GetConsumableIDsAndQuantities().TryGetValue(ammoID, out int ammoAvailable) && ammoAvailable > 0)
        {
            var ammoNeeded = AmmoModel.MaxAmmo - AmmoModel.CurrentAmmo;
            var ammoToLoad = Mathf.Min(ammoAvailable, ammoNeeded);

            // Delete ammo from inventory and add ammo to weapon //
            
            invData.DepleteConsumable(ammoID, ammoToLoad);
            AmmoModel.AddAmmo(AmmoModel.CurrentAmmoType, ammoToLoad);

            Debug.Log($"Reloaded {ammoToLoad} ammo into {currentWeaponData.DisplayName}");
        }
        else
        {
            Debug.Log("[WeaponHandler] No ammo available to reload!");
        }

        reloadCoroutine = null;
    }

}
