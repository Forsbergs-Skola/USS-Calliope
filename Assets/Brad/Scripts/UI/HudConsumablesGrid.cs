using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public interface IHudConsumableIcon
{
    void SetupIcon(ConsumableItemSO consumableData, int qty);
}
public class HudConsumablesGrid : MonoBehaviour
{
    
    [SerializeField] private GameObject consumablesIconPrefab;
    [SerializeField] private InventoryRuntimeData inventoryDataSO;

    private List<IHudConsumableIcon> currentConsumables = new List<IHudConsumableIcon>();
    private Transform iconHolder;

    private void Awake()
    {
        iconHolder = transform;
    }

    private void OnEnable()
    {
        UpdateIcons(inventoryDataSO.Value);

        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleInventoryUpdate;

    }
    private void OnDisable()
    {
        ClearIcons();
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleInventoryUpdate;
    }

    private void HandleInventoryUpdate(IRuntimeData data)
    {
        if (!(data is InventoryData)) return;
        //InventoryData invData = data as InventoryData;
        UpdateIcons(data as InventoryData);
    }

    private void UpdateIcons(InventoryData invData)
    {
        ClearIcons();
        Dictionary<string, int> consumablesDict = invData.GetConsumableIDsAndQuantities();
        foreach (string key in consumablesDict.Keys.ToList<string>())
        {
            ConsumableItemSO item = InventoryController.Instance.GetConsumableItemData(key);
            int qty = consumablesDict[key];

            GameObject iconObj = Instantiate(consumablesIconPrefab, iconHolder);
            IHudConsumableIcon hudIcon = iconObj.GetComponent<IHudConsumableIcon>();
            hudIcon.SetupIcon(item, qty);

        }
    }

    private void ClearIcons()
    {
        // TODO
    }


}
