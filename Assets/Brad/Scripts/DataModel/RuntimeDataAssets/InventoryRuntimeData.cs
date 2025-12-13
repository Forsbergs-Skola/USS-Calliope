using UnityEngine;

[CreateAssetMenu(fileName = "InventoryRuntimeData", menuName = "Runtime Data Assets/InventoryRuntimeData")]
public class InventoryRuntimeData : ScriptableObject
{
    [System.NonSerialized] public InventoryData Value;
    private void OnEnable()
    {
        Value = null;
        if (DataController.Instance == null)
        {
            Value = new InventoryData(true);
        }
    }
}
