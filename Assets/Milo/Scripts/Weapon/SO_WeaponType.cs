using UnityEngine;

[CreateAssetMenu(fileName = "SO_WeaponType", menuName = "Player Combat/SO_WeaponType")]
public class SO_WeaponType : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string id;
    [SerializeField] private string category;
    [SerializeField] private Sprite icon;
    [TextArea(2, 7)] 
    [SerializeField] private string description;
    
    [Header("Details")]
    [SerializeField, Min(5)] private int magSize;
    [SerializeField] private SO_AmmoType ammoType;
    // Explore here
    [SerializeField] private float reloadTime;
    
    [SerializeField, Min(0)] private int damage;
    [SerializeField, Min((float)0.01)] private float fireRate = 0.2f;
    [SerializeField] private bool isSemiAutomatic;
    
    [Header("Ballistics")]
    [SerializeField, Min(1f)] private float impactRange;
    [Tooltip("0.05+ for shotguns, 0.01+ for rifles & pistols")]
    [SerializeField, Range(0f, 0.1f)] private float spreadStandardDeviation;
    [SerializeField] private AnimationCurve damageOverDistance;
    [SerializeField, Min(1)] private int pelletCount = 1;
    
    // ! Next Step Implement Recoil ! // 
    [Header("Recoil")]
    [SerializeField, Min(0f)] private float recoilPerShotMin;
    [SerializeField, Min(0f)] private float recoilPerShotMax;
    [SerializeField, Min(0f)] private float recoilRecoverySpeed;

    [Header("Movement Inaccuracy")] 
    [Tooltip("Pistol 1.5, Rifle 2.5, Shotgun 1.2")]
    [SerializeField, Min(0f)] private float movementInaccuracyMultiplier;
    [Tooltip("Pistol 3.0, rifle 5.0, shotgun 2.0")]
    [SerializeField, Min(0f)] private float sprintInaccuracyMultiplier;
    [Tooltip("Pistol 0.20s, rifle 0.10s,  shotgun 0.25s")]
    [SerializeField, Range(0f, 0.5f)] private float accuracyGracePeriod = 0.15f;
    
    [Header("Visuals")]
    [SerializeField] private GameObject weaponModelPrefab;
    
    [Header("Audio")]
    [SerializeField] private AudioClip[] fireSounds;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip dryFireSound;
    
    //         //
    // GETTERS //
    //         //
    
    // General
    public string WeaponId => id;
    public Sprite WeaponIcon => icon; 
    public string WeaponDescription => description;
    public string WeaponCategory => category;
    
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
    
    // Calculates the final damage to apply in a hit
    public int GetDamageAtDistance(float distance)
    {
        float t = Mathf.Clamp01(distance / impactRange);
        
        float factor = damageOverDistance.Evaluate(t);
        
        return Mathf.RoundToInt(damage * factor);
    }
    
    // Recoil
    public float RecoilPerShotMin => recoilPerShotMin;
    public float RecoilPerShotMax => recoilPerShotMax;
    public float RecoilRecoverySpeed => recoilRecoverySpeed;
    
   
    // This function calculates the final spread value depending on the parameters 
    public float GetBaseSpreadIntensity(float movementTimer, bool isSprinting)
    {
        if (isSprinting) return spreadStandardDeviation * sprintInaccuracyMultiplier;

        if (movementTimer > accuracyGracePeriod)
        {
            return spreadStandardDeviation * movementInaccuracyMultiplier;
        }

        return spreadStandardDeviation;
    }
    
    // Visuals
    public GameObject WeaponModelPrefab => weaponModelPrefab;
    
    // Audio
    
    public  AudioClip[] FireSounds => fireSounds;
    public  AudioClip ReloadSounds => reloadSound;
    public  AudioClip DryFireSounds => dryFireSound;
}
