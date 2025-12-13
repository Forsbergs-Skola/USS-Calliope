using UnityEngine;

public class TestUI : MonoBehaviour
// This is also set up in a sandbox scene
{
    private void OnEnable()
    {
        if (EventRelay.Instance != null)
        {
            EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered += HandleOnDataChanged;
        }
    }

    private void OnDisable()
    {
        if (EventRelay.Instance != null)
        {
            EventRelay.Instance.GameEvents.DataUpdatedEvent.OnEventTriggered -= HandleOnDataChanged;
        }
    }

    private void HandleOnDataChanged()
    {
        // I have verified that I only see output here when
        // I load the sandbox scene from the bootstrap scene :)


        if (DataController.Instance == null) return;
        int health = DataController.Instance.PlayerRuntimeData.Value.Health;
        Debug.Log($"Player Health: {health}");
    }
}
