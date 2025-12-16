using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private AmmoModel ammoModel;
    private SO_WeaponType currentWeapon;

    void Awake()
    {
        ammoModel = new AmmoModel();
        ammoModel.AmmoChanged += OnAmmoChanged;
        ammoModel.OnError += OnAmmoError;
    }

    public void EquipWeapon(SO_WeaponType weapon)
    {
        if (weapon == null || weapon == currentWeapon)
            return;

        currentWeapon = weapon;
        ammoModel.Initialize(weapon);
        Debug.Log($"Equipped weapon: {weapon.WeaponId}");
    }

    private void OnAmmoChanged(int current, int max)
    {
        Debug.Log($"Ammo changed: {current}/{max}");
        // Update UI here
    }

    private void OnAmmoError(string message)
    {
        Debug.LogWarning(message);
    }

    public SO_WeaponType CurrentWeapon => currentWeapon;
    public AmmoModel AmmoModel => ammoModel;
}