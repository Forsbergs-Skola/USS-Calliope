using UnityEngine;

public class DialogueEventHandler
{
    ProgressionData progData { get => DataController.Instance.ProgressionRuntimeData.Value; }


    public void HandleConversationFinished(string convoID)
    {
        switch (convoID)
        {
            case IDConstants.CONVERSATION_BOB_00:
                Bob_00Finished();
                break;
            case IDConstants.CONVERSATION_BOB_01:
                Bob_01Finished();
                break;
        }
    }

    private void Bob_00Finished()
    {
        progData.BobContacted = true;
    }
    private void Bob_01Finished()
    {
        InventoryData invData = DataController.Instance.InventoryRuntimeData.Value;
        invData.RemoveQuestItem(IDConstants.INFECTED_SAMPLE);
        invData.AddQuestItem(IDConstants.CREW_QUARTERS_KEY);
    }
}
