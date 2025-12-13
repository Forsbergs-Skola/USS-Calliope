using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRuntimeData", menuName = "Runtime Data Assets/PlayerRuntimeData")]
public class PlayerRuntimeData : ScriptableObject
{
    [System.NonSerialized] public PlayerData Value;
    private void OnEnable()
    {
        Value = null;
        if (DataController.Instance == null)
        {
            Value = new PlayerData(true);
        }
    }
}
