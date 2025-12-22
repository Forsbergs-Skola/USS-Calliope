using UnityEngine;
using Events;

public class UIEvents : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent logoSplashFinishedEvent;

    public EmptyPayloadEvent LogoSplashFinishedEvent { get => logoSplashFinishedEvent; }
}
