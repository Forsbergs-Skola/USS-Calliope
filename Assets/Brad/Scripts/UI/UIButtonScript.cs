using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{

    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Color upColor = new Color(1f, 1f, 0f, 1f);
    [SerializeField] private Color downColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [Range(0.0f, 1.0f)][SerializeField] private float embiggenFactor = 0.1f;

    private float defaultTextSize;

    private void OnEnable()
    {
        defaultTextSize = buttonText.fontSize;
        buttonText.color = upColor;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        buttonText.fontSize = defaultTextSize * (1f + embiggenFactor);
    }
    public void OnPointerExit(PointerEventData data)
    {
        buttonText.fontSize = defaultTextSize;
    }

    public void OnPointerUp(PointerEventData data)
    {
        buttonText.color = upColor;
    }
    public void OnPointerDown(PointerEventData data)
    {
        buttonText.color = downColor;
    }
}
