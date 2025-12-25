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

        if (giveStartingAmmo && weaponView.WeaponType.AmmoType)
        {
            handler.AmmoModel.AddAmmo(
                weaponView.WeaponType.AmmoType,
                startingAmmoAmount,
                AmmoModel.AmmoDestination.Weapon
            );
        }

        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }


    private void AttachToSocket()
    {
        if (!weaponSocket) return;

        transform.SetParent(weaponSocket, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private void DisablePickup()
    {
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }
}