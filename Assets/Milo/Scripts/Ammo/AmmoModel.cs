using UnityEngine;

public class AmmoModel : MonoBehaviour
{
    private int currentAmmo;
    private int maxAmmo;
    private SO_AmmoType currentAmmoType;
    private string currentWeaponName;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    // Pass in the weapon when equipping
    public void Initialize(SO_WeaponType weapon)
    {
        maxAmmo = weapon.MaxMagSize;
        currentAmmoType = weapon.AmmoType;
        currentWeaponName = weapon.WeaponId; 

        Debug.Log($"{currentWeaponName} Equipped. Current ammo: {currentAmmo}/{maxAmmo} ({currentAmmoType.AmmoId})");
    }

    public void AddAmmo(SO_AmmoType ammoType, int amount)
    {
        if (ammoType != currentAmmoType)
        {
            Debug.Log($"Cannot add ammo: {ammoType.AmmoId} does not match weapon type {currentAmmoType.AmmoId}");
            return;
        }

        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        Debug.Log($"Ammo added for {currentWeaponName}: {currentAmmo}/{maxAmmo} ({currentAmmoType.AmmoId})");
    }

    public bool UseAmmo(int amount)
    {
        if (currentAmmo < amount) return false;
        currentAmmo -= amount;
        return true;
    }
}