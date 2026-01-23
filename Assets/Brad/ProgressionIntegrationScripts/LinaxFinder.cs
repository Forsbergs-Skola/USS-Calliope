using UnityEngine;

public static class LinaxFinder
{
    public static bool TrySetLinaxFound(bool linaxFound)
    {
        if (DataController.Instance == null) return false;
        DataController.Instance.ProgressionRuntimeData.Value.LinaxFound = linaxFound;
        DataController.Instance.InventoryRuntimeData.Value.AddQuestItem(IDConstants.BRIDGE_KEY);
        return true;
    }
}
