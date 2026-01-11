using UnityEngine;

[CreateAssetMenu(fileName = "PatrolZone", menuName = "NPCs/Patrol Zone")]
public class PatrolZone : ScriptableObject
{
    [SerializeField] private string zoneID;
    [SerializeField] private int roomID;

    public string ZoneID => zoneID;
    public int RoomID => roomID;

    // This is only for Unity, this is not for builds
    // This autocomplete the ID with the name
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(zoneID))
            zoneID = name;
    }
#endif
}