using UnityEngine;

public class AudioClipEmitter : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SO_LevelAudioClip levelAudio;
    [SerializeField] private bool isTriggerAudio;

    void Start()
    {
        if (!isTriggerAudio)
        {
            PlayClip();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isTriggerAudio) return;

        PlayClip();
        Destroy(this);
    }

    void PlayClip()
    {
        if (!audioSource || !levelAudio) return;
        audioSource.PlayOneShot(levelAudio.AudioClip);
    }
}