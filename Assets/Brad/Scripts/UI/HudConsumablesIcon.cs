using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudConsumablesIcon : MonoBehaviour, IHudConsumableIcon
{
    [SerializeField] private TMP_Text quantityText;
    private RawImage image;

    private void Awake()
    {
        image = GetComponent<RawImage>();
    }

    public void SetupIcon(ConsumableItemSO consumableData, int qty)
    {
        string stexturePath = consumableData.IconTexturePath;
        string quantityString = qty.ToString();
        // set the texture of image
        // set the text of quantityText
    }
}
