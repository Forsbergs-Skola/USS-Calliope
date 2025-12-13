using UnityEngine;
using Events;

public class GameEvents : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent newGameStartedEvent;
    [SerializeField] private GameDataPayloadEvent savedGameLoadedEvent;
    [SerializeField] private EmptyPayloadEvent dataServiceUpdatedEvent;

    public EmptyPayloadEvent NewGameStartedEvent { get => newGameStartedEvent; }
    public GameDataPayloadEvent SavedGameLoadedEvent { get => savedGameLoadedEvent; }
    public EmptyPayloadEvent DataServiceUpdatedEvent { get => dataServiceUpdatedEvent; }
}
