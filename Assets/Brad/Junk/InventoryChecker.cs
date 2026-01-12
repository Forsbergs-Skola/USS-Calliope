using UnityEngine;
using System.Collections.Generic;


interface IAdrenalineHandler
{
    bool TryDepleteAdrenaline(int depleteAmount);
    void ReplinishAdrenaline(int replinishAmount);
}

public class InventoryChecker : MonoBehaviour, IAdrenalineHandler
{
    [SerializeField] private InventoryRuntimeData inventoryDataSO;
    private Dictionary<string, int> consumablesDict
    {
        get => inventoryDataSO.Value.GetConsumableIDsAndQuantities();
    }

    private bool TryGetAdrenalineQty(out int qty)
    {
        
        if (consumablesDict.ContainsKey(IDConstants.ADRENALINE))
        {
            qty = consumablesDict[IDConstants.ADRENALINE];
            return true;
        }
        qty = -1;
        return false;
    }
    
    /////////
    // API //
    /////////
    
    
    public bool TryDepleteAdrenaline(int depleteAmount = 1)
    {
        if (TryGetAdrenalineQty(out int qty))
        {
            if(qty <= 0)
            {
                // you are out of adrenaline
                return false;
            }
            inventoryDataSO.Value.DepleteConsumable(IDConstants.ADRENALINE, depleteAmount);
            return true;
        }
        // you haven't discovered the adrenaline yet...
        return false;
    }
    public void ReplinishAdrenaline(int replinishAmount = 1)
    {
        if (consumablesDict.ContainsKey(IDConstants.ADRENALINE))
        {
            inventoryDataSO.Value.ReplenishConsumable(IDConstants.ADRENALINE, replinishAmount);
            return;
        }
        inventoryDataSO.Value.AddNewConsumable(IDConstants.ADRENALINE, replinishAmount);
    }

}
