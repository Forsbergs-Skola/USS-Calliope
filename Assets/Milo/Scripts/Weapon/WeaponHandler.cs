using System.Collections;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform firePoint;

    private AttackInput attackInput;
    private SO_WeaponType currentWeapon;
    private bool isHoldingTrigger;
    private Coroutine firingCoroutine;
    private PerformAttack performAttack;
    private PlayerAimController aimController;
    private WeaponCooldown weaponCooldown;
    private GameObject currentWeaponGO;

    public AmmoModel AmmoModel { get; private set; }

    //// UNITY LIFECYCLE
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

    //// PUBLIC METHODS
    public void EquipWeapon(SO_WeaponType equippedWeapon, GameObject weaponGO = null)
    {
        if (!equippedWeapon) return;

        if (currentWeaponGO)
            Destroy(currentWeaponGO);

        currentWeapon = equippedWeapon;
        currentWeaponGO = weaponGO;

        AmmoModel.Initialize(equippedWeapon);
        weaponCooldown.InitializeCooldown(equippedWeapon.FireRate);

        performAttack.SetCurrentWeapon(equippedWeapon);

        if (!currentWeaponGO || !firePoint) return;
        currentWeaponGO.transform.SetParent(firePoint.parent, worldPositionStays: false);
        currentWeaponGO.transform.localPosition = Vector3.zero;
        currentWeaponGO.transform.localRotation = Quaternion.identity;
        currentWeaponGO.transform.localScale = Vector3.one; 

    }


    //// INPUT CALLBACKS
    private void OnFireStarted()
    {
        if (!aimController.IsAiming || !currentWeapon)
            return;

        if (!currentWeapon.IsSemiAutomatic)
        {
            if (firingCoroutine != null) return;
            isHoldingTrigger = true;
            firingCoroutine = StartCoroutine(AutomaticFire());
        }
        else
        {
            TryShoot();
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

    //// CORE FUNCTIONALITY
    private void TryShoot()
    {
        if (!currentWeapon || !firePoint) return;
        if (!weaponCooldown.CanFire()) return;

        var ammoCost = currentWeapon.AttackCategories switch
        {
            SO_WeaponType.AttackCategory.Hitscan => 1,
            SO_WeaponType.AttackCategory.Taser => 1,
            SO_WeaponType.AttackCategory.Melee => 0,
            _ => 0
        };

        if (ammoCost > 0 && !AmmoModel.UseAmmo(ammoCost))
        {
            audioSource.PlayOneShot(currentWeapon.DryFireSound);
            return;
        }

        performAttack.Execute();
        weaponCooldown.StartCooldown(currentWeapon.FireRate);
    }

    private IEnumerator AutomaticFire()
    {
        while (isHoldingTrigger)
        {
            TryShoot();
            yield return new WaitForSeconds(currentWeapon.FireRate);
        }
    }

    //// AMMO EVENTS
    private void OnAmmoChanged(int current, int max)
    {
        Debug.Log($"Ammo: {current}/{max}");
    }

    private void OnAmmoError(string message)
    {
        Debug.LogWarning(message);
    }
}
