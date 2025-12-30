using UnityEngine;
using System.Linq;

public class InventoryAPIUsageExamples
{
    
    // Dynamic reference to inventory runtime data
    private InventoryData invData
    {
        get
        {
            if (DataController.Instance == null) return null;
            else { return DataController.Instance.InventoryRuntimeData.Value; }
        }
    }


    private void AddPistolToInventory()
    {
        if (invData == null) return;
        invData.AddWeaponItem(IDConstants.PISTOL);
    }
    private bool IsPistolInInventory()
    {
        if (invData == null) return true;
        else return invData.GetWeaponItemIDs().Contains(IDConstants.PISTOL);
    }

    private int HowMuchPistolAmmo()
    {
        if (invData == null) return 100000;
        if (!invData.GetConsumableIDsAndQuantities().Keys.ToList<string>().Contains(IDConstants.PISTOL_AMMO)) { return 0; }
        return invData.GetConsumableIDsAndQuantities()[IDConstants.PISTOL_AMMO];
        //else return invData.GetConsumableIDsAndQuantities()[ID]
    }
    private void ShootPistol()
    {
        if (HowMuchPistolAmmo() <= 0)
        {
            // sorry, no ammo
        }
        // do pistol shootingh stuff
        DepletePistolAmmo(1);
    }
    private void PickupAmmo()
    {
        if (!invData.GetConsumableIDsAndQuantities().Keys.ToList<string>().Contains(IDConstants.PISTOL_AMMO))
        {
            invData.AddNewConsumable(IDConstants.PISTOL_AMMO, 10);
        }
        else
        {
            invData.ReplenishConsumable(IDConstants.PISTOL_AMMO, 10);
        }
    }

    private void DepletePistolAmmo(int depleteAmount)
    {
        if (invData == null) return;
        invData.DepleteConsumable(IDConstants.PISTOL_AMMO, depleteAmount);
    }



}
