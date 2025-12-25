using UnityEngine;
using Events;
using System.Collections.Generic;

public class DialogueController : Singleton<DialogueController>
{
    [SerializeField] private List<DialogueConversationSO> conversations;

    private void Start()
    {
        EventRelay.Instance.UIEvents.DialogueConvoStartedEVent.OnEventTriggered += HandleConvoStartedEvent;
        EventRelay.Instance.UIEvents.DialogueConvoFinishedEvent.OnEventTriggered += HandleConvoEndedEvent;
        EventRelay.Instance.UIEvents.DialogueLineStartedEVent.OnEventTriggered += HandleLineStartedEvent;
        EventRelay.Instance.UIEvents.DialogueLineFinishedEvent.OnEventTriggered += HandleLineFinishedEvent;
    }
    private void OnDestroy()
    {
        EventRelay.Instance.UIEvents.DialogueConvoStartedEVent.OnEventTriggered -= HandleConvoStartedEvent;
        EventRelay.Instance.UIEvents.DialogueConvoFinishedEvent.OnEventTriggered -= HandleConvoEndedEvent;
        EventRelay.Instance.UIEvents.DialogueLineStartedEVent.OnEventTriggered -= HandleLineStartedEvent;
        EventRelay.Instance.UIEvents.DialogueLineFinishedEvent.OnEventTriggered -= HandleLineFinishedEvent;
    }


    /////////////
    // Private //
    /////////////

    private void HandleConvoStartedEvent(string convoID)
    {

    }
    private void HandleConvoEndedEvent(string convoID)
    {
        if (!UIController.Instance.GetIsCanvasUp(EnumCanvasUIName.DIALOGUE)) return;
        UIController.Instance.RemoveCanvas(EnumCanvasUIName.DIALOGUE);

    }
    private void HandleLineStartedEvent(string lineID)
    {

    }
    private void HandleLineFinishedEvent(string lineID)
    {

    }


    /////////
    // API //
    /////////

    public void StartConvoWithID(string convoID)
    {
        DialogueConversationSO convo = conversations.Find(conv => conv.ConvoID == convoID);
        if (convo != null)
        {
            UIController.Instance.ShowCanvas(EnumCanvasUIName.DIALOGUE);
            if (UIController.Instance.GetIsCanvasUp(EnumCanvasUIName.DIALOGUE))
            {
                DialogueCanvas dialogueCanvas = UIController.Instance.GetReferenceToCanvas(EnumCanvasUIName.DIALOGUE) as DialogueCanvas;
                dialogueCanvas.LaunchConversation(convo);
            }

        }
    }



}
