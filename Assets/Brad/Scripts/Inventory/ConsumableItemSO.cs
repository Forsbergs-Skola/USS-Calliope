using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItemSO", menuName = "Inventory/ConsumableItemSO")]
public class ConsumableItemSO : ScriptableObject, IInventoryItem
{
    [SerializeField] private string itemID = string.Empty;
    [SerializeField] private string displayName = string.Empty;
    [SerializeField] private string iconTexturePath = string.Empty;


    public string ItemID { get => itemID; }
    public string DisplayName { get => displayName; }
    public string IconTexturePath { get => iconTexturePath; }


    private void OnValidate()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }


    // Interface Methods
    public string GetDisplayName()
    {
        return displayName;
    }
    public EnumInventoryItemType GetItemType()
    {
        return EnumInventoryItemType.CONSUMABLE;
    }
    public string GetItemID()
    {
        return itemID;
    }


}
