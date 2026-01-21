using UnityEngine;

[CreateAssetMenu(fileName = "SO_AudioClip", menuName = "LevelSounds/SO_AudioClip")]
public class SO_LevelAudioClip : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private bool hasBeenPlayed;
    public AudioClip AudioClip => audioClip;
    public bool HasBeenPlayed => hasBeenPlayed;
    public string Id => id;

    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(id)) return;
        id = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
