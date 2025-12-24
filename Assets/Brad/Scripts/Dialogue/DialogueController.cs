using UnityEngine;
using System.Collections.Generic;

public class DialogueController : Singleton<DialogueController>
{
    [SerializeField] private List<DialogueConversationSO> conversations;
    

    /////////////
    // Private //
    /////////////
    


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
