using UnityEngine;

public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("Footstep Clips")]
    [SerializeField] private AudioClip[] footstepClips;

    [Header("Settings")]
    [SerializeField] private float volume = 0.35f;
    [SerializeField] private float pitchVariation = 0.05f;

    private int _lastIndex = -1;

    public void PlayFootstep()
    {
        int count = footstepClips.Length;
        if (count == 0) return;

        int index = (count > 1) ? Random.Range(0, count - 1) : 0;
        if (count > 1 && index >= _lastIndex && _lastIndex != -1) index++;
        _lastIndex = index;

        AudioSource pooledSource = AudioPoolManager.Instance.GetSource();

        float pitch = 1f + Random.Range(-pitchVariation, pitchVariation);

        if (pooledSource.TryGetComponent(out PlayerAudioPoolHandler handler))
        {
            handler.Play(footstepClips[index], volume, pitch);
        }
    }
}