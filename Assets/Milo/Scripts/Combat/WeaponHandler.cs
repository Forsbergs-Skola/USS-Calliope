using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public SO_WeaponType CurrentWeaponData => currentWeaponData;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SO_WeaponList weaponDatabase;
    [SerializeField] private Transform rightHand; // Defined this so the weapon has a parent!

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
        var availableWeapons = GetAvailableWeapons();
        if (availableWeapons.Count == 0) return;

        if (currentWeaponData != null && AmmoModel.CurrentAmmo > 0)
        {
            invData.ReplenishConsumable(currentWeaponData.AmmoType.AmmoID, AmmoModel.CurrentAmmo);
        }

        equippedWeaponIndex = (equippedWeaponIndex + 1) % availableWeapons.Count;
        string weaponId = availableWeapons[equippedWeaponIndex];

        if (!weaponDatabase) return;
        SO_WeaponType weaponData = weaponDatabase.GetWeapon(weaponId);
        if (weaponData == null) return;

        currentWeaponData = weaponData;

        if (currentWeaponPrefab != null) Destroy(currentWeaponPrefab);

        ApplyWeaponSetup(weaponData);

        if (invData.GetConsumableIDsAndQuantities().TryGetValue(weaponData.AmmoType.AmmoID, out int ammoAvailable) && ammoAvailable > 0)
        {
            invData.DepleteConsumable(weaponData.AmmoType.AmmoID, weaponData.MagSize);
            AmmoModel.AddAmmo(weaponData.AmmoType, weaponData.MagSize);
        }
    }

    private void ApplyWeaponSetup(SO_WeaponType data)
    {
        AmmoModel.InitializeAmmo(data);
        weaponCooldown.InitializeCooldown(data.FireRate);
        performAttack.SetCurrentWeapon(data);

        if (data.WeaponModelPrefab != null && firePoint != null)
        {
            currentWeaponPrefab = Instantiate(data.WeaponModelPrefab, firePoint);
            currentWeaponPrefab.transform.localPosition = Vector3.zero;
            currentWeaponPrefab.transform.localRotation = Quaternion.identity;
            currentWeaponPrefab.transform.localScale = Vector3.one;
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

    private void OnAimStarted() { }

    private void OnAimStopped() { }

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
        if (!weaponCooldown.CanFire()) return;
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
        if (reloadCoroutine != null) return;
        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(currentWeaponData.ReloadTime);
        string ammoID = currentWeaponData.AmmoType.AmmoID;

        if (invData.GetConsumableIDsAndQuantities().TryGetValue(ammoID, out int ammoAvailable) && ammoAvailable > 0)
        {
            var ammoNeeded = AmmoModel.MaxAmmo - AmmoModel.CurrentAmmo;
            var ammoToLoad = Mathf.Min(ammoAvailable, ammoNeeded);

            invData.DepleteConsumable(ammoID, ammoToLoad);
            AmmoModel.AddAmmo(AmmoModel.CurrentAmmoType, ammoToLoad);
        }

        reloadCoroutine = null;
    }
}