using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioPoolHandler : MonoBehaviour
{
    private AudioSource _source;

    void Awake() => _source = GetComponent<AudioSource>();

    public void Play(AudioClip clip, float volume, float pitch)
    {
        _source.clip = clip;
        _source.volume = volume;
        _source.pitch = pitch;
        _source.Play();

        // Start checking for completion
        StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        // Wait until the clip is done playing
        yield return new WaitWhile(() => _source.isPlaying);
        AudioPoolManager.Instance.ReturnToPool(_source);
    }
}