using System;
using UnityEngine;

public class WeaponPickupHandler : PickupBase
{
    
    // To access the current scriptable object data that each weapon has assigned in WeaponView
    [SerializeField] private WeaponView weaponView;
    
    [SerializeField] private PlayerWeaponHandler handler;

    protected override void OnPickup(GameObject picker)
    {
        if (!handler) return;

        handler.EquipWeapon(weaponView.WeaponType);

        gameObject.SetActive(false);  
    }

}