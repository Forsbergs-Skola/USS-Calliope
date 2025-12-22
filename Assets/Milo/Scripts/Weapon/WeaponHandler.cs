using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerAimController aimController;
    [SerializeField] private PerformAttack performAttack;
    [SerializeField] private WeaponCooldown cooldown;
    [SerializeField] private AudioSource audioSource;
    
    private AttackInput attackInput;

    private AmmoModel ammoModel;
    private SO_WeaponType currentWeapon;
    private Transform currentMuzzle;

    public AmmoModel AmmoModel => ammoModel;

    private void Awake()
    {
        if (attackInput == null)
            attackInput = GetComponent<AttackInput>();
        
        if (aimController == null)
            aimController = GetComponent<PlayerAimController>();

        ammoModel = new AmmoModel();
        ammoModel.AmmoChanged += OnAmmoChanged;
        ammoModel.OnError += OnAmmoError;
    }

    private void Start()
    {
        attackInput.FireRequested += OnFireRequested;
        attackInput.AimStarted += OnAimStarted;
        attackInput.AimStopped += OnAimStopped;
    }

    private void OnDisable()
    {
        attackInput.FireRequested -= OnFireRequested;
        attackInput.AimStarted -= OnAimStarted;
        attackInput.AimStopped -= OnAimStopped;
    }

    // nput event callbacks 
    private void OnAimStarted()
    {
        // trigger crosshair or camera zoom
    }

    private void OnAimStopped()
    {
        // reset crosshair or camera
    }

    private void OnFireRequested()
    {
        if (!aimController.IsAiming)
            return;

        TryShoot();
    }

    // Weapon 
    private void TryShoot()
    {
        if (currentWeapon == null || currentMuzzle == null) return;
        if (!cooldown.CanFire()) return;

        int ammoCost = currentWeapon.AttackCategories switch
        {
            SO_WeaponType.AttackCategory.Hitscan => 1,
            SO_WeaponType.AttackCategory.Taser => 1,
            SO_WeaponType.AttackCategory.Melee => 0,
            _ => 0
        };

        if (ammoCost > 0 && !ammoModel.UseAmmo(ammoCost))
        {
            Debug.Log($"Click! {currentWeapon.WeaponId} out of ammo.");
            audioSource.PlayOneShot(currentWeapon.DryFireSounds);
            return;
        }

        performAttack.Fire();
        cooldown.StartCooldown(currentWeapon.FireRate);
    }

    public void EquipWeapon(SO_WeaponType equippedWeapon, Transform muzzle)
    {
        if (equippedWeapon == null) return; 

        currentWeapon = equippedWeapon;
        currentMuzzle = muzzle; 

        ammoModel.Initialize(equippedWeapon);
        cooldown.InitializeCooldown(equippedWeapon.FireRate);
    
        performAttack.SetWeapon(equippedWeapon, muzzle);
    }

    // Ammo  
    private void OnAmmoChanged(int current, int max)
    {
        Debug.Log($"Ammo: {current}/{max}");
    }

    private void OnAmmoError(string message)
    {
        Debug.LogWarning(message);
    }
}
