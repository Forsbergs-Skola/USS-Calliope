using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemCatalogSO", menuName = "Inventory/ItemCatalogSO")]
public class ItemCatalogSO : ScriptableObject
{
    [SerializeField] private List<QuestItemSO> allQuestItems;
    [SerializeField] private List<WeaponItemSO> allWeaponItems;
    [SerializeField] private List<ConsumableItemSO> allConsumableItems;

    private List<IInventoryItem> allInventoryItems;

    public List<QuestItemSO> AllQuestItems { get => allQuestItems; }
    public List<WeaponItemSO> AllWeaponItems { get => allWeaponItems; }
    public List<ConsumableItemSO> AllConsumableItems { get => allConsumableItems; }
    public List<IInventoryItem> AllInventoryItems { get => allInventoryItems; }
    private void OnValidate()
    {
        allInventoryItems = new List<IInventoryItem>();
        if (allQuestItems.Count > 0)
        {
            foreach (IInventoryItem item in allQuestItems)
            {
                allInventoryItems.Add(item);
            }
        }
        if (allWeaponItems.Count > 0)
        {
            foreach (IInventoryItem item in allWeaponItems)
            {
                allInventoryItems.Add(item);
            }
        }
        if (allConsumableItems.Count > 0)
        {
            foreach (IInventoryItem item in allConsumableItems)
            {
                allInventoryItems.Add(item);
            }
        }
        //Debug.Log($"Item count: {allInventoryItems.Count}");
    }


}
