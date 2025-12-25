using UnityEngine;
using Events;

public class GameEvents : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent newGameStartedEvent;
    [SerializeField] private GameDataPayloadEvent savedGameLoadedEvent;
    [SerializeField] private EmptyPayloadEvent dataUpdatedEvent;
    [SerializeField] private IRuntimeDataPayloadEvent runtimeDataUpdatedEvent;
    [SerializeField] private StringPayloadEvent itemPickupEvent;

    public EmptyPayloadEvent NewGameStartedEvent { get => newGameStartedEvent; }
    public GameDataPayloadEvent SavedGameLoadedEvent { get => savedGameLoadedEvent; }
    public EmptyPayloadEvent DataUpdatedEvent { get => dataUpdatedEvent; }
    public IRuntimeDataPayloadEvent RuntimeDataUpdatedEvent { get => runtimeDataUpdatedEvent; }
    public StringPayloadEvent ItemPickupEvent { get => itemPickupEvent; }
}
