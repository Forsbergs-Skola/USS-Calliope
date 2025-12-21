using UnityEngine;

[CreateAssetMenu(fileName = "SO_WeaponType", menuName = "Player Combat/SO_WeaponType")]
public class SO_WeaponType : ScriptableObject
{
    //
    // ENUMS
    //
    public enum AttackCategory
    {
        Hitscan,
        Taser,
        Melee,  
    }

    //
    // GENERAL
    //
    [Header("General")]
    [SerializeField] private string id;
    [SerializeField] private string weaponCategory;
    [SerializeField] private string ammoCategory;
    [SerializeField] private Sprite icon;
    [TextArea(2, 7)] [SerializeField] private string description;

    //
    // DETAILS
    //
    [Header("Details")]
    [SerializeField, Min(5)] private int magSize;
    [SerializeField] private SO_AmmoType ammoType;
    [SerializeField] private float reloadTime;
    [SerializeField] private AttackCategory attackCategory; // Gameplay logic type
    [SerializeField, Min(0)] private int damage;
    [SerializeField, Min(0.01f)] private float fireRate = 0.2f;
    [SerializeField] private bool isSemiAutomatic;

    //
    // BALLISTICS
    //
    [Header("Ballistics")]
    [SerializeField, Min(1f)] private float impactRange;
    [Tooltip("0.05+ for shotguns, 0.01+ for rifles & pistols")]
    [SerializeField, Range(0f, 0.1f)] private float spreadStandardDeviation;
    [SerializeField] private AnimationCurve damageOverDistance;
    [SerializeField, Min(1)] private int pelletCount = 1;
    [SerializeField, Min(0f)] private float recoilPerShotMin;
    [SerializeField, Min(0f)] private float recoilPerShotMax;
    [SerializeField, Min(0f)] private float recoilRecoverySpeed;

    //
    // MOVEMENT INACCURACY
    //
    [Header("Movement Inaccuracy")]
    [Tooltip("Pistol 1.5, Rifle 2.5, Shotgun 1.2")]
    [SerializeField, Min(0f)] private float movementInaccuracyMultiplier;
    [Tooltip("Pistol 3.0, rifle 5.0, shotgun 2.0")]
    [SerializeField, Min(0f)] private float sprintInaccuracyMultiplier;
    [Tooltip("Pistol 0.20s, rifle 0.10s, shotgun 0.25s")]
    [SerializeField, Range(0f, 0.5f)] private float accuracyGracePeriod = 0.15f;

    //
    // TASER DATA
    //
    
    [Header("Taser")]
    [SerializeField, Min(0)] private float stunEffectTime;
    
    //
    // VISUALS
    //
    [Header("Visuals")]
    [SerializeField] private GameObject weaponModelPrefab;

    //
    // AUDIO
    //
    [Header("Audio")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip dryFireSound;
    

    // General
    public string WeaponId => id;
    public string WeaponCategory => weaponCategory;
    public string AmmoCategory => ammoCategory;
    public Sprite WeaponIcon => icon;
    public string WeaponDescription => description;
    public AttackCategory Category => attackCategory; 

    // Details
    public int MagSize => magSize;
    public SO_AmmoType AmmoType => ammoType;
    public int Damage => damage;
    public float FireRate => fireRate;
    public bool IsSemiAutomatic => isSemiAutomatic;
    public float ReloadTime => reloadTime;

    // Ballistics
    public float ImpactRange => impactRange;
    public int PelletCount => pelletCount;
    public float RecoilPerShotMin => recoilPerShotMin;
    public float RecoilPerShotMax => recoilPerShotMax;
    public float RecoilRecoverySpeed => recoilRecoverySpeed;

    public int GetDamageAtDistance(float distance)
    {
        float t = Mathf.Clamp01(distance / impactRange);
        float factor = damageOverDistance.Evaluate(t);
        return Mathf.RoundToInt(damage * factor);
    }

    public float GetBaseSpreadIntensity(float movementTimer, bool isSprinting)
    {
        if (isSprinting)
            return spreadStandardDeviation * sprintInaccuracyMultiplier;

        if (movementTimer > accuracyGracePeriod)
            return spreadStandardDeviation * movementInaccuracyMultiplier;

        return spreadStandardDeviation;
    }
    
    // Stun 
    
    public float StunEffectTime => stunEffectTime;

    // Visuals
    public GameObject WeaponModelPrefab => weaponModelPrefab;

    // Audio
    public AudioClip FireSound => fireSound;
    public AudioClip ReloadSounds => reloadSound;
    public AudioClip DryFireSounds => dryFireSound;
}
