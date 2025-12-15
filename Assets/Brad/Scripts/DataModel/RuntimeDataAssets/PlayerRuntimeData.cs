using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRuntimeData", menuName = "Runtime Data Assets/PlayerRuntimeData")]
public class PlayerRuntimeData : ScriptableObject
{
    [System.NonSerialized] public PlayerData Value;
    private void OnEnable()
    {
        Value = null;
        if (DataController.Instance == null) // If I am loaded into a sandbox scene...
        {
            Value = new PlayerData(true); // Initialize with IsSandbox = true
        }
    }
}
