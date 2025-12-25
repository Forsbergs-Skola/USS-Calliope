using System;

public class AmmoModel
{
    private int currentAmmo;
    private int maxAmmo;
    private SO_AmmoType currentAmmoType;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public SO_AmmoType CurrentAmmoType => currentAmmoType;

    public void Initialize(SO_WeaponType currentWeapon)
    {
        if (!currentWeapon || !currentWeapon.HasAmmo)
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

    public enum AmmoDestination
    {
        Weapon,
        Inventory
    }

    public void AddAmmo(SO_AmmoType ammoType, int amount, AmmoDestination destination)
    {
        // Explicit inventory routing (store pickups, overflow, etc.)
        if (destination == AmmoDestination.Inventory)
        {
            SendToInventory(ammoType, amount);
            return;
        }

        // Weapon cannot accept ammo
        if (!CanAcceptAmmo(ammoType))
        {
            SendToInventory(ammoType, amount);
            return;
        }

        // Add ammo to weapon
        currentAmmo = Math.Min(currentAmmo + amount, maxAmmo);
    }

    private bool CanAcceptAmmo(SO_AmmoType ammoType)
    {
        if (!currentAmmoType)
            return false;

        if (ammoType != currentAmmoType)
            return false;

        if (currentAmmo >= maxAmmo)
            return false;

        return true;
    }

    private void SendToInventory(SO_AmmoType ammoType, int amount)
    {
        // Inventory system hooks here later
        // Inventory.AddAmmo(ammoType, amount);
    }

    public bool UseAmmo(int amount)
    {
        if (currentAmmo < amount)
            return false;

        currentAmmo -= amount;
        return true;
    }
}