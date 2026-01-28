using UnityEngine;
using TMPro;

public class MinimapRoomLightUp : MonoBehaviour
{
    public Color inactiveColor = new Color(0f, 1f, 0.5f, 0.3f);
    public Color activeColor   = new Color(0f, 1f, 0.7f, 1f);

    TextMeshProUGUI tmp;
    private bool isActive;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        ApplyColor();
    }

    void OnEnable()
    {
        if (!tmp) tmp = GetComponent<TextMeshProUGUI>();
        ApplyColor();
    }

    public void SetActive(bool active)
    {
        isActive = active;
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (!tmp) return;
        tmp.color = isActive ? activeColor : inactiveColor;
    }
}