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


    public AmmoModel AmmoModel { get; private set; }

    private void Awake()
    {
        weaponCooldown = GetComponent<WeaponCooldown>();
        attackInput = GetComponent<AttackInput>();
        aimController = GetComponent<PlayerAimController>();
        performAttack = GetComponent<PerformAttack>();

        AmmoModel = new AmmoModel();
        AmmoModel.AmmoChanged += OnAmmoChanged;
        AmmoModel.OnError += OnAmmoError;
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


    private void OnAimStarted()
    {
    }

    private void OnAimStopped()
    {
    }

    private void OnFireStarted()
    {
        if (!aimController.IsAiming || currentWeapon == null)
            return;

        if (!currentWeapon.IsSemiAutomatic)
        {
            if (firingCoroutine == null)
            {
                isHoldingTrigger = true; // important!
                firingCoroutine = StartCoroutine(AutomaticFire());
            }
        }
        else
        {
            TryShoot(); // single fire
        }
    }

    private void OnFireStopped()
    {
        isHoldingTrigger = false; // stop the loop
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    private IEnumerator AutomaticFire()
    {
        while (isHoldingTrigger)
        {
            TryShoot();
            yield return new WaitForSeconds(currentWeapon.FireRate);
        }
    }

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
            Debug.Log($"Click! {currentWeapon.WeaponId} out of ammo.");
            audioSource.PlayOneShot(currentWeapon.DryFireSounds);
            return;
        }

        performAttack.Execute();
        weaponCooldown.StartCooldown(currentWeapon.FireRate);
    }

    public void EquipWeapon(SO_WeaponType equippedWeapon)
    {
        if (!equippedWeapon) return;

        currentWeapon = equippedWeapon;

        AmmoModel.Initialize(equippedWeapon);
        weaponCooldown.InitializeCooldown(equippedWeapon.FireRate);

        performAttack.SetWeapon(equippedWeapon);
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