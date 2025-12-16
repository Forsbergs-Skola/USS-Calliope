using System;
using UnityEngine;

public class AmmoPickupPresenter : PickupBase
{
    [SerializeField] private AmmoPickUpView view;

    protected override void OnPickup(GameObject picker)
    {
        // Get the PlayerWeaponHandler instead
        var weaponHandler = picker.GetComponent<PlayerWeaponHandler>();
        if (weaponHandler == null)
        {
            Debug.LogWarning("AmmoPickup: Player has no PlayerWeaponHandler!");
            return;
        }

        // Access the AmmoModel from the handler
        var ammoModel = weaponHandler.AmmoModel;
        ammoModel.AddAmmo(view.AmmoType, view.AmmoAmount);

        Destroy(gameObject);
    }

}