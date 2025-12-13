using UnityEngine;
using Events;

public class GameEvents : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent newGameStartedEvent;
    [SerializeField] private GameDataPayloadEvent savedGameLoadedEvent;
    [SerializeField] private EmptyPayloadEvent dataUpdatedEvent;

    public EmptyPayloadEvent NewGameStartedEvent { get => newGameStartedEvent; }
    public GameDataPayloadEvent SavedGameLoadedEvent { get => savedGameLoadedEvent; }
    public EmptyPayloadEvent DataUpdatedEvent { get => dataUpdatedEvent; }
}
