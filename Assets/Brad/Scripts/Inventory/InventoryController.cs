using UnityEngine;
using System.Collections.Generic;
using Events;
using System.Linq;

public enum EnumInventoryItemType
{
    WEAPON,
    QUEST_ITEM,
    CONSUMABLE
}

public interface IInventoryItem
{
    public string GetDisplayName();
    public EnumInventoryItemType GetItemType();
    public string GetItemID();
}





public class InventoryController : Singleton<InventoryController>
{
    [SerializeField] private ItemCatalogSO itemCatalog;
    [SerializeField] private StringPayloadEvent itemPickupEvent;


    private void Start()
    {
        itemPickupEvent.OnEventTriggered += HandleItemPickupEvent;
    }
    private void OnDestroy()
    {
        itemPickupEvent.OnEventTriggered -= HandleItemPickupEvent;
    }


    /////////
    // API //
    /////////

    public void AddItemToInventory(string itemID)
    {
        if (DataController.Instance == null) return;
        EnumInventoryItemType itemType = GetItemTypeByID(itemID);
        switch (itemType)
        {
            case EnumInventoryItemType.WEAPON:
                DataController.Instance.InventoryRuntimeData.Value.AddWeaponItem(itemID);
                break;
            case EnumInventoryItemType.QUEST_ITEM:
                DataController.Instance.InventoryRuntimeData.Value.AddQuestItem(itemID);
                break;
            case EnumInventoryItemType.CONSUMABLE:
                Dictionary<string, int> consumablesDict = DataController.Instance.InventoryRuntimeData.Value.GetConsumableIDsAndQuantities();
                if (consumablesDict.Keys.ToList<string>().Contains(itemID))
                {
                    DataController.Instance.InventoryRuntimeData.Value.ReplenishConsumable(itemID, 1);
                }
                else
                {
                    DataController.Instance.InventoryRuntimeData.Value.AddNewConsumable(itemID, 1);
                }
                break;
        }
        EventRelay.Instance.GameEvents.ItemPickupEvent.TriggerEvent(itemID);
    }
    public void RemoveItemFromInventory(string itemID)
    {
        // TODO
    }


    ///////////////////////////////////////////
    // Getting data from the Inventory model //
    ///////////////////////////////////////////
    
    public List<WeaponItemSO> GetWeaponInventoryData()
    {
        if (DataController.Instance == null) return null;

        List<WeaponItemSO> weaponList = new List<WeaponItemSO>();
        List<string> weaponIDs = DataController.Instance.InventoryRuntimeData.Value.GetWeaponItemIDs();
        foreach(string _id in weaponIDs)
        {
            weaponList.Add(GetWeaponDataWithID(_id));
        }
        return weaponList;
    }
    public List<QuestItemSO> GetQuestItemInventoryData()
    {
        if (DataController.Instance == null) return null;

        List<QuestItemSO> questItemList = new List<QuestItemSO>();
        List<string> questItemIDs = DataController.Instance.InventoryRuntimeData.Value.GetQuestItemIDs();
        foreach (string _id in questItemIDs)
        {
            questItemList.Add(GetQuestItemDataWithID(_id));
        }
        return questItemList;
    }
    public List<ConsumableItemSO> GetConsumableInventoryData()
    {
        if (DataController.Instance == null) return null;

        List<ConsumableItemSO> consumablesList = new List<ConsumableItemSO>();
        Dictionary<string, int> consumablesDict = DataController.Instance.InventoryRuntimeData.Value.GetConsumableIDsAndQuantities();
        foreach(string _id in consumablesDict.Keys.ToList<string>())
        {
            consumablesList.Add(GetConsumableItemData(_id));
        }
        return consumablesList;
    }
    public int GetConsumableQuantity(string consumableID)
    {
        if (DataController.Instance == null) return -1;
        Dictionary<string, int> consumablesDict = DataController.Instance.InventoryRuntimeData.Value.GetConsumableIDsAndQuantities();
        if (!consumablesDict.Keys.ToList<string>().Contains(consumableID)) return -1;
        return consumablesDict[consumableID];
    }


    ///////////////////////////////////
    // Getting data from the catalog //
    ///////////////////////////////////
    
    public WeaponItemSO GetWeaponDataWithID(string weaponID)
    {
        WeaponItemSO weaponData = itemCatalog.AllWeaponItems.Find(weap => weap.ItemID == weaponID);
        return weaponData;
    }
    public QuestItemSO GetQuestItemDataWithID(string questItemID)
    {
        QuestItemSO questItemData = itemCatalog.AllQuestItems.Find(qu => qu.ItemID == questItemID);
        return questItemData;
    }
    public ConsumableItemSO GetConsumableItemData(string consumableID)
    {
        ConsumableItemSO consumableData = itemCatalog.AllConsumableItems.Find(cons => cons.ItemID == consumableID);
        return consumableData;
    }

    
    public EnumInventoryItemType GetItemTypeByID(string itemID)
    {
        IInventoryItem item = itemCatalog.AllInventoryItems.Find(ite => ite.GetItemID() == itemID);
        if (item == null) return EnumInventoryItemType.CONSUMABLE;
        return (item.GetItemType());
    }

    public IInventoryItem GetInterfaceByID(string itemID)
    {
        return itemCatalog.AllInventoryItems.Find(ite => ite.GetItemID() == itemID);
    }

    /////////////////////////////////////////
    
    private void HandleItemPickupEvent(string itemID)
    {
        //DebugInventoryData();
    }

    private void DebugInventoryData()
    {
        InventoryData data = DataController.Instance.InventoryRuntimeData.Value;
        List<string> weaponsStrings = data.GetWeaponItemIDs();
        List<string> questItemStrings = data.GetQuestItemIDs();
        Dictionary<string, int> consDict = data.GetConsumableIDsAndQuantities();

        List<WeaponItemSO> weapons = new List<WeaponItemSO>();
        List<QuestItemSO> questItems = new List<QuestItemSO>();
        List<ConsumableItemSO> consumables = new List<ConsumableItemSO>();

        if (weaponsStrings.Count > 0)
        {
            foreach(string _str in weaponsStrings)
            {
                WeaponItemSO weaponData = GetWeaponDataWithID(_str);
                weapons.Add(weaponData);
            }
        }
        if (questItemStrings.Count > 0)
        {
            foreach(string _str in questItemStrings)
            {
                QuestItemSO questData = GetQuestItemDataWithID(_str);
                questItems.Add(questData);
            }
        }
        if (consDict.Keys.ToList<string>().Count > 0)
        {
            foreach(string _str in consDict.Keys.ToList<string>())
            {
                ConsumableItemSO consData = GetConsumableItemData(_str);
                consumables.Add(consData);
            }
        }

        string debugStr = "--- INVENTORY DEBUG ---\n\n";
        debugStr += "Weapons:\n";
        if (weapons.Count <= 0)
        {
            debugStr += "None\n\n";
        }
        else
        {
            foreach (WeaponItemSO weap in weapons)
            {
                debugStr += $"{weap.DisplayName}, ";
            }
            debugStr += "\n\n";
        }

        debugStr += "Quest Items:\n";
        if (questItems.Count <= 0)
        {
            debugStr += "None\n\n";
        }
        else
        {
            foreach(QuestItemSO qi in questItems)
            {
                debugStr += $"{qi.DisplayName}, ";
            }
            debugStr += "\n\n";
        }

        debugStr += "Consumables:\n";
        if (consumables.Count <= 0)
        {
            debugStr += "None\n\n";
        }
        else
        {
            foreach(ConsumableItemSO cons in consumables)
            {
                string consName = cons.DisplayName;
                int amount = GetConsumableQuantity(cons.ItemID);
                debugStr += $"- {consName}: {amount}\n";
            }
        }
        Debug.Log(debugStr);
    }
    

}
