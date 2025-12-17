using UnityEngine;

public class WeaponPickupHandler : PickupBase
{
    [SerializeField] private SO_WeaponType weapon;

    [SerializeField] private Transform weaponSocket;

    protected override void OnPickup(GameObject picker)
    {
        if (!picker.TryGetComponent<PlayerWeaponHandler>(out var handler))
            return;

        handler.EquipWeapon(weapon);

        if (weaponSocket == null) return;
        // Keep the world position (optional: or remove if it should snap to socket)
        transform.SetParent(weaponSocket, worldPositionStays: false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

}