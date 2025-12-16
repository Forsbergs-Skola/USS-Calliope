using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryData : IRuntimeData
// all the save-worthy inventory information
{
    public bool IsSandbox { get; private set; }

    /////////////////
    // Data Fields //
    /////////////////

    // TODO all the inventory data fields

    //////////////////
    // Constructors //
    //////////////////
    public InventoryData()
    {
        IsSandbox = false;
    }
    public InventoryData(bool isSandbox)
    {
        IsSandbox = isSandbox;
    }
    public InventoryData(InventoryData inData)
    {
        IsSandbox = false;
    }
    public bool GetIsSandbox() { return IsSandbox; }

}
