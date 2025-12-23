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
            if (!currentWeapon)
            {
                currentAmmoType = null;
                maxAmmo = 0;
                currentAmmo = 0;
                return;
            }

            currentAmmoType = currentWeapon.AmmoType;
            maxAmmo = currentWeapon.MagSize;
            currentAmmo = 0;
        }
        
        public enum AmmoDestination
        {
            Weapon,
            Inventory
        }

        public void AddAmmo(SO_AmmoType ammoType, int amount, AmmoDestination destination)
        {
            // Determine if ammo should go to inventory
            var destinationIsInventory = destination == AmmoDestination.Inventory;
            var currentWeaponCannotAcceptAmmo = !currentAmmoType || ammoType != currentAmmoType || currentAmmo >= maxAmmo;

            if (destinationIsInventory || currentWeaponCannotAcceptAmmo)
            {
                // Send to inventory
                return;
            }

            // Add ammo to weapon
            currentAmmo = Math.Min(currentAmmo + amount, maxAmmo);
        }
        
        public bool UseAmmo(int amount)
        {
            if (currentAmmo < amount)
            {
                return false;
            }

            currentAmmo -= amount;
            return true;
        }
    }
