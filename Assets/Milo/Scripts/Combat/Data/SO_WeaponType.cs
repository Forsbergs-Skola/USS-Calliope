using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SO_WeaponType", menuName = "Player/Player Combat/SO_WeaponType")]
public class SO_WeaponType : ScriptableObject
{
    public enum AttackCategory
    {
        Hitscan,
        NonLethal,
        Melee,
    }

    public enum RecoilTypes
    {
        CrosshairSway,
        SpreadPerShot,
    }

    public enum WeaponType
    {
        Unarmed = 0,
        Pistol = 1,
        Rifle = 2,
        Melee = 3
    }

    [SerializeField] private string weaponID = string.Empty;
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private bool usesLeftHandIK;
    [SerializeField] private string displayName;
    [SerializeField] private string weaponCategory;
    [SerializeField] private string ammoCategory;
    [SerializeField] private Sprite icon;
    [TextArea(2, 7)] [SerializeField] private string description;

    [SerializeField, Min(0)] private int damage;      
    [SerializeField, Min(0.01f)] private float fireRate = 0.2f;   
    [SerializeField] private bool isSemiAutomatic;
    [SerializeField] private AttackCategory attackCategory;
    [SerializeField] private bool hasAmmo;
    [SerializeField, Min(5)] private int magSize;
    [SerializeField] private SO_AmmoType ammoType;
    [SerializeField] private float reloadTime;

    [SerializeField] private bool hasBallistics;
    [SerializeField, Min(1f)] private float impactRange;
    [Tooltip("0.05+ for shotguns, 0.01+ for rifles & pistols")]
    [SerializeField, Range(0f, 0.1f)] private float spreadStandardDeviation;
    [SerializeField] private AnimationCurve damageOverDistance;
    [SerializeField, Min(1)] private int pelletCount = 1;
    
    [SerializeField] private RecoilTypes recoilType;
    [SerializeField, Min(0f)] private float recoilPerShotMin;
    [SerializeField, Min(0f)] private float recoilPerShotMax;
    [SerializeField, Min(0f)] private float recoilRecoverySpeed;

    [Tooltip("Pistol 1.5, Rifle 2.5, Shotgun 1.2")]
    [SerializeField, Min(0f)] private float movementInaccuracyMultiplier;
    [Tooltip("Pistol 3.0, Rifle 5.0, Shotgun 2.0")]
    [SerializeField, Min(0f)] private float sprintInaccuracyMultiplier;
    [Tooltip("Pistol 0.20s, Rifle 0.10s, Shotgun 0.25s")]
    [SerializeField, Range(0f, 0.5f)] private float accuracyGracePeriod = 0.15f;

    [SerializeField, Min(0)] private float stunEffectTime;

    [SerializeField] private bool isMelee;
    [SerializeField, Min(0.1f)] private float meleeReach;
    [SerializeField, Min(0.1f)] private float meleeHitRadius;
    [SerializeField] private float meleeHitForce;

    //// VISUALS
    [SerializeField] private GameObject weaponModelPrefab;
    [SerializeField] private ParticleSystem muzzleFlashPrefab;

    //// AUDIO
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip dryFireSound;

    //// PROPERTIES
   
    public string GetWeaponID()
    {
        return weaponID;
    }

    public string WeaponID => weaponID;
    public string DisplayName => displayName;
    public string WeaponCategory => weaponCategory;
    public string AmmoCategory => ammoCategory;
    public Sprite WeaponIcon => icon;
    public string WeaponDescription => description;
    public AttackCategory AttackCategories => attackCategory;
    public WeaponType TypeOfWeapon => weaponType;
    public bool UsesLeftHandIK => usesLeftHandIK;

    public int Damage => damage;
    public float FireRate => fireRate;
    public bool IsSemiAutomatic => isSemiAutomatic;
    public bool HasAmmo => hasAmmo;
    public int MagSize => magSize;
    public SO_AmmoType AmmoType => ammoType;
    public float ReloadTime => reloadTime;

    public bool HasBallistics => hasBallistics;
    public float ImpactRange => impactRange;
    public float SpreadStandardDeviation => spreadStandardDeviation;
    public AnimationCurve DamageOverDistance => damageOverDistance;
    public int PelletCount => pelletCount;
    public RecoilTypes RecoilType => recoilType;
    public float RecoilPerShotMin => recoilPerShotMin;
    public float RecoilPerShotMax => recoilPerShotMax;
    public float RecoilRecoverySpeed => recoilRecoverySpeed;

    public float MovementInaccuracyMultiplier => movementInaccuracyMultiplier;
    public float SprintInaccuracyMultiplier => sprintInaccuracyMultiplier;
    public float AccuracyGracePeriod => accuracyGracePeriod;

    public float StunEffectTime => stunEffectTime;
    public bool IsMelee => isMelee;
    public float MeleeReach => meleeReach;
    public float MeleeHitRadius => meleeHitRadius;
    public float MeleeHitForce => meleeHitForce;

    public GameObject WeaponModelPrefab => weaponModelPrefab;
    public ParticleSystem MuzzleFlashPrefab => muzzleFlashPrefab;
    public AudioClip AttackSound => attackSound;
    public AudioClip ReloadSound => reloadSound;
    public AudioClip DryFireSound => dryFireSound;

    public int GetDamageAtDistance(float distance)
    {
        float t = Mathf.Clamp01(distance / impactRange);
        float factor = damageOverDistance.Evaluate(t);
        return Mathf.RoundToInt(damage * factor);
    }

    public float GetBaseSpreadIntensity(float movementTimer, bool isSprinting, float hitChanceScore)
    {
        // invert hitChanceScore so low score = high spread
        float multiplier = 1f - hitChanceScore;

        if (!isSprinting && movementTimer <= accuracyGracePeriod)
            return spreadStandardDeviation * multiplier;

        if (isSprinting)
            return spreadStandardDeviation * sprintInaccuracyMultiplier * multiplier;

        return spreadStandardDeviation * movementInaccuracyMultiplier * multiplier;
    }

    public bool ShouldTrackMovement()
    {
        return attackCategory == AttackCategory.Hitscan;
    }
    
    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(weaponID)) return;
        weaponID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
