using UnityEngine;

public class AmmoPickupHandler : PickupBase
{
    [SerializeField] private AmmoPickUpView ammoView;
    [SerializeField, Min(1)] private int amount = 10;
    
    private InventoryData invData
    {
        get
        {
            if (!DataController.Instance) return null;
            else return DataController.Instance.InventoryRuntimeData.Value;
        }
    }

    protected override void OnPickup(GameObject picker)
    {
        if (invData == null || !ammoView.AmmoType) return;

        var consumables = invData.GetConsumableIDsAndQuantities();

        if (consumables.ContainsKey(ammoView.AmmoType.AmmoID))
        {
            invData.ReplenishConsumable(ammoView.AmmoType.AmmoID, amount);
        }
        else
        {
            invData.AddNewConsumable(ammoView.AmmoType.AmmoID, amount);
        }        Destroy(gameObject);
    }
}