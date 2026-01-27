using UnityEngine;

[CreateAssetMenu(fileName = "UISoundCatalogSO", menuName = "UI Sounds/UISoundCatalogSO")]
public class UISoundCatalogSO : ScriptableObject
{
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip pressClip;
    [SerializeField] private AudioClip pauseScreenSound;
    [SerializeField] private AudioClip logoSound;
    [SerializeField] private AudioClip scarySound;

    public AudioClip HoverClip { get => hoverClip; }
    public AudioClip PressClip { get => pressClip; }
    public AudioClip PauseScreenSound { get => pauseScreenSound; }
    public AudioClip LogoSound { get => logoSound; }

    public AudioClip ScarySound { get => scarySound; }

}
