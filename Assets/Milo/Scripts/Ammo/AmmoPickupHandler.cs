using UnityEngine;

public class AmmoPickupHandler : PickupBase
{
    [SerializeField] private SO_AmmoType ammoType;
    [SerializeField, Min(1)] private int amount = 10;

    protected override void OnPickup(GameObject picker)
    {
        if (!picker.TryGetComponent<PlayerWeaponHandler>(out var handler))
            return;

        handler.AmmoModel.AddAmmo(ammoType, amount);
        Destroy(gameObject);
    }
}