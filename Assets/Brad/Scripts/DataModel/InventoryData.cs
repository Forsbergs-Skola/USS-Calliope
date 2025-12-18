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

    private List<EnumInventoryItem> _items;
    private Dictionary<EnumInventoryResource, int> _resources;

    // Public Access//
    public List<EnumInventoryItem> GetItemsList()
    {
        return new List<EnumInventoryItem>(_items);
        // for iteration, not mutation
    }
    public void AddItem(EnumInventoryItem _item)
    {
        if (_items.Contains(_item)) return;
        _items.Add(_item);
        DataTools.HandleOnDataChanged(this);
    }
    public void RemoveItem(EnumInventoryItem _item)
    {
        if (!_items.Contains(_item)) return;
        _items.Remove(_item);
        DataTools.HandleOnDataChanged(this);
    }

    public Dictionary<EnumInventoryResource, int> GetResourceDict()
    {
        return new Dictionary<EnumInventoryResource, int>(_resources);
    }
    public void DepleteResource(EnumInventoryResource resource, int depleteAmount)
    {
        depleteAmount = Mathf.Abs(depleteAmount);
        if (depleteAmount <= 0) return;
        if (_resources[resource] <= 0) return;

        int newValue = _resources[resource] - depleteAmount;
        newValue = Mathf.Max(0, newValue); // don't go below zero
        _resources[resource] = newValue;
        DataTools.HandleOnDataChanged(this);
    }
    public void ReplenishResource(EnumInventoryResource resource, int replenishAmount)
    {
        replenishAmount = Mathf.Abs(replenishAmount);
        if (replenishAmount <= 0) return;

        int newValue = _resources[resource] + replenishAmount;
        _resources[resource] = newValue;
        DataTools.HandleOnDataChanged(this);
    }


    //////////////////
    // Constructors //
    //////////////////
    public InventoryData()
    {
        IsSandbox = false;
        _items = new List<EnumInventoryItem>();
        _resources = new Dictionary<EnumInventoryResource, int>();
        InitializeResourcesDict();
    }
    public InventoryData(bool isSandbox)
    {
        IsSandbox = isSandbox;
        _items = new List<EnumInventoryItem>();
        _resources = new Dictionary<EnumInventoryResource, int>();
        InitializeResourcesDict();
    }
    public InventoryData(InventoryData inData)
    {
        IsSandbox = false;
        _items = inData.GetItemsList();
        _resources = inData.GetResourceDict();
    }
    public bool GetIsSandbox() { return IsSandbox; }

    /////////////
    // UTILITY //
    /////////////

    private void InitializeResourcesDict()
    {
        foreach(EnumInventoryResource _resource in System.Enum.GetValues(typeof(EnumInventoryResource)))
        {
            
            //Debug.Log(_resource.ToString());
            _resources[_resource] = 0;
            
        }
    }

}
