    using System;

    public class AmmoModel
    {
        private int currentAmmo;
        private int maxAmmo;
        private SO_AmmoType currentAmmoType;
        private string currentWeaponName;

        public int CurrentAmmo => currentAmmo;
        public int MaxAmmo => maxAmmo;
        public string WeaponName => currentWeaponName;
        public SO_AmmoType CurrentAmmoType => currentAmmoType;

        public event Action<int, int> AmmoChanged;

        public event Action<string> OnError;

        public void Initialize(SO_WeaponType weapon)
        {
            if (weapon == null)
            {
                currentAmmoType = null;
                currentWeaponName = null;
                maxAmmo = 0;
                currentAmmo = 0;
                OnError?.Invoke("AmmoModel: No weapon provided during initialization.");
                return;
            }

            currentAmmoType = weapon.AmmoType;
            currentWeaponName = weapon.WeaponId;
            maxAmmo = weapon.MagSize;
            currentAmmo = 0;

            AmmoChanged?.Invoke(currentAmmo, maxAmmo);
        }

        public void AddAmmo(SO_AmmoType ammoType, int amount)
        {
            if (currentAmmoType == null)
            {
                OnError?.Invoke($"AmmoModel: No weapon equipped. Ammo pickup of {ammoType?.AmmoId ?? "null"} ignored.");
                return;
            }

            if (ammoType != currentAmmoType)
            {
                OnError?.Invoke($"AmmoModel: Cannot add ammo {ammoType.AmmoId}, does not match weapon {currentAmmoType.AmmoId}");
                return;
            }

            currentAmmo = Math.Min(currentAmmo + amount, maxAmmo);
            AmmoChanged?.Invoke(currentAmmo, maxAmmo);
        }

        public bool UseAmmo(int amount)
        {
            if (currentAmmo < amount)
            {
                OnError?.Invoke($"AmmoModel: Not enough ammo to use {amount}. Current: {currentAmmo}");
                return false;
            }

            currentAmmo -= amount;
            AmmoChanged?.Invoke(currentAmmo, maxAmmo);
            return true;
        }
    }
