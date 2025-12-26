using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItemUIPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //[SerializeField] private RawImage iconImage;
    [SerializeField] private TMP_Text amountText;
    private string testID = string.Empty;
    private InventoryPanel inventoryPanel = null;

    private string nameText = string.Empty;


    private void OnEnable()
    {
        testID = System.Guid.NewGuid().ToString();
    }
    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }

    public void OnPointerEnter(PointerEventData pointerData)
    {
        if (inventoryPanel == null) return;
        inventoryPanel.SetItemNameText(nameText);
        //Debug.Log($"Pointer Enter: {testID}");
    }
    public void OnPointerExit(PointerEventData pointerData)
    {
        if (inventoryPanel == null) return;
        inventoryPanel.SetItemNameText(string.Empty);
        //Debug.Log($"Pointer Exit: {testID}");
    }
    public void OnPointerClick(PointerEventData pointerData)
    {
        if (inventoryPanel == null) return;
        //Debug.Log($"Click on: {testID}");
    }

    public void Setup(InventoryPanel _invPanel, string texturePath, string displayName, string _amountText)
    {
        inventoryPanel = _invPanel;
        RawImage img = GetComponent<RawImage>();
        img.texture = Resources.Load<Texture>(texturePath);
        amountText.text = _amountText;

        nameText = displayName;
    }

}
