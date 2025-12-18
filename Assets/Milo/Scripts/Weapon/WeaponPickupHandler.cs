using UnityEngine;

public class WeaponPickupHandler : PickupBase
{
    [SerializeField] private WeaponView weaponView;

    [SerializeField] private Transform weaponSocket;

    protected override void OnPickup(GameObject picker)
    {
        if (!picker.TryGetComponent<PlayerWeaponHandler>(out var handler))
            return;

        handler.EquipWeapon(weaponView.WeaponType);

        if (weaponSocket == null) return;
        transform.SetParent(weaponSocket, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

}