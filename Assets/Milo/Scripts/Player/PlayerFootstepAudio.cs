using UnityEngine;

public class PlayerFootstepAudio : MonoBehaviour
{
    [Header("Footstep Clips")]
    [SerializeField] private AudioClip[] footstepClips;

    [Header("Settings")]
    [SerializeField] private float volume = 0.35f;
    [SerializeField] private float pitchVariation = 0.05f;

    private int _lastIndex = -1;

    public AudioClip[] FootstepClips { get => footstepClips; set => footstepClips = value; }
    public float Volume { get => volume; set => volume = value; }
    public float PitchVariation { get => pitchVariation; set => pitchVariation = value; }
    public int LastIndex { get => _lastIndex; set => _lastIndex = value; }

    public void PlayFootstep()
    {
        int count = FootstepClips.Length;
        if (count == 0) return;

        int index = (count > 1) ? Random.Range(0, count - 1) : 0;
        if (count > 1 && index >= LastIndex && LastIndex != -1) index++;
        LastIndex = index;

        AudioSource pooledSource = AudioPoolManager.Instance.GetSource();

        float pitch = 1f + Random.Range(-PitchVariation, PitchVariation);

        if (pooledSource.TryGetComponent(out PlayerAudioPoolHandler handler))
        {
            handler.Play(FootstepClips[index], Volume, pitch);
        }
    }
}