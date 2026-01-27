using Olle.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    public SO_WeaponType CurrentWeaponData => currentWeaponData;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SO_WeaponList weaponDatabase;
    [SerializeField] private UnarmedAttack unarmedAttack;
    [SerializeField] private Animator animator;
    [SerializeField] private WeaponReload weaponReload;
 
    private AttackInput attackInput;
    private SO_WeaponType currentWeaponData;
    private SO_FlashLight flashlightData;
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
        attackInput.ReloadTriggered += TryReload;
        attackInput.UnEquipWeaponTriggered += UnEquipWeapon;
    }

    private void OnDisable()
    {
        attackInput.FireStarted -= OnFireStarted;
        attackInput.FireStopped -= OnFireStopped;
        attackInput.AimStarted -= OnAimStarted;
        attackInput.AimStopped -= OnAimStopped;
        attackInput.SwitchWeaponTriggered -= EquipNextWeapon;
        attackInput.ReloadTriggered -= TryReload;
        attackInput.UnEquipWeaponTriggered -= UnEquipWeapon;    
    }

    private void UnEquipWeapon()
    {
        if (unarmedAttack.isUnarmed) return;

        if (currentWeaponData != null && currentWeaponData.HasAmmo && AmmoModel.CurrentAmmo > 0)
        {
            invData.ReplenishConsumable(currentWeaponData.AmmoType.AmmoID, AmmoModel.CurrentAmmo);
        }

        if (currentWeaponPrefab != null)
        {
            Destroy(currentWeaponPrefab);
        }

        if (weaponReload != null)
        {
            weaponReload.CancelReload();
        }

        animator.SetInteger("WeaponType", (int)0);

        OnFireStopped();

        currentWeaponData = null;
        unarmedAttack.isUnarmed = true;
    }

    private void EquipNextWeapon()
    {
        var availableWeapons = GetAvailableWeapons();
        if (availableWeapons.Count == 0)
        {
            unarmedAttack.isUnarmed = true;
            return;
        }

        if (!unarmedAttack.isUnarmed)
        {
            UnEquipWeapon();
        }

        equippedWeaponIndex = (equippedWeaponIndex + 1) % availableWeapons.Count;
        string weaponId = availableWeapons[equippedWeaponIndex];

        if (!weaponDatabase) return;
        SO_WeaponType weaponData = weaponDatabase.GetWeapon(weaponId);
        if (weaponData == null) return;

        currentWeaponData = weaponData;
        unarmedAttack.isUnarmed = false;

        ApplyWeaponSetup(weaponData);

        if (invData.GetConsumableIDsAndQuantities().TryGetValue(weaponData.AmmoType.AmmoID, out int ammoAvailable) && ammoAvailable > 0)
        {
            int amountToTake = Mathf.Min(weaponData.MagSize, ammoAvailable);
            invData.DepleteConsumable(weaponData.AmmoType.AmmoID, amountToTake);
            AmmoModel.AddAmmo(weaponData.AmmoType, amountToTake);
        }

    }

    private void ApplyWeaponSetup(SO_WeaponType data)
    {
        if (data.HasAmmo)
        {
            AmmoModel.InitializeAmmo(data);
        }
        weaponCooldown.InitializeCooldown(data.FireRate);
        performAttack.SetCurrentWeapon(data);


        if (data.WeaponModelPrefab != null && firePoint != null)
        {
            currentWeaponPrefab = Instantiate(data.WeaponModelPrefab, firePoint);

            currentWeaponPrefab.transform.localPosition = currentWeaponData.PelletCount == 1 ? new Vector3(0.05f, 0, 0) : Vector3.zero;
            currentWeaponPrefab.transform.localRotation = Quaternion.Euler(-90f, 0f, 90f); 
            
            // Quick solution to scaling down shotgun 
            currentWeaponPrefab.transform.localScale = currentWeaponData.PelletCount == 5 ? new Vector3(0.6f, 0.6f, 0.6f) : Vector3.one;
        }
    }

    private void OnFireStarted()
    {
        if (!currentWeaponData && !unarmedAttack.isUnarmed) return;

        if (unarmedAttack.isUnarmed)
        {
            Debug.Log("WeaponHandler: TryUnarmedAttack Called");
            TryUnarmedAttack();
            return;
        }

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
        if (weaponReload.reloadCoroutine != null) return;
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

    private void TryUnarmedAttack()
    {
        Debug.Log("WeaponHandler: TryUnarmedAttack Called");
        if (!weaponCooldown.CanFire()) return;
        performAttack.Execute();
        weaponCooldown.StartCooldown(unarmedAttack.Cooldown);
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
        weaponReload.TryReload(currentWeaponData, AmmoModel, invData);
    }
}