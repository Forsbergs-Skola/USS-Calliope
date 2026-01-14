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

        UpdateTheBackEnd();

    }

    private void ClearWeaponAmmo()
    {
        currentAmmoType = null;
        maxAmmo = 0;
        currentAmmo = 0;

        UpdateTheBackEnd();

    }

    public void AddAmmo(SO_AmmoType ammoType, int amount)
    {
        if (!CanAcceptAmmo(ammoType)) return;

        currentAmmo = Math.Min(currentAmmo + amount, maxAmmo);

        UpdateTheBackEnd();
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
        UpdateTheBackEnd();
        return true;
    }

    private void UpdateTheBackEnd()
    {
        if (DataController.Instance == null) return;

        PlayerData pData = DataController.Instance.PlayerRuntimeData.Value;
        pData.PlayerCurrentAmmo = currentAmmo;

    }

}