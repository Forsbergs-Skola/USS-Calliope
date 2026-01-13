using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class ConsumablesGrid : MonoBehaviour
{
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private InventoryRuntimeData inventorySO;


    private void Awake()
    {
        if (InventoryController.Instance == null)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (DataController.Instance == null) { Destroy(gameObject); return; };
        if (EventRelay.Instance == null) { Destroy(gameObject); return; };

        InventoryData data = DataController.Instance.InventoryRuntimeData.Value;
        FixGrid(data);

        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered += HandleRuntimeDataUpdate;
    }
    private void OnDisable()
    {
        if (EventRelay.Instance == null) return;
        EventRelay.Instance.GameEvents.RuntimeDataUpdatedEvent.OnEventTriggered -= HandleRuntimeDataUpdate;
    }

    private void HandleRuntimeDataUpdate(IRuntimeData data)
    {
        if (!(data is InventoryData)) return;
        FixGrid(data as InventoryData);
    }
    private void FixGrid(InventoryData invData)
    {
        ClearGrid();
        Dictionary<string, int> consumablesDict = invData.GetConsumableIDsAndQuantities();
        foreach(string itemID in consumablesDict.Keys.ToList<string>())
        {
            ConsumableItemSO itemData = InventoryController.Instance.GetConsumableItemData(itemID);
            string resourcePath = itemData.IconTexturePath;
            int qty = consumablesDict[itemID];
            GameObject iconObj = Instantiate(iconPrefab, transform);
            HudConsumableIcon icon = iconObj.GetComponent<HudConsumableIcon>();
            icon.Setup(resourcePath, qty);
        }
    }

    private void ClearGrid()
    {
        foreach(Transform xform in transform)
        {
            Destroy(xform.gameObject);
        }

    }
}
