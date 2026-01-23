using UnityEngine;

public static class KeyedDoorProgressionHandler
{
    public static void HandleDoorUnlocked(string keyID)
    {
        if (DataController.Instance == null) return;

        bool isHandled = false;

        switch (keyID)
        {
            case IDConstants.CREW_QUARTERS_KEY:
                isHandled = DataController.Instance.ProgressionRuntimeData.Value.CrewQuartersUnlocked == true;
                if (!isHandled)
                {
                    DataController.Instance.ProgressionRuntimeData.Value.CrewQuartersUnlocked = true;
                }
                break;
            case IDConstants.BRIDGE_KEY:
                isHandled = DataController.Instance.ProgressionRuntimeData.Value.BridgeUnlocked == true;
                if (!isHandled)
                {
                    DataController.Instance.ProgressionRuntimeData.Value.BridgeUnlocked = true;
                }
                break;
            // and so on...

            default: return;
        }
    }
}
