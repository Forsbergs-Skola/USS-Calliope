using UnityEngine;

[CreateAssetMenu(fileName = "ProgressionRuntimeData", menuName = "Runtime Data Assets/ProgressionRuntimeData")]
public class ProgressionRuntimeData : ScriptableObject
{
    [System.NonSerialized] public ProgressionData Value;
    private void OnEnable()
    {
        Value = null;
        if (DataController.Instance == null)
        {
            Value = new ProgressionData(true);
        }
    }
}
