using System;
using UnityEngine;

public class WeaponPickupHandler : PickupBase
{
    private InventoryData invData
    {
        get
        {
            if (DataController.Instance == null) return null;
            else { return DataController.Instance.InventoryRuntimeData.Value; }
        }
    }
    
    // To access the current scriptable object data that each weapon has assigned in WeaponView
    [SerializeField] private WeaponView weaponView;
    
    [SerializeField] private PlayerWeaponHandler handler;

    protected override void OnPickup(GameObject picker)
    {
        if (!handler) return;
        
        // Add it to inventory
        

       //  handler.EquipWeapon(weaponView.WeaponType);

        gameObject.SetActive(false);  
    }

}