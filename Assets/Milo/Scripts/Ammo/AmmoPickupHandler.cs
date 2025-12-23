using System;
using UnityEngine;

public class AmmoPickupHandler : PickupBase
{
    [SerializeField] private AmmoPickUpView view;

    protected override void OnPickup(GameObject picker)
    {
        var weaponHandler = picker.GetComponent<PlayerWeaponHandler>();
        if (weaponHandler == null)
        {
            Debug.LogWarning("AmmoPickup: Player has no PlayerWeaponHandler!");
            return;
        }

        var ammoModel = weaponHandler.AmmoModel;
        ammoModel.AddAmmo(view.AmmoType, view.AmmoAmount, AmmoModel.AmmoDestination.Weapon);

        Destroy(gameObject);
    }
}