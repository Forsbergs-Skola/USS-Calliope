using UnityEngine;
using System.Collections.Generic;

public class PlayerPickupHandler
{
    public void HandleConsumablePickup(InventoryData invData, string worldID, string itemID, int qty)
    {
        Dictionary<string, int> consumablesDict = invData.GetConsumableIDsAndQuantities();

        invData.AddExhausedPickup(worldID);

        if (consumablesDict.ContainsKey(itemID))
        {
            invData.ReplenishConsumable(itemID, qty);
        }
        else
        {
            invData.AddNewConsumable(itemID, qty);
        }
    }


    public bool TryUseConsumable(InventoryData invData, string itemID, int qty = 1)
    {

        Dictionary<string, int> consumablesDict = invData.GetConsumableIDsAndQuantities();
        if (consumablesDict.ContainsKey(itemID))
        {
            if (consumablesDict[itemID] >= qty)
            {
                invData.DepleteConsumable(itemID, qty);
                return true;
            }
        }
        // default...
        return false;
    }

}
