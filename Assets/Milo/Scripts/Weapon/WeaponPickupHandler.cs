using UnityEngine;

public class WeaponPickupHandler : PickupBase
{
    [SerializeField] private WeaponView weaponView;
    [SerializeField] private Transform weaponSocket;

    [Header("Optional Ammo On Pickup")]
    [SerializeField] private bool giveStartingAmmo;
    [SerializeField] private int startingAmmoAmount = 0;

    protected override void OnPickup(GameObject picker)
    {
        if (!picker.TryGetComponent<PlayerWeaponHandler>(out var handler))
            return;

        handler.EquipWeapon(weaponView.WeaponType, weaponView.gameObject);

        if (!giveStartingAmmo && !weaponView.WeaponType.AmmoType) return;
        handler.AmmoModel.AddAmmo(weaponView.WeaponType.AmmoType, startingAmmoAmount);

        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }
}