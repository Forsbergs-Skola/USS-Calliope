using UnityEngine;
using UnityEngine.UI;
using Events;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private EmptyPayloadEvent newGamePressedEvent;

    private void OnEnable()
    {
        newGameButton.onClick.AddListener(HandleNewGamePressed);
    }
    private void OnDisable()
    {
        newGameButton.onClick.RemoveAllListeners();
    }

    private void HandleNewGamePressed()
    {
        newGamePressedEvent.TriggerEvent();
    }
}
