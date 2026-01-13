using UnityEngine;
using System.Collections.Generic;

public enum EnumUISound
{
    HOVER,
    PRESS,
    PAUSE_SCREEN
}

public class UISoundPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioSource> players;
    [SerializeField] private UISoundCatalogSO uiSoundCatalog;

    Dictionary<EnumUISound, AudioClip> clipDict = new();

    private void Awake()
    {
        clipDict[EnumUISound.HOVER]         = uiSoundCatalog.HoverClip;
        clipDict[EnumUISound.PRESS]         = uiSoundCatalog.PressClip;
        clipDict[EnumUISound.PAUSE_SCREEN]  = uiSoundCatalog.PauseScreenSound;
    }

    public void PlayUISound(EnumUISound sound)
    {
        if (!clipDict.ContainsKey(sound)) return;
        if (!TryGetFirstAvailableSource(out AudioSource _player)) return;
        if (_player.clip != clipDict[sound]) _player.clip = clipDict[sound];
        _player.Play();
    }

    private bool TryGetFirstAvailableSource(out AudioSource source)
    {
        foreach(AudioSource _source in players)
        {
            if (!_source.isPlaying)
            {
                source = _source;
                return true;
            }
        }

        source = null;
        return false;
    }
}
