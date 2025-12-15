using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private AmmoModel ammoModel;
    [SerializeField] private SO_WeaponType startingWeapon; // assign in inspector

    private SO_WeaponType currentWeapon;

    private void Start()
    {
        if (startingWeapon != null)
            EquipWeapon(startingWeapon); // initialize ammo for combat prototype
    }

    public void EquipWeapon(SO_WeaponType weapon)
    {
        currentWeapon = weapon;
        ammoModel.Initialize(currentWeapon);
    }

    public SO_WeaponType GetCurrentWeapon() => currentWeapon;
}