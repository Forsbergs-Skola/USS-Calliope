using UnityEngine;
using System.Collections.Generic;

public class PlayerPickupHandler
{
    public void HandleConsumablePickup(InventoryData invData, string worldID, string itemID, int qty)
    {
        Dictionary<string, int> consumablesDict = invData.GetConsumableIDsAndQuantities();

        invData.ExhaustedPickups.Add(worldID);

        if (consumablesDict.ContainsKey(itemID))
        {
            invData.ReplenishConsumable(itemID, qty);
        }
        else
        {
            invData.AddNewConsumable(itemID, qty);
        }
    }
}
