using System;
using UnityEngine;

public class AmmoPickupPresenter : PickupBase
{
    [SerializeField] private AmmoPickUpView view;

    public override void OnPickup(GameObject picker)
    {
        var ammoModel = picker.GetComponentInChildren<AmmoModel>();
        if (ammoModel == null)
        {
            Debug.LogWarning("AmmoPickup: Player has no AmmoModel!");
            return;
        }

        ammoModel.AddAmmo(view.AmmoType, view.AmmoAmount);

        Destroy(gameObject);
    }
}