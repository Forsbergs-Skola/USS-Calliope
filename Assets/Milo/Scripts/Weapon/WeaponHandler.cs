using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public SO_WeaponType CurrentWeaponData => currentWeaponData;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform firePoint;
    [SerializeField] private WeaponData weaponDatabase;

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
        attackInput.SwitchWeaponTriggered += EquipNextWeapon;

    }

    private void OnDisable()
    {
        attackInput.FireStarted -= OnFireStarted;
        attackInput.FireStopped -= OnFireStopped;
        attackInput.AimStarted -= OnAimStarted;
        attackInput.AimStopped -= OnAimStopped;
        attackInput.SwitchWeaponTriggered -= EquipNextWeapon;

    }

    private void EquipNextWeapon()
    {
        var availableWeapons = GetAvailableWeapons();
        if (availableWeapons.Count <= 0) return;

        // Increment index
        equippedWeaponIndex = (equippedWeaponIndex + 1) % availableWeapons.Count;
        var weaponId = availableWeapons[equippedWeaponIndex];

        // Get weapon data
        if (weaponDatabase == null)
        {
            Debug.LogWarning("[WeaponHandler] WeaponDatabase is missing!");
            return;
        }

        SO_WeaponType weaponData = weaponDatabase.GetWeapon(weaponId);
        if (weaponData == null) return;

        // Destroy old prefab
        if (currentWeaponPrefab != null)
        {
            Destroy(currentWeaponPrefab);
            currentWeaponPrefab = null;
        }

        // Initialize systems
        AmmoModel.InitializeAmmo(weaponData);
        weaponCooldown.InitializeCooldown(weaponData.FireRate);
        performAttack.SetCurrentWeapon(weaponData);

        // Spawn visuals
        if (weaponData.WeaponModelPrefab != null && firePoint != null)
        {
            currentWeaponPrefab = Instantiate(weaponData.WeaponModelPrefab, firePoint);
            currentWeaponPrefab.transform.localPosition = Vector3.zero;
            currentWeaponPrefab.transform.localRotation = Quaternion.identity;
            currentWeaponPrefab.transform.localScale = Vector3.one;

            Debug.Log("[WeaponHandler] Weapon spawned: " + currentWeaponPrefab.name);
        }
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
