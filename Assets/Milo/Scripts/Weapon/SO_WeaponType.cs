using UnityEngine;

[CreateAssetMenu(fileName = "SO_WeaponType", menuName = "Player Combat/SO_WeaponType")]
public class SO_WeaponType : ScriptableObject
{
    [SerializeField] private string weaponId;
    [SerializeField] private Sprite weaponIcon;
    [SerializeField] private int weaponDamage;
    [SerializeField] private int magSize;
    [SerializeField] private SO_AmmoType ammoType;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private bool isSemiAutomatic = false;

    public string WeaponId => weaponId;
    public Sprite WeaponIcon => weaponIcon;
    public int WeaponDamage => weaponDamage;
    public int MagSize => magSize;
    public SO_AmmoType AmmoType => ammoType;
    public float FireRate => fireRate;
    public bool IsSemiAutomatic => isSemiAutomatic;
}