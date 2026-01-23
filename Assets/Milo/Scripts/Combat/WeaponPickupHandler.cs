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
    
    [SerializeField] private WeaponView weaponView;
    
    [SerializeField] private PlayerWeaponHandler handler;

    protected override void OnPickup(GameObject picker)
    {
        
        if (handler == null)
        {
            handler = picker.GetComponent<PlayerWeaponHandler>();
            if (handler == null)
            {
                return;
            }
        }

        if (weaponView == null || weaponView.WeaponType == null)
        {
            return;
        }
        
        if (invData != null) invData.AddWeaponItem(weaponView.WeaponType.WeaponID);

        Destroy(gameObject);
    }


}