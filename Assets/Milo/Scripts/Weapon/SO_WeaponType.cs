using UnityEngine;

[CreateAssetMenu(fileName = "SO_WeaponType", menuName = "Player Combat/SO_WeaponType")]
public class SO_WeaponType : ScriptableObject
{
    [SerializeField] private string weaponId;
    [SerializeField] private Sprite weaponIcon;
    [SerializeField, Min(0)] private int weaponDamage;
    [SerializeField, Min(5)] private int magSize;
    [SerializeField] private SO_AmmoType ammoType;
    [SerializeField, Min((float)0.01)] private float fireRate = 0.2f;
    [SerializeField] private bool isSemiAutomatic = false;
    [SerializeField, Min(1)] private float impactRange;
    [SerializeField] private AnimationCurve damageOverDistance;
    
    public string WeaponId => weaponId;
    public Sprite WeaponIcon => weaponIcon;
    public int WeaponDamage => weaponDamage;
    public int MagSize => magSize;
    public SO_AmmoType AmmoType => ammoType;
    public float FireRate => fireRate;
    public bool IsSemiAutomatic => isSemiAutomatic;
    public float ImpactRange => impactRange;
    
    public int GetDamageAtDistance(float distance)
    {
        float t = Mathf.Clamp01(distance / impactRange);
        
        float factor = damageOverDistance.Evaluate(t);
        
        return Mathf.RoundToInt(weaponDamage * factor);
    }
}