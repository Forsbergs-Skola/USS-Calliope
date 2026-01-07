using System;

public class AmmoModel
{
    private int currentAmmo;
    private int maxAmmo;
    private SO_AmmoType currentAmmoType;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public SO_AmmoType CurrentAmmoType => currentAmmoType;
    
    public void InitializeAmmo(SO_WeaponType currentWeapon)
    {
        if (currentWeapon == null || !currentWeapon.HasAmmo)
        {
            ClearWeaponAmmo();
            return;
        }

        currentAmmoType = currentWeapon.AmmoType;
        maxAmmo = currentWeapon.MagSize;
        currentAmmo = 0;
    }

    private void ClearWeaponAmmo()
    {
        currentAmmoType = null;
        maxAmmo = 0;
        currentAmmo = 0;
    }

    public void AddAmmo(SO_AmmoType ammoType, int amount)
    {
        if (!CanAcceptAmmo(ammoType)) return;

        currentAmmo = Math.Min(currentAmmo + amount, maxAmmo);
    }

    private bool CanAcceptAmmo(SO_AmmoType ammoType)
    {
        if (currentAmmoType == null) return false;
        else if (ammoType != currentAmmoType) return false;
        
        return currentAmmo < maxAmmo;
    }

    public bool UseAmmo(int amount)
    {
        if (currentAmmo < amount)
            return false;

        currentAmmo -= amount;
        return true;
    }
}