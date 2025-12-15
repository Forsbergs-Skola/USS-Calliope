///////////////////////////
// EVENT RELAY SINGLETON //
///////////////////////////

using UnityEngine;

public class EventRelay : Singleton<EventRelay>
{
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private PlayerEvents playerEvents;
    [SerializeField] private UIEvents uiEvents;
    //...and so on for future event classes

    public GameEvents GameEvents { get => gameEvents; }
    public PlayerEvents PlayerEvents { get => playerEvents; }
    public UIEvents UIEvents { get => uiEvents; }
    // ...and so on
}