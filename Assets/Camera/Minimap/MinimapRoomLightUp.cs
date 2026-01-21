using UnityEngine;
using TMPro;

public class MinimapRoomLightUp : MonoBehaviour
{
    public Color inactiveColor = new Color(0f, 1f, 0.5f, 0.3f);
    public Color activeColor   = new Color(0f, 1f, 0.7f, 1f);

    TextMeshProUGUI tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        if (!tmp) return;
        tmp.color = active ? activeColor : inactiveColor;
    }
}