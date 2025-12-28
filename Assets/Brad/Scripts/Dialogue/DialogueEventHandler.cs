using UnityEngine;

public class DialogueEventHandler : MonoBehaviour
{
    ProgressionData progData { get => DataController.Instance.ProgressionRuntimeData.Value; }


    public void HandleConversationFinished(string convoID)
    {
        switch (convoID)
        {
            case IDConstants.CONVERSATION_BOB_00:
                Bob_00Finished();
                break;
        }
    }

    private void Bob_00Finished()
    {
        progData.BobContacted = true;
    }



}
