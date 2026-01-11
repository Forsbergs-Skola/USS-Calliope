using System.Collections;
using UnityEngine;

public class DynamicAmbience : MonoBehaviour
{
    public static DynamicAmbience Instance;

    [Header("Global Audio Source")]
    public AudioSource audioSource;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void PlayZoneAmbience(AudioClip clip, float volume, float fadeDuration)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToClip(clip, volume, fadeDuration));
    }

    private IEnumerator FadeToClip(AudioClip clip, float volume, float fadeDuration)
    {
        if (audioSource.isPlaying)
            yield return AudioEffects.FadeOut(audioSource, fadeDuration);

        audioSource.clip = clip;
        audioSource.volume = 0f;
        audioSource.Play();

        yield return AudioEffects.FadeIn(audioSource, fadeDuration, volume);
    }
}
