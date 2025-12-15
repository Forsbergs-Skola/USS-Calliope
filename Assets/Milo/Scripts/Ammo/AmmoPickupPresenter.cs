using System;
using UnityEngine;

public class AmmoPickup : PickupBase
{
    public event Action<SO_AmmoType, int> OnPickedUp;

    [SerializeField] private AmmoPickUpView view;

    public override void OnPickup(GameObject picker)
    {
        // Use GetComponentInChildren in case AmmoModel is on a child
        var ammoModel = picker.GetComponentInChildren<AmmoModel>();
        if (ammoModel != null)
        {
            // Pass both the ammo type and amount
            ammoModel.AddAmmo(view.AmmoType, view.AmmoAmount);

            OnPickedUp?.Invoke(view.AmmoType, view.AmmoAmount);
        }
    }
}