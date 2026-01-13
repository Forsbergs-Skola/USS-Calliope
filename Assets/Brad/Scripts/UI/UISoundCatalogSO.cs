using UnityEngine;

[CreateAssetMenu(fileName = "UISoundCatalogSO", menuName = "UI Sounds/UISoundCatalogSO")]
public class UISoundCatalogSO : ScriptableObject
{
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip pressClip;
    [SerializeField] private AudioClip pauseScreenSound;

    public AudioClip HoverClip { get => hoverClip; }
    public AudioClip PressClip { get => pressClip; }
    public AudioClip PauseScreenSound { get => pauseScreenSound; }

}
