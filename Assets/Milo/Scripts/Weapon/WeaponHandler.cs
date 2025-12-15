using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AmmoModel ammoModel;

    private SO_WeaponType currentWeapon;

    /// <summary>
    /// Equip a new weapon. Ammo stays 0 until picked up.
    /// </summary>
    public void EquipWeapon(SO_WeaponType weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("PlayerWeaponHandler: Tried to equip null weapon.");
            return;
        }

        if (weapon == currentWeapon)
            return;

        currentWeapon = weapon;

        // Inform the AmmoModel of the equipped weapon type
        if (ammoModel != null)
            ammoModel.Initialize(weapon); // sets ammo type, keeps currentAmmo = 0

        Debug.Log($"Equipped weapon: {weapon.WeaponId}");
    }

    public SO_WeaponType CurrentWeapon => currentWeapon;
}
