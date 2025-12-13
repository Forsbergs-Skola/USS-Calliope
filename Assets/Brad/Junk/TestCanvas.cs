using UnityEngine;

public class TestCanvas : MonoBehaviour
{
    public void NewGame()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.TriggerEvent();
    }
}
