using UnityEngine;

[CreateAssetMenu(fileName = "SO_WeaponType", menuName = "Player Combat/SO_WeaponType")]
public class SO_WeaponType : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string weaponName;
    [SerializeField] private string weaponCategory;
    [SerializeField] private Sprite weaponIcon;
    [TextArea(2, 7)] 
    [SerializeField] private string weaponDescription;
    
    [Header("Details")]
    [SerializeField, Min(5)] private int magSize;
    [SerializeField] private SO_AmmoType ammoType;
    
    [SerializeField, Min(0)] private int damage;
    [SerializeField, Min((float)0.01)] private float fireRate = 0.2f;
    [SerializeField] private bool isSemiAutomatic;
    
    [Header("Ballistics")]
    [SerializeField, Min(1f)] private float impactRange;
    [Tooltip("0.05+ for shotguns, 0.01+ for rifles & pistols")]
    [SerializeField, Range(0f, 0.1f)] private float spreadStandardDeviation;
    [SerializeField] private AnimationCurve damageOverDistance;
    [SerializeField, Min(1)] private int pelletCount = 1;
    
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
    
    // General
    public string WeaponId => weaponName;
    public Sprite WeaponIcon => weaponIcon; 
    public string WeaponDescription => weaponDescription;
    public string WeaponCategory => weaponCategory;
    
    // Details
    public int MagSize => magSize;
    public SO_AmmoType AmmoType => ammoType;
    public int Damage => damage;
    public float FireRate => fireRate;
    public bool IsSemiAutomatic => isSemiAutomatic;
    
    // Ballistics
    public float ImpactRange => impactRange;
    public int PelletCount => pelletCount;
    
    // This function calculates the final damage to apply in a hit
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
    
    // Tactical 
    
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
    
}