using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_LevelAudioCatalog", menuName = "LevelSounds/SO_LevelAudioCatalog")]
public class SO_LevelAudioCatalog : ScriptableObject
{
    [SerializeField] private List<SO_LevelAudioClip> levelAudioClips = new List<SO_LevelAudioClip>();

    public List<SO_LevelAudioClip> LevelAudioClips => levelAudioClips;
}
