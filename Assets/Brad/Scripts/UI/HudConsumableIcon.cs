using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudConsumableIcon : MonoBehaviour
{
    [SerializeField] private TMP_Text qtyText;
    [SerializeField] private RawImage background;
    [SerializeField] private RawImage foreground;
    private RawImage image;

    private void Awake()
    {
        image = GetComponent<RawImage>();
    }

    public void Setup(string texturePath, int qty)
    {
        //image.texture = Resources.Load<Texture>(texturePath);
        foreground.texture = Resources.Load<Texture>(texturePath);
        qtyText.text = qty.ToString();
    }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }

}
