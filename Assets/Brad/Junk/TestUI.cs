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
        if (DataController.Instance == null) return;

        int health = DataController.Instance.PlayerRuntimeData.Value.Health;
        Debug.Log($"Player Health: {health}");

        int xp = DataController.Instance.PlayerRuntimeData.Value.XP;
        Debug.Log($"Player XP: {xp}");
    }
}
