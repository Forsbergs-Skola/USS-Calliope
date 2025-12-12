using UnityEngine;
using Events;

public class GameEvents : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent newGameStartedEvent;

    public EmptyPayloadEvent NewGameStartedEvent { get => newGameStartedEvent; }
}
