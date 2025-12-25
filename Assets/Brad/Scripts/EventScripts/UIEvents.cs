using UnityEngine;
using Events;

public class UIEvents : MonoBehaviour
{
    [SerializeField] private EmptyPayloadEvent logoSplashFinishedEvent;
    [SerializeField] private StringPayloadEvent dialogueLineStartedEvent;
    [SerializeField] private StringPayloadEvent dialogueLineFinishedEvent;
    [SerializeField] private StringPayloadEvent dialogueConvoStartedEvent;
    [SerializeField] private StringPayloadEvent dialogueConvoFinishedEvent;

    public EmptyPayloadEvent LogoSplashFinishedEvent { get => logoSplashFinishedEvent; }
    public StringPayloadEvent DialogueLineStartedEVent { get => dialogueLineStartedEvent; }
    public StringPayloadEvent DialogueLineFinishedEvent { get => dialogueLineFinishedEvent; }
    public StringPayloadEvent DialogueConvoStartedEVent { get => dialogueConvoStartedEvent; }
    public StringPayloadEvent DialogueConvoFinishedEvent { get => dialogueConvoFinishedEvent; }
}
