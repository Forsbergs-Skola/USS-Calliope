using UnityEngine;

public class TestEventPublisher : MonoBehaviour
{


    float _ta = 0.0f;
    float _ui = 2.0f;

    private void Update()
    {
        _ta += Time.deltaTime;
        if (_ta < _ui) return;
        _ta = 0.0f;

        EventRelay.Instance.GameEvents.NewGameStartedEvent.TriggerEvent();
    }


}
