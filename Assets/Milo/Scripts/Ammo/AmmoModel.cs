using UnityEngine;

public class AmmoModel : MonoBehaviour
{
    private int currentAmmo;
    private int maxAmmo;
    private SO_AmmoType currentAmmoType;
    private string currentWeaponName;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    public void Initialize(SO_WeaponType weapon)
    {
        if (weapon == null)
        {
            currentAmmoType = null;
            currentWeaponName = null;
            maxAmmo = 0;
            currentAmmo = 0;
            return;
        }

        currentAmmoType = weapon.AmmoType;
        currentWeaponName = weapon.WeaponId;
        maxAmmo = weapon.MagSize;
        
        currentAmmo = 0;

        Debug.Log($"AmmoModel: Equipped {currentWeaponName}. Ammo: {currentAmmo}/{maxAmmo}");
    }

    public void AddAmmo(SO_AmmoType ammoType, int amount)
    {
        // When we have an inventory, add this ammo to the inventory 
        if (currentAmmoType == null)
        { 
            Debug.Log($"AmmoModel: No weapon equipped yet. Ammo pickup of {ammoType.AmmoId} ignored.");
            return;
        }
        // When we have an inventory, add this ammo to the inventory 
        if (ammoType != currentAmmoType)
        {
            Debug.Log($"AmmoModel: Cannot add ammo {ammoType.AmmoId}, does not match weapon {currentAmmoType.AmmoId}");
            return;
        }

        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        Debug.Log($"AmmoModel: Added ammo for {currentWeaponName}. Current ammo: {currentAmmo}/{maxAmmo}");
    }

    public bool UseAmmo(int amount)
    {
        if (currentAmmo < amount)
            return false;

        currentAmmo -= amount;
        return true;
    }
}