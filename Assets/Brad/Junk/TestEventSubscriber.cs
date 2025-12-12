using UnityEngine;

public class TestEventSubscriber : MonoBehaviour
{
    private void Start()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered += HandleTestEvent;
    }
    private void OnDestroy()
    {
        EventRelay.Instance.GameEvents.NewGameStartedEvent.OnEventTriggered -= HandleTestEvent;
    }

    private void HandleTestEvent()
    {
        Debug.Log("TEST!!");
    }

}
