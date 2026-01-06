using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] private InventoryRuntimeData inventoryRuntimeData;
    [SerializeField] private GameObject inventoryItemUIPrefab;
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private TMP_Text itemNameText;


    private InventoryData inventoryData = null;


    private void OnEnable()
    {
        itemNameText.text = string.Empty;
        if (InventoryController.Instance == null) return;
        inventoryData = inventoryRuntimeData.Value;
        ClearInventoryUI();
        SetupInventoryUI();
    }

    private void OnDisable()
    {
        Resources.UnloadUnusedAssets();
    }

    private void ClearInventoryUI()
    {
        foreach(Transform childXform in scrollViewContent.transform)
        {
            Debug.Log($"Destroying {childXform.gameObject.name}");
            Destroy(childXform.gameObject);
        }
    }

    public void SetItemNameText(string nameText)
    {
        itemNameText.text = nameText;
    }

    private void SetupInventoryUI()
    {
        InventoryController invController = InventoryController.Instance;
        List<string> weaponIDs = inventoryData.GetWeaponItemIDs();
        List <string> questItemIDs = inventoryData.GetQuestItemIDs();
        Dictionary<string, int> consumablesDict = inventoryData.GetConsumableIDsAndQuantities();
        //
        List<WeaponItemSO> weapons = new List<WeaponItemSO>();
        List<QuestItemSO> questItems = new List<QuestItemSO>();
        List<ConsumableItemSO> consumables = new List<ConsumableItemSO>();

        if (weaponIDs.Count > 0)
        {
            foreach (string _str in weaponIDs)
            {
                WeaponItemSO weaponData = invController.GetWeaponDataWithID(_str);
                weapons.Add(weaponData);
            }
        }
        if (questItemIDs.Count > 0)
        {
            foreach (string _str in questItemIDs)
            {
                QuestItemSO questData = invController.GetQuestItemDataWithID(_str);
                questItems.Add(questData);
            }
        }
        if (consumablesDict.Keys.ToList<string>().Count > 0)
        {
            foreach (string _str in consumablesDict.Keys.ToList<string>())
            {
                ConsumableItemSO consData = invController.GetConsumableItemData(_str);
                consumables.Add(consData);
            }
        }

        // Consumables
        if (consumables.Count > 0)
        {
            foreach (ConsumableItemSO consumableData in consumables)
            {
                string iconPath = consumableData.IconTexturePath;
                string displayName = consumableData.DisplayName;
                int amount = inventoryData.GetConsumableIDsAndQuantities()[consumableData.ItemID];
                GameObject itemObj = Instantiate(inventoryItemUIPrefab, scrollViewContent.transform);
                InventoryItemUIPrefab itemUI = itemObj.GetComponent<InventoryItemUIPrefab>();
                itemUI.Setup(this, iconPath, displayName, amount.ToString());
            }
        }

        // Weapons
        if (weapons.Count > 0)
        {
            foreach(WeaponItemSO weaponData in weapons)
            {
                string iconPath = weaponData.IconTexturePath;
                string displayName = weaponData.DisplayName;
                GameObject itemObj = Instantiate(inventoryItemUIPrefab, scrollViewContent.transform);
                InventoryItemUIPrefab itemUI = itemObj.GetComponent<InventoryItemUIPrefab>();
                itemUI.Setup(this, iconPath, displayName, string.Empty);
            }
        }

        // QuestItems
        if (questItems.Count > 0)
        {
            foreach(QuestItemSO questItemData in questItems)
            {
                string iconPath = questItemData.IconTexturePath;
                string displayName = questItemData.DisplayName;
                GameObject itemObj = Instantiate(inventoryItemUIPrefab, scrollViewContent.transform);
                InventoryItemUIPrefab itemUI = itemObj.GetComponent<InventoryItemUIPrefab>();
                itemUI.Setup(this, iconPath, displayName, string.Empty);
            }
        }

        



    }

}
